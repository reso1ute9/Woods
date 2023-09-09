using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : MapObjectBase
{
    [SerializeField] protected new Collider collider;
    public Collider Collider { get => collider; }

    // 初始化预览方法
    public virtual void InitOnPreview() {
        // 预览时关闭碰撞体, 如果开启的话可能会存在问题
        collider.enabled = false;
    }   
}
