using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using System;

// 物品快捷栏数据
[Serializable]
public class InventoryData
{
    // 物品快捷栏中装的物品
    public ItemData[] itemDatas { get; private set; }
    public ItemData weaponSlotItemData { get; private set; }

    public InventoryData(int itemCount) {
        itemDatas = new ItemData[itemCount];
    }

    // 移除某一个物品
    public void RemoveItem(int index) {
        itemDatas[index] = null;
    }

    // 放置某一个物品
    public void SetItem(int index, ItemData itemData) {
        itemDatas[index] = itemData;
    }

    // 移除武器
    public void RemoveWeaponItem() {
        weaponSlotItemData = null;
    }

    // 放置武器
    public void SetWeaponItem(ItemData itemData) {
        weaponSlotItemData = itemData;
    }
}
