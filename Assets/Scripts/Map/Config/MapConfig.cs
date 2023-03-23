using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;

// 地图物品生成配置
[CreateAssetMenu(fileName = "地图配置", menuName = "Config/地图配置")]
public class MapConfig : ConfigBase
{
    public Dictionary<MapVertexType, List<MapObjectSpawnConfigModel>> mapObjectConfig = new Dictionary<MapVertexType, List<MapObjectSpawnConfigModel>>();
}


public class MapObjectSpawnConfigModel {
    [LabelText("空的(不生成物品)")]
    public bool isEmpty = false;
    [LabelText("预制体")]
    public GameObject prefab;
    [LabelText("生成概率(百分比类型)")]
    public int probability;
}