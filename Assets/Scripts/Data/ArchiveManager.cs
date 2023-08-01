using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;


public class ArchiveManager : Singleton<ArchiveManager>
{
    public PlayerTransformData playerTransformData { get; private set; }    // 当前存档玩家位置数据
    public PlayerMainData playerMainData { get; private set; }              // 当前存档玩家主要数据
    public MapInitData mapInitData { get; private set; }                    // 当前存档地图初始化参数
    public MapData mapData { get; private set; }                            // 当前存档地图数据
    public InventoryData inventoryData { get; private set; }                // 当前存档物品快捷栏数据
    public TimeData timeData { get; private set; }                          // 当前存档时间数据
    public bool haveArchive { get; private set; }                           // 判断当前情况是否有存档
    

    public ArchiveManager() {
        LoadSaveData();
    }

    // 加载存档系统里的数据
    public void LoadSaveData() {
        // 单存档情况下默认获取
        SaveItem saveItem = SaveManager.GetSaveItem(0);
        haveArchive = (saveItem != null);
    }

    // 保存时间数据
    public void SaveTimeData() {
        SaveManager.SaveObject(timeData);
    }

    // 保存物品快捷栏数据
    public void SaveInventoryData() {
        SaveManager.SaveObject(inventoryData);
    }

    // 保存玩家位置数据存档到磁盘上
    public void SavePlayerTransformData() {
        SaveManager.SaveObject(playerTransformData);
    }

    // 保存玩家主要属性到磁盘上
    public void SavePlayerMainData() {
        SaveManager.SaveObject(playerMainData);
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
        // 2. 存储玩家初始化数据: 1. 位置数据； 2. 血量、饱食度
        MapConfig mapConfig = ConfigManager.Instance.GetConfig<MapConfig>(ConfigName.Map);
        float mapSizeOnWorld = mapSize * mapConfig.mapChunkSize * mapConfig.cellSize;
        playerTransformData = new PlayerTransformData() {
            position = new Vector3(mapSizeOnWorld / 2, 0, mapSizeOnWorld / 2),
            rotation = Vector3.zero
        };
        SavePlayerTransformData();

        PlayerConfig playerConfig = ConfigManager.Instance.GetConfig<PlayerConfig>(ConfigName.Player);
        playerMainData = new PlayerMainData() {
            hp = playerConfig.maxHP,
            hungry = playerConfig.maxHungry
        };
        SavePlayerMainData();

        // 3. 地图数据
        mapData = new MapData();
        SaveMapData();
        // 4. 初始化物品快捷栏数据, 默认14个快捷栏
        inventoryData = new InventoryData(14);

        #region 物品快捷栏测试数据
        inventoryData.itemDatas[0] = ItemData.CreateItemData(0);
        (inventoryData.itemDatas[0].itemTypeData as ItemMaterialData).count = 10;
        inventoryData.itemDatas[1] = ItemData.CreateItemData(1);
        inventoryData.itemDatas[2] = ItemData.CreateItemData(2);
        (inventoryData.itemDatas[2].itemTypeData as ItemWeaponData).durability = 60;
        inventoryData.itemDatas[3] = ItemData.CreateItemData(3);
        (inventoryData.itemDatas[3].itemTypeData as ItemConsumableData).count = 10;
        #endregion
        
        SaveInventoryData();
        
        // 5. 初始化时间数据
        TimeConfig timeConfig = ConfigManager.Instance.GetConfig<TimeConfig>(ConfigName.Time);
        timeData = new TimeData {
            stateIndex = 0, 
            calcTime = timeConfig.timeStateConfig[1].durationTime, 
            dayNum = 1
        };
        SaveTimeData();
    }

    // 加载当前存档
    public void LoadCurrentArchive() {
        // 地图初始化数据
        mapInitData = SaveManager.LoadObject<MapInitData>(0);
        // 玩家数据
        playerTransformData = SaveManager.LoadObject<PlayerTransformData>(0);
        playerMainData = SaveManager.LoadObject<PlayerMainData>(0);
        // 地图数据
        mapData = SaveManager.LoadObject<MapData>(0);
        // 物品快捷栏
        inventoryData = SaveManager.LoadObject<InventoryData>(0);
        // 时间数据
        timeData = SaveManager.LoadObject<TimeData>(0);
    }
}
