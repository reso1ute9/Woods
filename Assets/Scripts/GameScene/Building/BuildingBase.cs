using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingBase : MapObjectBase, IBuilding 
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
    
    public virtual void OnPreView() {}
    #endregion

    #region 运行模式
    [SerializeField] private List<ulong> unlockScienceOnBuild;     // 该建筑物能解锁的科技
    
    // 当建筑物被选中时
    public virtual void OnSelect() {}

    public override void Init(MapChunkController mapChunk, ulong mapObjectId, bool isFromBuild) {
        base.Init(mapChunk, mapObjectId, isFromBuild);
        if (isFromBuild == true) {
            for (int i = 0; i < unlockScienceOnBuild.Count; i++) {
                ScienceManager.Instance.AddScience(unlockScienceOnBuild[i]);
            }
        }
    }
    #endregion
}   