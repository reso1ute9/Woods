using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using System;


// 玩家物品的动态数据
[Serializable]
public class ItemData
{
    public int configId;
    public IItemTypeData itemTypeData;

    private ItemConfig config {
        get => ConfigManager.Instance.GetConfig<ItemConfig>(ConfigName.Item);
    }
    
    public static ItemData CreateItemData(int configId) {
        ItemData itemData = new ItemData();
        itemData.configId = configId;
        // 根据物品实际类型来创建符合类型的动态数据
        switch (itemData.config.itemType)
        {
            case ItemType.Weapon:
                itemData.itemTypeData = new ItemWeaponData();
                break;
            case ItemType.Consumable:
                itemData.itemTypeData = new ItemConsumableData() { count = 1 };
                break;
            case ItemType.Meterial:
                itemData.itemTypeData = new ItemMeterialData() { count = 1 };
                break;
            default:
                break;
        }
        return itemData;
    }
}


// 物品数据接口类
public interface IItemTypeData {}

// 武器接口类
public class ItemWeaponData : IItemTypeData {}

// 消耗品接口类
public class ItemConsumableData : IItemTypeData 
{
    public int count = 1;
}

// 材料接口类
public class ItemMeterialData : IItemTypeData 
{
    public int count = 1;
}