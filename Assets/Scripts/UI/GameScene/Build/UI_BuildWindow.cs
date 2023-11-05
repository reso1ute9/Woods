using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using JKFrame;
using UnityEngine;
using UnityEngine.UI;
using System;


[UIElement(false, "UI/UI_BuildWindow", 1)]
public class UI_BuildWindow : UI_WindowBase
{
    [SerializeField] UI_BuildWindow_MainMenuItem[] mainMenuItems;   // 所有的一级菜单选项
    [SerializeField] UI_BuildWindow_SecondaryMenu  secondaryMenu;   // 二级菜单
    private UI_BuildWindow_MainMenuItem currentMenuItem;            // 当前被选中的菜单选项
    private bool isTouch = false;                                   // 检查鼠标是否在UI_BuildWindow内

    public override void Init() {
        base.Init();
        // 初始化子窗口
        for (int i = 0; i < mainMenuItems.Length; i++) {
            mainMenuItems[i].Init((BuildType)i, this);
        }
        // 初始化二级菜单
        secondaryMenu.Init();
    }

    public void Update() {
        // 检查鼠标是否在UI_BuildWindow内
        if (isTouch == true) {
            // 判断一个点或一张图片是否在另一张图片区域内, 一共有两个参数
            // 第一个填入RectTransform类型，把作为区域的大图拖进来就行。
            // 第二个需要填入一个vector2，这是一个屏幕坐标的点，如果是需要判断鼠标的位置是否在这个图片范围内的话，直接传入Input.mousePosition就行了
            if (RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition) == false) {
                isTouch = false;
                CloseMenu();
                currentMenuItem.UnSelect();
            }
        }
    }

    // 选择一级菜单选项
    public void SelectMainMenuItem(UI_BuildWindow_MainMenuItem mainMenuItem) {
        if (currentMenuItem != null) {
            currentMenuItem.UnSelect();
        }
        currentMenuItem = mainMenuItem;
        currentMenuItem.Select();
        // 开启二级菜单
        secondaryMenu.Show(mainMenuItem.MenuType);

        isTouch = true;
    }

    private void CloseMenu() {
        secondaryMenu.Close();
    }
}
