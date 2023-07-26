using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;


public class ArchiveManager : Singleton<ArchiveManager>
{
    public PlayerTransformData playerTransformData { get; private set; }
    public MapInitData mapInitData { get; private set; }
    public MapData mapData { get; private set; }
    public bool haveArchive { get; private set; }           // 判断当前情况是否有存档
    
    public ArchiveManager() {
        LoadSaveData();
    }

    // 加载存档系统里的数据
    public void LoadSaveData() {
        // 单存档情况下默认获取
        SaveItem saveItem = SaveManager.GetSaveItem(0);
        haveArchive = (saveItem != null);
    }

    // 保存玩家位置数据存档到磁盘上
    public void SavePlayerTransformData() {
        SaveManager.SaveObject(playerTransformData);
    }

    // 保存地图数据
    public void SaveMapData() {
        SaveManager.SaveObject(mapData);
    }

    // 添加单个地图块数据
    public void AddAndSaveMapChunkData(Vector2Int chunkIndex, MapChunkData mapChunkData) {
        // 保存索引
        Serialization_Vector2 index = chunkIndex.ConverToSVector2();
        mapData.MapChunkIndexList.Add(index);
        SaveMapData();
        // 保存地图块
        SaveManager.SaveObject(mapChunkData, "Map_" + index.ToString());  
    }

    // 获取单个地图块数据
    public MapChunkData GetMapChunkData(Serialization_Vector2 chunkIndex) {
        return SaveManager.LoadObject<MapChunkData>("Map_" + chunkIndex.ToString());
    }

    // 创建新的空存档
    public void CreateNewArchive(int mapSize, int mapSeed, int spawnSeed, float marshLimit) {
        // 清空当前存档
        SaveManager.Clear();
        // 创建存档信息
        SaveManager.CreateSaveItem();
        haveArchive = true;
        // 1. 存储地图初始化数据
        mapInitData = new MapInitData() {
            mapSize = mapSize,
            mapSeed = mapSeed,
            spawnSeed = spawnSeed,
            marshLimit = marshLimit
        };
        SaveManager.SaveObject(mapInitData);
        // 2. 存储玩家初始化数据
        MapConfig mapConfig = ConfigManager.Instance.GetConfig<MapConfig>(ConfigName.Map);
        float mapSizeOnWorld = mapSize * mapConfig.mapChunkSize * mapConfig.cellSize;
        playerTransformData = new PlayerTransformData() {
            position = new Vector3(mapSizeOnWorld / 2, mapSizeOnWorld / 2),
            rotation = Vector3.zero
        };
        SavePlayerTransformData();
        // 3. 地图数据
        mapData = new MapData();
        SaveMapData();
    }

    // 加载当前存档
    public void LoadCurrentArchive() {
        // 地图初始化数据
        mapInitData = SaveManager.LoadObject<MapInitData>(0);
        // 玩家数据
        playerTransformData = SaveManager.LoadObject<PlayerTransformData>(0);
        // 地图数据
        mapData = SaveManager.LoadObject<MapData>(0);
    }
}
