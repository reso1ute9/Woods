using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "篝火配置", menuName = "Config/篝火配置")]
public class CampfireConfig : ConfigBase
{
    [LabelText("默认燃料数值")]
    public float defaultFuelValue;
    [LabelText("最大燃料数值")]
    public float maxFuelValue;
    [LabelText("燃烧速度/每秒消耗量")]
    public float buringSpeed;
    [LabelText("最大灯光强度")]
    public float maxLightIntensity;
    [LabelText("最大灯光范围")]
    public float maxLightRange;
    [LabelText("燃料和物品对照关系")]
    public Dictionary<int, float> itemFuelDict;
    [LabelText("可以烧烤的物品对照关系")]
    public Dictionary<int, int> bakedItemDict; 

    public bool TryGetFuelValueByItemId(int itemId, out float fuelValue) {
        return itemFuelDict.TryGetValue(itemId, out fuelValue);
    }

    public bool TryGetBakedItemByItemId(int itemId, out int bakedItemId) {
        return bakedItemDict.TryGetValue(itemId, out bakedItemId);
    }
}
