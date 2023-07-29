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

    public ItemConfig config {
        get => ConfigManager.Instance.GetConfig<ItemConfig>(ConfigName.Item, configId);
    }
    
    public static ItemData CreateItemData(int configId) {
        ItemData itemData = new ItemData();
        itemData.configId = configId;
        // 根据物品实际类型来创建符合类型的动态数据
        switch (itemData.config.itemType)
        {
            case ItemType.Weapon:
                itemData.itemTypeData = new ItemWeaponData() { durability = 100 };
                break;
            case ItemType.Consumable:
                itemData.itemTypeData = new ItemConsumableData() { count = 1 };
                break;
            case ItemType.Meterial:
                itemData.itemTypeData = new ItemMaterialData() { count = 1 };
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
[Serializable]
public class ItemWeaponData : IItemTypeData {
    public int durability;          // 武器耐久度
}

// 消耗品接口类
[Serializable]
public class ItemConsumableData : IItemTypeData 
{
    public int count;
}

// 材料接口类
[Serializable]
public class ItemMaterialData : IItemTypeData 
{
    public int count;
}