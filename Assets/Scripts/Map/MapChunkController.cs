using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.AI;



public class MapChunkController : MonoBehaviour
{
    public MapChunkData mapChunkData { get; private set; }
    public Dictionary<ulong, MapObjectBase> mapObjectDict;                     // 记录当前数据块中的地图对象
    public Dictionary<ulong, AIBase> AIObjectDict;    
    private Dictionary<ulong, MapObjectData> wantDestoryMapObjectDict;          // 记录当前需要销毁的地图对象
    private static List<ulong> wantDestoryMapObjectId = new List<ulong>(20);    // 记录当前需要销毁的地图对象id

    public Vector3 centrePosition { get; private set; }
    public Vector2Int chunkIndex { get; private set; }                          // 当前地图块索引
    public bool isAllForest { get; private set; }   
    private bool isActive = false;
    public bool isInitialized = false;                                          // 标记地图UI是否已经初始化

    public void Init(Vector2Int chunkIndex, Vector3 centrePosition, bool isAllForest, MapChunkData mapChunkData) {
        this.centrePosition = centrePosition;
        this.chunkIndex = chunkIndex;
        // 初始化地图块数据
        this.mapChunkData = mapChunkData;
        this.mapObjectDict = new Dictionary<ulong, MapObjectBase>(mapChunkData.mapObjectDataDict.dictionary.Count);
        this.AIObjectDict = new Dictionary<ulong, AIBase>(mapChunkData.AIDataDict.dictionary.Count);
        this.wantDestoryMapObjectDict = new Dictionary<ulong, MapObjectData>();
        foreach (var item in mapChunkData.mapObjectDataDict.dictionary.Values) {
            if (item.destoryDay > 0) {
                this.wantDestoryMapObjectDict.Add(item.id, item);
            }
        }
        this.isAllForest = isAllForest;
        this.isInitialized = true;
        // 添加地图块刷新事件
        EventManager.AddEventListener(EventName.OnMorning, OnMorning);
        
        EventManager.AddEventListener(EventName.SaveGame, OnGameSave);
    }

