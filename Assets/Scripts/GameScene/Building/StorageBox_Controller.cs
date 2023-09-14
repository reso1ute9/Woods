using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBox_Controller : BuildingBase
{
    [SerializeField] Vector2Int UIWindowGridSize;       // 储物箱格子 w * h
    private StorageBoxData storageBoxData;
    

    public override void Init(MapChunkController mapChunk, ulong mapObjectId, bool isFromBuild) {
        base.Init(mapChunk, mapObjectId, isFromBuild);
        // 查找是否有存档
        if (isFromBuild == true) {
            storageBoxData = new StorageBoxData(UIWindowGridSize.x * UIWindowGridSize.y);
            ArchiveManager.Instance.AddMapObjectTypeData(mapObjectId, storageBoxData);
        } else {
            storageBoxData = ArchiveManager.Instance.GetMapObjectTypeData(mapObjectId) as StorageBoxData;
        }
    }
    
    // 建筑物选中逻辑
    public override void OnSelect() {
        // 打开储物箱UI窗口
        InventoryManager.Instance.OpenStorageBoxWindow(this, storageBoxData.InventoryData, UIWindowGridSize);
    }

    private void OnDisable() {
        storageBoxData = null;
    }
}
