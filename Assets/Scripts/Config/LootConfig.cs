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

    // 根据掉落配置生成地图对象物品
    public void GenerateMapObject(MapChunkController mapChunk, Vector3 position) {
        // 根据概率决定是否实例化
        for (int i = 0; i < Configs.Count; i++) {
            int randValue = Random.Range(1, 101);
            if (randValue <= Configs[i].Probability) {
                // 生成掉落物品
                // 1. 掉落物品在父物体的上方一些
                float randomX = 1.0f * Random.Range(-10, 10) / 20;
                float randomZ = 1.0f * Random.Range(-10, 10) / 20;
                Vector3 pos = position + new Vector3(randomX, 1, randomZ);
                MapManager.Instance.GenerateMapObject(
                    mapChunk, Configs[i].LootObjectConfigId, pos, false
                );
            }
        }
    }
}


public class LootConfigModel 
{
    [LabelText("掉落物体Id")] public int LootObjectConfigId; 
    [LabelText("掉落概率%")] public int Probability; 
}