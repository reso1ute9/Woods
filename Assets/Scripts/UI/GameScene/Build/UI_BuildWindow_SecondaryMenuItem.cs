using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;
using Unity.VisualScripting;


// 合成窗口的二级菜单选项
public class UI_BuildWindow_SecondaryMenuItem : MonoBehaviour
{
    [SerializeField] Image bgImage;
    [SerializeField] Button button;
    [SerializeField] Image iconImage;
    [SerializeField] Sprite[] bgSprites;                    // 默认和选中时的图片

    public BuildConfig buildConfig { get; private set; }    // 当前选项代表的建造配置
    private UI_BuildWindow_SecondaryMenu ownerWindow;       // 合成窗口二级菜单
    
    private void Start() {
        UITool.BindMouseEffect(this);
        button.onClick.AddListener(OnClick);
    }

    public void Init(BuildConfig buildConfig, UI_BuildWindow_SecondaryMenu ownerWindow) {
        this.buildConfig = buildConfig;
        this.ownerWindow = ownerWindow;

        if (this.buildConfig.buildType == BuildType.Weapon) {
            iconImage.sprite = ConfigManager.Instance.GetConfig<ItemConfig>(ConfigName.Item, buildConfig.targetId).itemIcon;
        } else {
            iconImage.sprite = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, buildConfig.targetId).mapIconSprite;
        }
        if (buildConfig.CheckBuildConfigCondition()) {
            iconImage.color = Color.white;
        } else {
            iconImage.color = Color.black;
        }
        UnSelect();
    }

    private void OnClick() {
        ownerWindow.SelectSecondaryMenuItem(this);
    }

    public void Select() {
        bgImage.sprite = bgSprites[1];
    }

    public void UnSelect() {
        bgImage.sprite = bgSprites[0];
    }
}
