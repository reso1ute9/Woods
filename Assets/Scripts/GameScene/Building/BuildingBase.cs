using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : MapObjectBase, IBuilding 
{
    [SerializeField] protected new Collider collider;
    private List<Material> materialList = null;

    #region 预览模式
    GameObject IBuilding.gameObject => gameObject;
    Collider IBuilding.Collider => collider;
    List<Material> IBuilding.materialList { 
        get => materialList;
        set => materialList = value; 
    }
    #endregion

    #region 运行模式
    // 当建筑物被选中时
    public virtual void OnSelect() {}
    #endregion
}