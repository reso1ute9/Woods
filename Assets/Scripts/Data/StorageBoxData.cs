using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StorageBoxData : IMapObjectTypeData
{
    private InventoryData inventoryData;    // 储物箱数据
    public InventoryData InventoryData { get => inventoryData; }
    
    public StorageBoxData(int itemCount) {
        inventoryData = new InventoryData(itemCount);
    }
}
