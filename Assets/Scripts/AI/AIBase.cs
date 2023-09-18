using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBase : MonoBehaviour
{
    protected MapChunkController mapChunk;                      // 当前所在的地图块控制器
    ulong mapObjectId;                                          // 当前地图物品id

    public virtual void Init(MapChunkController mapChunk, ulong mapObjectId) {
        this.mapChunk = mapChunk;
        this.mapObjectId = mapObjectId;
    }

    public virtual void RemoveOnMap() {
        // 通知地图块控制器移除当前AI对象
    }
}
