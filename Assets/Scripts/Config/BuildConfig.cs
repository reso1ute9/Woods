using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using JKFrame;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "建造合成配置", menuName = "Config/建造与合成配置")]
public class BuildConfig : ConfigBase
{
    [LabelText("合成类型")] 
    public BuildType buildType;

    [LabelText("合成条件")]
    public List<BuildConfigCondition> buildConfigConditions = new List<BuildConfigCondition>();
    [LabelText("合成产物")]
    public int targetId;

    // 检查是否满足当前监造/合成配置
    public bool CheckBuildConfigCondition() {
        for (int j = 0; j < buildConfigConditions.Count; j++) {
            int currentCount = InventoryManager.Instance.GetItemCount(buildConfigConditions[j].itemId);
            if (currentCount < buildConfigConditions[j].count) {
                return false;
            }
        }
        return true;
    }
}


public class BuildConfigCondition 
{
    [LabelText("物品Id"), HorizontalGroup]
    public int itemId;
    [LabelText("物品数量"), HorizontalGroup]
    public int count;
}