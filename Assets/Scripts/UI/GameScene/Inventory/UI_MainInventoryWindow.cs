using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;
using Unity.VisualScripting;
using System.Linq;


[UIElement(false, "UI/UI_MainInventoryWindow", 1)]
public class UI_MainInventoryWindow : UI_InventoryWindowBase
{
    private MainInventoryData mainInventoryData;
    [SerializeField] public UI_ItemSlot weaponSlot;       // 装备槽

    public void Update() {
        #region 测试逻辑
        if (Input.GetKeyDown(KeyCode.Alpha0)) AddItemAndPlayAudio(0);
        if (Input.GetKeyDown(KeyCode.Alpha1)) AddItemAndPlayAudio(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) AddItemAndPlayAudio(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) AddItemAndPlayAudio(3);
        #endregion
    }

    public override void Init() {
        base.Init();
        EventManager.AddEventListener(EventName.PlayerWeaponAttackSucceed, OnPlayerWeaponAttackSucceed);
    }

    // 初始化数据
    public void InitData() {
        // 初始化快捷栏数据
        inventoryData = ArchiveManager.Instance.mainInventoryData;
        mainInventoryData = inventoryData as MainInventoryData;
        // 初始化格子: 每个槽对应的位置和父窗口信息
        InitSlotData();
        // 将武器更新到角色模型上
        Player_Controller.Instance.ChangeWeapon(mainInventoryData.weaponSlotItemData);
    }
    
    // 初始化物品槽数据: 已重写该方法
    protected override void InitSlotData() {
        weaponSlot.Init(mainInventoryData.itemDatas.Length, this, UseItem);
        weaponSlot.InitData(mainInventoryData.weaponSlotItemData);
        UI_ItemSlot.weaponSlot = weaponSlot;
        for (int i = 0; i < mainInventoryData.itemDatas.Length; i++) {
            // 初始化每个槽中物品对应的各种UI信息
            slots[i].Init(i, this, UseItem);
            slots[i].InitData(mainInventoryData.itemDatas[i]);
        }
    }
    
    // 设置格子中的内容
    public override void SetItem(int index, ItemData itemData) {
        // 判断是否为为武器还是普通格子
        if (index == mainInventoryData.itemDatas.Length) {
            mainInventoryData.SetWeaponItem(itemData);
            weaponSlot.InitData(itemData);
            // 将武器数据同步给玩家
            Player_Controller.Instance.ChangeWeapon(itemData);
        } else {
            base.SetItem(index, itemData);
        }
    }

    // 从物品快捷栏中移除index处的格子
    protected override void RemoveItem(int index) {
        // 判断是否为移除武器还是移除普通格子
        if (index == mainInventoryData.itemDatas.Length) {
            mainInventoryData.RemoveWeaponItem();
            weaponSlot.InitData(null);
        } else {
            base.RemoveItem(index);
        }
    }

    // 丢掉一件物品
    public override void DiscardItem(int index) {
        // 如果是武器直接丢弃, 注意需要修改模型
        if (index == slots.Count || slots[index].itemData.config.itemType == ItemType.Weapon) {
            RemoveItem(index);
            Player_Controller.Instance.ChangeWeapon(null);
            return;
        }
        base.DiscardItem(index);
    }

    // 使用物品
    private AudioType UseItem(int index) {
        // TODO: 玩家的状态不一定能使用物品
        if (Player_Controller.Instance.canUseItem == false) {
            return AudioType.PlayerConnot;
        }
        // 1. 武器物品栏
        if (index == slots.Count) {
            int emptySlotIndex = GetEmptySlot();
            if (emptySlotIndex >= 0) {
                UI_ItemSlot.SwapSlotItem(weaponSlot, slots[emptySlotIndex]);
                return AudioType.TakeDownWeapon;
            } else {
                return AudioType.Fail;
            }
        }
        // 2. 普通物品栏
        ItemData itemData = slots[index].itemData;
        switch (itemData.config.itemType) {
            case ItemType.Weapon:
                // 使用武器
                UI_ItemSlot.SwapSlotItem(slots[index], weaponSlot);
                return AudioType.TakeUpWeapon;
            case ItemType.Consumable:
                // 使用消耗品
                ItemConsumableInfo info = itemData.config.itemTypeInfo as ItemConsumableInfo;
                if (info.recoverHP != 0) {
                    Player_Controller.Instance.RecoverHP(info.recoverHP);
                }
                if (info.recoverHungry != 0) {
                    Player_Controller.Instance.RecoverHungry(info.recoverHungry);
                }
                ItemConsumableData data = itemData.itemTypeData as ItemConsumableData;
                data.count -= 1;
                if (data.count == 0) {
                    RemoveItem(index);
                } else {
                    slots[index].UpdateNumTextView();
                }
                return AudioType.ConsumableOK;
            case ItemType.Meterial:
                // 使用材料
                return AudioType.Fail;
            default:
                return AudioType.Fail;
        }
    }

    // 使用合成/建造功能时更新物品数量
    public void UpdateItemsForBuild(BuildConfig buildConfig) {
        for (int i = 0; i < buildConfig.buildConfigConditions.Count; i++) {
            UpdateItemForBuild(buildConfig.buildConfigConditions[i]);
        }
    }

    // 使用合成/建造功能时更新物品数量
    private void UpdateItemForBuild(BuildConfigCondition buildConfigCondition) {
        int configId = buildConfigCondition.itemId;
        int count = buildConfigCondition.count;
        for (int i = 0; i < mainInventoryData.itemDatas.Length; i++) {
            ItemData itemData = mainInventoryData.itemDatas[i];
            if (itemData != null && itemData.configId == configId) {
                if (itemData.itemTypeData is PileItemTypeDataBase) {
                    PileItemTypeDataBase pileItemTypeData = itemData.itemTypeData as PileItemTypeDataBase;
                    int quantity = pileItemTypeData.count - count;
                    if (quantity > 0) {
                        pileItemTypeData.count -= count;
                        slots[i].UpdateNumTextView();
                        return;
                    } else {
                        count -= pileItemTypeData.count;
                        RemoveItem(i);
                        if (count == 0) {
                            return;
                        } 
                    }
                } else {
                    count -= 1;
                    RemoveItem(i);
                    if (count == 0) {
                        return;
                    } 
                }
            }
        }
        UnityEngine.Debug.LogError("建造条件背包不满足");
    }

    // 玩家使用武器成功攻击
    private void OnPlayerWeaponAttackSucceed() {
        if (mainInventoryData.weaponSlotItemData == null) {
            return;
        }
        // 获取当前快捷栏中武器数据
        ItemWeaponData itemWeaponData = mainInventoryData.weaponSlotItemData.itemTypeData as ItemWeaponData;
        ItemWeaponInfo itemWeaponInfo = mainInventoryData.weaponSlotItemData.config.itemTypeInfo as ItemWeaponInfo;
        itemWeaponData.durability -= itemWeaponInfo.attackDurabilityCost;
        // 检查当前武器是否损坏
        if (itemWeaponData.durability <= 0) {
            // 移除武器数据和UI
            mainInventoryData.RemoveWeaponItem();
            weaponSlot.InitData(null);
            // 玩家模型更换武器
            Player_Controller.Instance.ChangeWeapon(null);
        } else {
            // 更新UI
            weaponSlot.UpdateNumTextView();
        }
    }
}
