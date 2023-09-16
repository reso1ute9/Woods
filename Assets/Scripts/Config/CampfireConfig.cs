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
}
