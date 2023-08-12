using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 地图物品类型
public enum mapObjectType {
    Tree, 
    Stone, 
    SamllStone,
}

// 地图对象基类
public abstract class MapObjectBase : MonoBehaviour {
    [SerializeField] mapObjectType objectType;
    public mapObjectType ObjectType { get => objectType; } 
}
