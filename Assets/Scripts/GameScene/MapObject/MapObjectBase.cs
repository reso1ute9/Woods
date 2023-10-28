using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine.AI;


// 地图物品类型
public enum mapObjectType 
{
    Tree, 
    Stone, 
    Bush,
    Material,               // 材料
    Consumable,             // 消耗品
    Weapon,                 // 武器
    Building,               // 建筑
}

// 地图对象基类
public abstract class MapObjectBase : MonoBehaviour 
{
    [SerializeField] mapObjectType objectType;                  // 地图对象类型
    public mapObjectType ObjectType { get => objectType; } 
    protected MapChunkController mapChunk;                      // 当前所在的地图块控制器
    ulong mapObjectId;                                          // 当前地图物品id
    
    [SerializeField] protected float touchDistance;             // 交互距离
    public float TouchDistance { get => touchDistance; }
    [SerializeField] protected bool canPickUp;                  // 能否采摘
    public bool CanPickUp { get => canPickUp; }
    [SerializeField] protected int pickUpItemConfigId = -1;     // 捡取对象id, -1意味着无法获取
    public int PickUpItemConfigId { get => pickUpItemConfigId; }

    

    public virtual void Init(MapChunkController mapChunk, ulong mapObjectId, bool isFromBuild) {
        this.mapChunk = mapChunk;
        this.mapObjectId = mapObjectId;
    }

    public virtual void RemoveOnMap() {
        // 通知地图块控制器移除当前地图对象
        mapChunk.RemoveMapObject(mapObjectId);
    }

    // 当被捡起时需要消除地图上UI、数据并返回对应id
    public virtual int OnPickUp() {
        RemoveOnMap();
        return pickUpItemConfigId;
    }

    // 添加导航
    [Sirenix.OdinInspector.Button]
    public void AddNavMeshObstacle() {
        NavMeshObstacle navMeshObstacle = transform.AddComponent<NavMeshObstacle>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        if (boxCollider != null) {
            navMeshObstacle.shape = NavMeshObstacleShape.Box;
            navMeshObstacle.center = boxCollider.center;
            navMeshObstacle.size = boxCollider.size;
            navMeshObstacle.carving = true;
        } else if (capsuleCollider != null) {
            navMeshObstacle.shape = NavMeshObstacleShape.Capsule;
            navMeshObstacle.center = capsuleCollider.center;
            navMeshObstacle.height = capsuleCollider.height;
            navMeshObstacle.radius = capsuleCollider.radius;
            navMeshObstacle.carving = true;
        } 
        
    }
}
