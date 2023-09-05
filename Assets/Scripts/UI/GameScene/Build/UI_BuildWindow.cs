using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using JKFrame;
using UnityEngine;
using UnityEngine.UI;


[UIElement(false, "UI/UI_BuildWindow", 1)]
public class UI_BuildWindow : UI_WindowBase
{
    [SerializeField] UI_BuildWindow_MainMenuItem[] mainMenuItems;   // 所有的一级菜单选项
    private UI_BuildWindow_MainMenuItem currentMenuItem;            // 当前被选中的菜单选项

    public override void Init() {
        base.Init();
        // 初始化子窗口
        for (int i = 0; i < mainMenuItems.Length; i++) {
            mainMenuItems[i].Init((BuildType)i, this);
        }
    }

    // 选择一级菜单选项
    public void SelectMainMenuItem(UI_BuildWindow_MainMenuItem mainMenuItem) {
        if (currentMenuItem != null) {
            currentMenuItem.UnSelect();
        }
        currentMenuItem = mainMenuItem;
        currentMenuItem.Select();

        // TODO: 开启二级菜单
        UnityEngine.Debug.Log("开启二级菜单:" + currentMenuItem.MenuType.ToString());
    }
}
