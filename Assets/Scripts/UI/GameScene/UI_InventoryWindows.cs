using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;


[UIElement(false, "UI/UI_InventoryWindow", 1)]
public class UI_InventoryWindows : UI_WindowBase
{
    private InventoryData inventoryData;

    public override void OnShow()
    {
        base.OnShow();
        // 根据存档复原
        inventoryData = ArchiveManager.Instance.inventoryData;
    }

    private void InitData() {
        // TODO: 基于存档初始化物品快捷栏所有格子
    }

    public void AddItem() {

    }

    public void RemoveItem(int index) {

    }

    public void SetItem(int index, ItemData itemData) {

    }
}
