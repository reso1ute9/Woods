using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;

// 地图物品生成配置
[CreateAssetMenu(fileName = "地图配置", menuName = "Config/地图配置")]
public class MapConfig : ConfigBase {
    [LabelText("地图块包含网格数")]
    public int mapChunkSize;                // 地图块大小
    [LabelText("地图块网格大小")]
    public float cellSize;                  // 网格大小
    [LabelText("噪声图采样间隔大小")]
    public float noiseLacunarity;           // 噪声图采样间隔大小
    [LabelText("森林贴图")]
    public Texture2D forestTexture;         // 森林贴图    
    [LabelText("沼泽贴图")]
    public Texture2D[] marshTextures;       // 沼泽贴图
    [LabelText("地图材质")]
    public Material mapMaterial;            // 地图材质
    [LabelText("玩家可视距离")]
    public int viewDistance;                // 玩家可视距离, 单位为ChunkSize
    [LabelText("地图块刷新概率")]        
    public int mapChunkRefreshProbability;  // 每天早晨地图块刷新地图物品的概率
}