using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;


[UIElement(true, "UI/UI_InventoryWindow", 1)]
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
        // 将武器更新到角色模型上
        Player_Controller.Instance.ChangeWeapon(inventoryData.weaponSlotItemData);
    }

    public override void OnShow() {
        base.OnShow();
        // 根据存档复原物品快捷栏格子
        InitData(inventoryData);
    }

    public void Update() {
        #region 测试逻辑
        if (Input.GetKeyDown(KeyCode.Alpha0)) AddItemAndPlayAudio(0);
        if (Input.GetKeyDown(KeyCode.Alpha1)) AddItemAndPlayAudio(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) AddItemAndPlayAudio(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) AddItemAndPlayAudio(3);
        #endregion
    }
    
    // 初始化物品快捷栏所有格子UI界面
    private void InitData(InventoryData data) {
        for (int i = 0; i < data.itemDatas.Length; i++) {
            // 初始化每个槽中物品对应的各种UI信息
            slots[i].InitData(data.itemDatas[i]);
        }
        weaponSlot.InitData(data.weaponSlotItemData);
    }

    // 添加物品
    public bool AddItemAndPlayAudio(int configId) {
        bool res = AddItem(configId);
        if (res) {
            ProjectTool.PlayerAudio(AudioType.Bag);
        } else {
            ProjectTool.PlayerAudio(AudioType.Fail);
        }
        return res;
    }

    // 使用物品
    public AudioType UseItem(int index) {
        // TODO: 玩家的状态不一定能使用物品
        if (Player_Controller.Instance.canUseItem == false) {
            return AudioType.PlayerConnot;
        }
        // 1. 武器物品栏
        if (index == slots.Length) {
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

    // 逻辑层面添加物品
    public bool AddItem(int configId) {
        ItemConfig itemConfig = ConfigManager.Instance.GetConfig<ItemConfig>(ConfigName.Item, configId);
        switch (itemConfig.itemType) {
            case ItemType.Weapon:
                // 武器只能放空位
                return CheckAndAddItemForEmptySlot(configId);
            case ItemType.Consumable:
                // 消耗品优先堆叠, 当消耗品数量达到堆叠上限时再选择空格子进行放置
                if (CheckAndPileItemForSlot(configId)) {
                    return true;
                } else {
                    return CheckAndAddItemForEmptySlot(configId);
                }
            case ItemType.Meterial:
                // 材料优先堆叠, 当消耗品数量达到堆叠上限时再选择空格子进行放置
                if (CheckAndPileItemForSlot(configId)) {
                    return true;
                } else {
                    return CheckAndAddItemForEmptySlot(configId);
                }
            default:
                break;
        }
        return false;
    }

    // 检查和添加物品到空格子
    private bool CheckAndAddItemForEmptySlot(int configId) {
        int index = GetEmptySlot();
        if (index >= 0) {
            SetItem(index, ItemData.CreateItemData(configId));
            return true;
        } else {
            return false;
        }
    }

    // 得到一个空格子, return -1代表没有空格子
    private int GetEmptySlot() {
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].itemData == null) {
                return i;
            }
        }
        return -1;
    }

    // 检测并堆放物体到格子上
    private bool CheckAndPileItemForSlot(int configId) {
        for (int i = 0; i < slots.Length; i++) {
            // 当前格子不为空 & 物品类型一致
            if (slots[i].itemData != null && 
                slots[i].itemData.configId == configId
            ) {
                // 是否到达堆叠上限
                PileItemTypeDataBase data = slots[i].itemData.itemTypeData as PileItemTypeDataBase;
                PileItemTypeInfoBase info = slots[i].itemData.config.itemTypeInfo as PileItemTypeInfoBase;
                if (data.count < info.maxCount) {
                    data.count += 1;
                    // 刷新物品UI数值
                    slots[i].UpdateNumTextView();
                    return true;
                }
            }
        }
        return false;
    }

    // 从物品快捷栏中移除index处的格子
    private void RemoveItem(int index) {
        // 判断是否为移除武器还是移除普通格子
        if (index == inventoryData.itemDatas.Length) {
            inventoryData.RemoveWeaponItem();
            weaponSlot.InitData(null);
        } else {
            inventoryData.RemoveItem(index);
            slots[index].InitData(null);
        }
    }

    // 丢掉一件物品
    public void DiscardItem(int index) {
        // 如果是武器直接丢弃
        if (index == slots.Length || slots[index].itemData.config.itemType == ItemType.Weapon) {
            RemoveItem(index);
            return;
        }
        // 根据类型去判断, 对于可堆积的物品每次减少一个
        ItemData itemData = slots[index].itemData;
        // 当进入到这里时一定时消耗品
        PileItemTypeDataBase typeData = itemData.itemTypeData as PileItemTypeDataBase;
        typeData.count -= 1;
        if (typeData.count == 0) {
            RemoveItem(index);
        } else {
            slots[index].UpdateNumTextView();
        }
    }

    public void SetItem(int index, ItemData itemData) {
        // 判断是否为为武器还是普通格子
        if (index == inventoryData.itemDatas.Length) {
            inventoryData.SetWeaponItem(itemData);
            weaponSlot.InitData(itemData);
            // 将武器数据同步给玩家
            Player_Controller.Instance.ChangeWeapon(itemData);
        } else {
            inventoryData.SetItem(index, itemData);
            slots[index].InitData(itemData);
        }
    }

    // 玩家使用武器成功攻击
    private void OnPlayerWeaponAttackSucceed() {
        if (inventoryData.weaponSlotItemData == null) {
            return;
        }
        // 获取当前快捷栏中武器数据
        ItemWeaponData itemWeaponData = inventoryData.weaponSlotItemData.itemTypeData as ItemWeaponData;
        ItemWeaponInfo itemWeaponInfo = inventoryData.weaponSlotItemData.config.itemTypeInfo as ItemWeaponInfo;
        // itemWeaponData.durability = Mathf.Clamp(itemWeaponData.durability - itemWeaponInfo.attackDurabilityCost, 0, 100);
        UnityEngine.Debug.Log("itemWeaponData.durability:" + itemWeaponData.durability);
        UnityEngine.Debug.Log("itemWeaponInfo.attackDurabilityCost:" + itemWeaponInfo.attackDurabilityCost);
        itemWeaponData.durability -= itemWeaponInfo.attackDurabilityCost;
        // 检查当前武器是否损坏
        if (itemWeaponData.durability <= 0) {
            // 移除武器数据和UI
            RemoveItem(inventoryData.itemDatas.Length);
            // 玩家模型更换武器
            Player_Controller.Instance.ChangeWeapon(null);
        } else {
            // 更新UI
            weaponSlot.UpdateNumTextView();
        }
    }

    // 注册监听事件
    protected override void RegisterEventListener() {
        base.RegisterEventListener();
        // 窗口隐藏时该事件也应该持续监听
        EventManager.AddEventListener(EventName.PlayerWeaponAttackSucceed, OnPlayerWeaponAttackSucceed);
    }

    // 取消监听事件
    protected override void CancelEventListener() {
        base.CancelEventListener();
        // EventManager.RemoveEventListener(EventName.PlayerWeaponAttackSucceed, OnPlayerWeaponAttackSucceed);
    }
}
