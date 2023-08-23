using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using System.ComponentModel;


// 地图物品类型
public enum mapObjectType {
    Tree, 
    Stone, 
    SamllStone,
    Bush,
    MushRoom, 
}

// 地图对象基类
public abstract class MapObjectBase : MonoBehaviour {
    [SerializeField] mapObjectType objectType;                  // 地图对象类型
    public mapObjectType ObjectType { get => objectType; } 
    [SerializeField] protected float touchDistance;             // 交互距离
    public float TouchDistance { get => touchDistance; }
    [SerializeField] protected bool canPickUp;                  // 能否采摘
    public bool CanPickUp { get => canPickUp; }
    [SerializeField] protected int pickUpItemConfigId = -1;     // 捡取对象id, -1意味着无法获取
    public int PickUpItemConfigId { get => pickUpItemConfigId; }

    protected MapChunkController mapChunk;                      // 当前所在的地图块控制器
    ulong mapObjectId;                                          // 当前地图物品id

    public virtual void Init(MapChunkController mapChunk, ulong mapObjectId) {
        this.mapChunk = mapChunk;
        this.mapObjectId = mapObjectId;
    }

    public virtual void RemoveOnMap() {
        // 通知地图块控制器移除当前地图对象
        mapChunk.RemoveMapObject(mapObjectId);
        // 将当地图对象放到对象池
        this.JKGameObjectPushPool();
        // TODO: 掉落物品
    }

    // 当被捡起时需要消除地图上UI、数据并返回对应id
    public virtual int OnPickUp() {
        RemoveOnMap();
        return pickUpItemConfigId;
    }
}
