using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;

// UI建造窗口主菜单一级菜单
public class UI_BuildWindow_MainMenuItem : MonoBehaviour
{
    [SerializeField] Image bgImage;
    [SerializeField] Button button;
    [SerializeField] Image iconImage;
    [SerializeField] Sprite[] bgSprite;     // 默认和选中时不同的精灵图片

    public BuildType MenuType { get; private set; }
    private UI_BuildWindow ownerWindow;     // 父窗口
    
    public void Init(BuildType buildType, UI_BuildWindow ownerWindow) {
        this.MenuType = buildType;
        this.ownerWindow = ownerWindow;
        UITool.BindMouseEffect(this);
        button.onClick.AddListener(OnClick);
        UnSelect();
    }

    private void OnClick() {
        ownerWindow.SelectMainMenuItem(this);
    }

    // 选中效果
    public void Select() {
        bgImage.sprite = bgSprite[1];
    }

    // 未选中效果
    public void UnSelect() {
        // 默认第0个图片为默认背景图片
        bgImage.sprite = bgSprite[0];
    }
}
