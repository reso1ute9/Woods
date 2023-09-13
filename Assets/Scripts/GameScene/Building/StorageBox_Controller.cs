using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBox_Controller : BuildingBase
{
    public override void Init(MapChunkController mapChunk, ulong mapObjectId, bool isFromBuild) {
        base.Init(mapChunk, mapObjectId, isFromBuild);
        // TODO: 查找是否有存档
    }
}
