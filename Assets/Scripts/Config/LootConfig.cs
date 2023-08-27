using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;

// 掉落配置
[CreateAssetMenu(fileName = "掉落配置", menuName = "Config/掉落配置")]
public class LootConfig : ConfigBase
{
    [LabelText("掉落配置列表")] public List<LootConfigModel> Configs;
}


public class LootConfigModel 
{
    [LabelText("掉落物体Id")] public int LootObjectConfigId; 
    [LabelText("掉落概率%")] public int Probability; 
}