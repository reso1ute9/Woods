using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using JKFrame;

// 地图物品生成配置
[CreateAssetMenu(fileName = "生物配置", menuName = "Config/生物配置")]
public class AIConfig : ConfigBase
{
    [LabelText("空的(不生成物品)")]
    public bool isEmpty = false;
    [LabelText("地图顶点类型")]
    public MapVertexType mapVertexType;
    [LabelText("预制体")]
    public GameObject prefab;
    [LabelText("生成概率(百分比类型)")]
    public int probability;
    [LabelText("腐烂天数")]
    public int destoryDay = -1;                 // -1代表无效
}