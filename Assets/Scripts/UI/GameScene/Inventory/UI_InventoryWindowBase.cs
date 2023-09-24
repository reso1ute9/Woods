using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;
using Unity.VisualScripting;
using System.Linq;

[UIElement(false,"UI/UI_InventoryWindow",1)]
public abstract class UI_InventoryWindowBase : UI_WindowBase
{
    protected InventoryData inventoryData;
    [SerializeField] protected List<UI_ItemSlot> slots;             // 物品槽
    
    public Sprite[] bgSprite;                                       // 框图
    
    public override void Init() {
        base.Init();
    }
    
    // 初始化物品快捷栏所有格子UI界面
    protected virtual void InitSlotData() {
        for (int i = 0; i < inventoryData.itemDatas.Length; i++) {
            // 初始化每个槽中物品对应的各种UI信息
            slots[i].Init(i, this, null);
            slots[i].InitData(inventoryData.itemDatas[i]);
        }
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

    // 设置格子中的内容
    public virtual void SetItem(int index, ItemData itemData) {
        inventoryData.SetItem(index, itemData);
        slots[index].InitData(itemData);
    }

    // 从物品快捷栏中移除index处的格子
    protected virtual void RemoveItem(int index) {
        inventoryData.RemoveItem(index);
        slots[index].InitData(null);
    }

    // 丢掉一件物品
    public virtual void DiscardItem(int index) {
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

    // 得到一个空格子, return -1代表没有空格子
    protected int GetEmptySlot() {
        for (int i = 0; i < slots.Count; i++) {
            if (slots[i].itemData == null) {
                return i;
            }
        }
        return -1;
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

    // 检测并堆放物体到格子上
    private bool CheckAndPileItemForSlot(int configId) {
        for (int i = 0; i < slots.Count; i++) {
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

    // 获取某个物品的数量
    public int GetItemCount(int configId) {
        int count = 0;
        for (int i = 0; i < inventoryData.itemDatas.Length; i++) {
            if (inventoryData.itemDatas[i] != null && inventoryData.itemDatas[i].configId == configId) {
                if (inventoryData.itemDatas[i].itemTypeData is PileItemTypeDataBase) {
                    count += ((PileItemTypeDataBase)inventoryData.itemDatas[i].itemTypeData).count;
                } else {
                    count += 1;
                }
            }
        }
        return count;
    }

    // 注册监听事件
    protected override void RegisterEventListener() {
        base.RegisterEventListener();
    }

    // 取消监听事件
    protected override void CancelEventListener() {
        base.CancelEventListener();
    }
}
