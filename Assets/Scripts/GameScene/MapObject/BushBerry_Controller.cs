using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 浆果灌木丛控制器
public class BushBerry_Controller : Bush_Controller
{   
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material[] materials;          // 0: 有浆果的材质; 1. 没有浆果的材质


    // 重写摘取方法, 当摘取浆果后灌木丛变为可收集状态
    public override int OnPickUp() {
        // 修改浆果灌木丛外观
        meshRenderer.sharedMaterial = materials[1];
        // 设置为不可采摘
        canPickUp = false;    
        return pickUpItemConfigId;
    }
}