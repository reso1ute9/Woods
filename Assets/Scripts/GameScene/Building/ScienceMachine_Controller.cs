using System.Collections;
using System.Collections.Generic;
using JKFrame;
using UnityEngine;

// 科学机器控制器
public class ScienceMachine_Controller : BuildingBase
{
    public override void Init(MapChunkController mapChunk, ulong mapObjectId, bool isFromBuild) {
        base.Init(mapChunk, mapObjectId, isFromBuild);
        if (isFromBuild == true) {
            // 更新科技信息
            // ulong Id = ConfigManager.Instance.GetConfig<MapObjectConfig>
            ScienceManager.Instance.AddScience(28);
        }
    }
}
