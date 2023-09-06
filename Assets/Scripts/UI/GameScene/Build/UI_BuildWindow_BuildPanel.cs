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

    public void Init() {
        button.onClick.AddListener(OnClick);
        Close();
    }

    private void OnClick() {

    }

    public void Show(BuildConfig buildConfig, bool isMeetCondition) {
        this.buildConfig = buildConfig;
        // 显示合成需要物品
        for (int i = 0; i < buildConfig.buildConfigConditions.Count; i++) {
            int configId = buildConfig.buildConfigConditions[i].itemId;
            int currentCount = UI_InventoryWindow.Instance.GetItemCount(configId);
            int needCount = buildConfig.buildConfigConditions[i].count;
            buildPanelItems[i].Show(configId, currentCount, needCount);
        }
        descriptionText.text = ConfigManager.Instance.GetConfig<ItemConfig>(ConfigName.Item, buildConfig.targetId).descript;
        if (isMeetCondition == false) {
            button.interactable = false;
        }
        gameObject.SetActive(true);
    }

    public void Close() {
        for (int i = 0; i < buildPanelItems.Length; i++) {
            buildPanelItems[i].Close();
        }
        gameObject.SetActive(false);
    }
}
