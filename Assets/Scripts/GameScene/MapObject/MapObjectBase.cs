using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;


// 地图物品类型
public enum mapObjectType {
    Tree, 
    Stone, 
    SamllStone,
    Bush,
}

// 地图对象基类
public abstract class MapObjectBase : MonoBehaviour {
    [SerializeField] mapObjectType objectType;
    public mapObjectType ObjectType { get => objectType; } 
    protected MapChunkController mapChunk;      // 当前所在的地图块控制器
    ulong mapObjectId;                          // 当前地图物品id

    public virtual void Init(MapChunkController mapChunk, ulong mapObjectId) {
        this.mapChunk = mapChunk;
        this.mapObjectId = mapObjectId;
    }

    public virtual void RemoveOnMap() {
        // 通知地图块控制器移除当前地图对象
        mapChunk.RemoveMapObject(mapObjectId);
        // 将当前树木对象放到对象池
        this.JKGameObjectPushPool();
        // TODO: 掉落物品
    }
}
