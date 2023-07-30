using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;


[UIElement(false, "UI/UI_InventoryWindow", 1)]
public class UI_InventoryWindow : UI_WindowBase
{
    private InventoryData inventoryData;
    [SerializeField] public UI_ItemSlot[] slots;          // 物品槽
    [SerializeField] public UI_ItemSlot weaponSlot;       // 装备槽
    public Sprite[] bgSprite;                       // 框图
    
    public override void Init() {
        base.Init();
        // 确定物品快捷栏存档数据
        inventoryData = ArchiveManager.Instance.inventoryData;
        // 初始化每个槽对应的位置和父窗口信息
        for (int i = 0; i < slots.Length; i++) {
            slots[i].Init(i, this);
        }
        weaponSlot.Init(slots.Length, this);
        UI_ItemSlot.weaponSlot = weaponSlot;
    }

    public override void OnShow() {
        base.OnShow();
        // 根据存档复原物品快捷栏格子
        InitData(inventoryData);

    }

    // 初始化物品快捷栏所有格子UI界面
    private void InitData(InventoryData data) {
        for (int i = 0; i < data.itemDatas.Length; i++) {
            // 初始化每个槽中物品对应的各种UI信息
            slots[i].InitData(data.itemDatas[i]);
        }
        weaponSlot.InitData(data.weaponSlotItemData);
    }

    public void AddItem() {

    }

    // 从物品快捷栏中移除index处的格子
    public void RemoveItem(int index) {
        // 判断是否为移除武器还是移除普通格子
        if (index == inventoryData.itemDatas.Length) {
            inventoryData.RemoveWeaponItem();
        } else {
            inventoryData.RemoveItem(index);
        }
    }

    public void SetItem(int index, ItemData itemData) {
        // 判断是否为为武器还是普通格子
        if (index == inventoryData.itemDatas.Length) {
            inventoryData.SetWeaponItem(itemData);
        } else {
            inventoryData.SetItem(index, itemData);
        }
    }
}
