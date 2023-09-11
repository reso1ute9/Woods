using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 浆果灌木丛控制器
public class BushBerry_Controller : Bush_Controller, IBuilding
{   
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] new Collider collider;
    [SerializeField] List<Material> materials;              // 0: 有浆果的材质; 1. 没有浆果的材质
    [SerializeField] int bushBerryGrowDayNum;               // 浆果成长天数

    private BushBerryTypeData bushBerryTypeData;            // 浆果动态数据

    // 实现建筑物接口
    GameObject IBuilding.gameObject => gameObject;
    Collider IBuilding.Collider => collider;
    List<Material> IBuilding.materialList { 
        get => materials; 
        set => materials = value; 
    }

    public override void Init(MapChunkController mapChunk, ulong mapObjectId) {
        base.Init(mapChunk, mapObjectId);
        // 获取类型数据存档
        if (ArchiveManager.Instance.TryGetMapObjectTypeData(mapObjectId, out IMapObjectTypeData tempData)) {
            bushBerryTypeData = (BushBerryTypeData)tempData;
        } else {
            bushBerryTypeData = new BushBerryTypeData();
            ArchiveManager.Instance.AddMapObjectTypeData(mapObjectId, bushBerryTypeData);
        }
        // 检测和设置当前浆果状态
        CheckAndSetState();
    }

    // 重写摘取方法, 当摘取浆果后灌木丛变为可收集状态
    public override int OnPickUp() {
        // 设置为不可采摘
        canPickUp = false;
        // 修改浆果灌木丛外观
        meshRenderer.sharedMaterial = materials[1];
        // 记录采摘天数
        bushBerryTypeData.lastPickUpDayNum = TimeManager.Instance.currentDayNum;
        return pickUpItemConfigId;
    }

    // 检查并修改浆果状态
    private void CheckAndSetState() {
        // 浆果有没有采摘过, -1默认为未采摘过
        if (bushBerryTypeData.lastPickUpDayNum == -1) {
            canPickUp = true;
            meshRenderer.sharedMaterial = materials[0];
        } else {
            if (TimeManager.Instance.currentDayNum - bushBerryTypeData.lastPickUpDayNum >= bushBerryGrowDayNum) {
                canPickUp = true;
                meshRenderer.sharedMaterial = materials[0];
            } else {
                canPickUp = false;
                meshRenderer.sharedMaterial = materials[1];
            }
        }
    }
}