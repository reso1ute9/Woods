using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "地图物体配置", menuName = "Config/地图物体配置")]
public class MapObjectConfig : ConfigBase
{
    [LabelText("空的(不生成物品)")]
    public bool isEmpty = false;
    [LabelText("地图顶点类型")]
    public MapVertexType mapVertexType;
    [LabelText("预制体")]
    public GameObject prefab;
    [LabelText("生成概率(百分比类型)")]
    public int probability;
}
