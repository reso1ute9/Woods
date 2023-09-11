using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : MapObjectBase, IBuilding 
{
    [SerializeField] protected new Collider collider;
    public List<Material> materialList = null;

    #region 预览模式
    GameObject IBuilding.gameObject => gameObject;
    Collider IBuilding.Collider => collider;
    List<Material> IBuilding.materialList { 
        get => materialList;
        set => materialList = value; 
    }
    #endregion
}