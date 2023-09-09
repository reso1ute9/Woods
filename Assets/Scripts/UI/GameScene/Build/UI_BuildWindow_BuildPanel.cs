using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;
using Sirenix.Utilities;

public class UI_BuildWindow_BuildPanel : MonoBehaviour
{
    [SerializeField] UI_BuildWindow_BuildPanelItem[] buildPanelItems;
    [SerializeField] Text descriptionText;
    [SerializeField] Button button;
    private BuildConfig buildConfig;
    private UI_BuildWindow_SecondaryMenu ownerWindow;

    public void Init(UI_BuildWindow_SecondaryMenu ownerWindow) {
        this.ownerWindow = ownerWindow;
        button.onClick.AddListener(OnClick);
        Close();
    }

    private void OnClick() {
        if (buildConfig.buildType == BuildType.Weapon) {
            if (UI_InventoryWindow.Instance.AddItemAndPlayAudio(buildConfig.targetId)) {
                // 根据建造配置减少背包中的物品
                UI_InventoryWindow.Instance.UpdateItemsForBuild(buildConfig);
                // 刷新当前二三级窗口状态
                RefreshView();
            } else {
                UIManager.Instance.AddTips("背包没有空间了");
            }
        } else {
            // 进入建造模式
            EventManager.EventTrigger<BuildConfig>(EventName.BuildBuilding, buildConfig);
            // 关闭二三级菜单
            ownerWindow.Close();
        }
    }

    public void Show(BuildConfig buildConfig) {
        this.buildConfig = buildConfig;
        // 显示合成需要物品
        for (int i = 0; i < buildConfig.buildConfigConditions.Count; i++) {
            int configId = buildConfig.buildConfigConditions[i].itemId;
            int currentCount = UI_InventoryWindow.Instance.GetItemCount(configId);
            int needCount = buildConfig.buildConfigConditions[i].count;
            buildPanelItems[i].Show(configId, currentCount, needCount);
        }
        if (buildConfig.buildType == BuildType.Weapon) {
            descriptionText.text = ConfigManager.Instance.GetConfig<ItemConfig>(ConfigName.Item, buildConfig.targetId).descript;
        } else {
            descriptionText.text = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, buildConfig.targetId).descript;
        }        
        button.interactable = buildConfig.CheckBuildConfigCondition();
        gameObject.SetActive(true);
        
    }

    public void RefreshView() {
        Show(buildConfig);
        // 刷新二级窗口
        ownerWindow.RefreshView();
    }

    public void Close() {
        for (int i = 0; i < buildPanelItems.Length; i++) {
            buildPanelItems[i].Close();
        }
        gameObject.SetActive(false);
    }
}