    // 地图对象实例化方法
    private void InstantiateMapObject(MapObjectData mapObjectData, bool isFromBuild) {
        // 获取该地图对象配置信息
        MapObjectConfig mapObjectConfig = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, mapObjectData.configId);
        // 从对象池中获取
        // MapObjectBase mapObject = PoolManager.Instance.GetGameObject<MapObjectBase>(mapObjectConfig.prefab, transform);
        MapObjectBase mapObject = PoolManager.Instance.GetGameObject(mapObjectConfig.prefab, transform).GetComponent<MapObjectBase>();
        mapObject.transform.position = mapObjectData.position;
        mapObject.Init(this, mapObjectData.id, isFromBuild);
        mapObjectDict.Add(mapObjectData.id, mapObject);
    }

    // AI对象实例化方法
    private void InstantiateAIObject(MapObjectData AIObjectData) {
        // 获取该地图对象配置信息
        AIConfig AIObjectConfig = ConfigManager.Instance.GetConfig<AIConfig>(ConfigName.AI, AIObjectData.configId);
        // 从对象池中获取
        AIBase AIObject = PoolManager.Instance.GetGameObject(AIObjectConfig.prefab, transform).GetComponent<AIBase>();
        // 未初始化坐标
        if (AIObjectData.position == Vector3.zero) {
            AIObjectData.position = GetAIRandomPosition(AIObjectConfig.mapVertexType);
        }
        AIObject.Init(this, AIObjectData);
        AIObjectDict.Add(AIObjectData.id, AIObject);
    }

    // 获取AI可以到达的随机坐标
    public Vector3 GetAIRandomPosition(MapVertexType mapVertexType) {
        List<MapVertex> mapVertexList = null;
        if (mapVertexType == MapVertexType.Forest) {
            // 顶点不够时会出现bug, 因此判断当顶点不够时使用另一份数据
            if (mapChunkData.forestVertexList.Count < MapManager.Instance.MapConfig.generateAIMinVertexCount) {
                mapVertexList = mapChunkData.marshVertexList;
            } else {
                mapVertexList = mapChunkData.forestVertexList;
            }
        } else if (mapVertexType == MapVertexType.Marsh) {
            // 顶点不够时会出现bug, 因此判断当顶点不够时使用另一份数据
            if (mapChunkData.marshVertexList.Count < MapManager.Instance.MapConfig.generateAIMinVertexCount) {
                mapVertexList = mapChunkData.forestVertexList;
            } else {
                mapVertexList = mapChunkData.marshVertexList;
            }
        }
        int index = Random.Range(0, mapVertexList.Count);
        // 确保AI可以到达该位置
        if (NavMesh.SamplePosition(mapVertexList[index].position, out NavMeshHit hitInfo, 1, NavMesh.AllAreas)) {
            return hitInfo.position;
        }
        return GetAIRandomPosition(mapVertexType);
    }

    // 当前chunk设置为可显示时需要显示出地图+地图中的对象(树, 石头...)
    public void SetActive(bool active) {
        if (isActive != active) {
            isActive = active;
            gameObject.SetActive(isActive);
            // 从对象池生成/回收所有对象
            if (isActive) {
                // 处理地图对象
                foreach (var mapObject in mapChunkData.mapObjectDataDict.dictionary.Values) {
                    InstantiateMapObject(mapObject, false);
                }
                // 处理AI对象
                foreach (var mapObject in mapChunkData.AIDataDict.dictionary.Values) {
                    InstantiateAIObject(mapObject);
                }
            } else {
                // 处理地图对象: 注意放回的时候放的时mapObjectList中的对象
                foreach (var mapObject in mapObjectDict) {
                    mapObject.Value.JKGameObjectPushPool();
                }
                // 处理AI对象
                foreach (var aiObject in AIObjectDict) {
                    aiObject.Value.Destroy();
                }
                mapObjectDict.Clear();
                AIObjectDict.Clear();
            }
        }
    }

    // 添加一个地图对象: 默认仅MapManager调用该方法
    public void AddMapObject(MapObjectData mapObjectData, bool isFromBuild) {
        // 添加存档数据
        mapChunkData.mapObjectDataDict.dictionary.Add(mapObjectData.id, mapObjectData);
        if (mapObjectData.destoryDay > 0) {
            this.wantDestoryMapObjectDict.Add(mapObjectData.id, mapObjectData);
        }
        // 实例化物品
        if (isActive == true) {
            InstantiateMapObject(mapObjectData, isFromBuild);
        }
    }

    // 添加一个AI对象: 默认仅MapManager调用该方法
    public void AddAIObject(MapObjectData aiObjectData) {
        // 添加存档数据
        mapChunkData.AIDataDict.dictionary.Add(aiObjectData.id, aiObjectData);
        // 实例化物品
        if (isActive) {
            InstantiateAIObject(aiObjectData);
        }
    }

    // 移除一个地图对象
    public void RemoveMapObject(ulong mapObjectId) {
        // 数据层面移除(控制器层面)
        mapChunkData.mapObjectDataDict.dictionary.Remove(mapObjectId, out MapObjectData removeMapObjectData);
        removeMapObjectData.JKObjectPushPool();
        // 显示层面移除
        if (mapObjectDict.TryGetValue(mapObjectId, out MapObjectBase mapObjectBase)) {
            mapObjectBase.JKGameObjectPushPool();
            mapObjectDict.Remove(mapObjectId);
        }
        // UI层面移除
        MapManager.Instance.RemoveMapObject(mapObjectId);
    }

    // 添加一个AI物体(物体迁移)
    public void AddAIOjbectOnTransfer(MapObjectData aiObjectData, AIBase aiObject) {
        mapChunkData.AIDataDict.dictionary.Add(aiObjectData.id, aiObjectData);
        AIObjectDict.Add(aiObjectData.id, aiObject);
        aiObject.transform.SetParent(transform);
        aiObject.InitOnTransfer(this);
    }

    // 删除一个AI物体: AI游戏物体、数据存档都需要移除
    public void RemoveAIObject(ulong AIObjectId) {
        // 数据层面
        mapChunkData.AIDataDict.dictionary.Remove(AIObjectId, out MapObjectData aiObjectData);
        aiObjectData.JKObjectPushPool();
        // AI游戏物体
        if (AIObjectDict.Remove(AIObjectId, out AIBase aiObject)) {
            aiObject.Destroy();
        }
    }

    // 删除一个AI物体(物体迁移): 只删除数据不删除AI对象
    public void RemoveAIObjectOnTransfer(ulong aiObjectId) {
        mapChunkData.AIDataDict.dictionary.Remove(aiObjectId);
        AIObjectDict.Remove(aiObjectId);
    }

    // 当整个地图块数据被销毁时(例如关闭游戏), 需要将当前新的地图块数据保存到磁盘上
    private void OnGameSave() {
        ArchiveManager.Instance.SaveMapChunkData(chunkIndex, mapChunkData);    
    }
    
    // 当关闭游戏场景时释放资源
    public void OnCloseGameScene() {
        SetActive(false);
    }

    #region 监听事件
    // 当早晨时需要触发事件刷新物体
    private void OnMorning() {
        // 遍历可能需要销毁的地图对象, 更新销毁时间
        foreach (var item in wantDestoryMapObjectDict.Values) {
            item.destoryDay -= 1;
            if (item.destoryDay == 0) {
                wantDestoryMapObjectId.Add(item.id);
            }
        }
        for (int i = 0; i < wantDestoryMapObjectId.Count; i++) {
            RemoveMapObject(wantDestoryMapObjectId[i]);
        }
        wantDestoryMapObjectId.Clear();

        // 获得刷新后新增的地图块中包含的地图对象
        List<MapObjectData> mapObjectDatas = MapManager.Instance.GenerateMapObjectDataOnMapChunkRefresh(chunkIndex);
        for (int i = 0; i < mapObjectDatas.Count; i++) {
            AddMapObject(mapObjectDatas[i], false);
        }

        // 刷新AI: 每隔x天刷新一次AI
        if (TimeManager.Instance.currentDayNum % 3 == 0) {
            List<MapObjectData> aiObjectDataList = MapManager.Instance.GenerateAIObjectDataOnMapChunkRefresh(mapChunkData);
            for (int i = 0; i < aiObjectDataList.Count; i++) {
                AddAIObject(aiObjectDataList[i]);
            }
        }
    }
    #endregion
}
