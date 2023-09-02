using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.Rendering.PostProcessing;



public class MapChunkController : MonoBehaviour
{
    public MapChunkData mapChunkData { get; private set; }
    private Dictionary<ulong, MapObjectBase> mapObjectDict;                     // 记录当前数据块中的地图对象
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
        this.mapObjectDict = new Dictionary<ulong, MapObjectBase>(mapChunkData.mapObjectDict.dictionary.Count);
        this.wantDestoryMapObjectDict = new Dictionary<ulong, MapObjectData>();
        foreach (var item in mapChunkData.mapObjectDict.dictionary.Values) {
            if (item.destoryDay > 0) {
                this.wantDestoryMapObjectDict.Add(item.id, item);
            }
        }
        this.isAllForest = isAllForest;
        this.isInitialized = true;
        // 添加地图块刷新事件
        EventManager.AddEventListener(EventName.OnMorning, OnMorning);
    }

    private void InstantiateMapObject(MapObjectData mapObjectData) {
        // 获取该地图对象配置信息
        MapObjectConfig mapObjectConfig = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.mapObject, mapObjectData.configId);
        // 从对象池中获取
        // MapObjectBase mapObject = PoolManager.Instance.GetGameObject<MapObjectBase>(mapObjectConfig.prefab, transform);
        MapObjectBase mapObject = PoolManager.Instance.GetGameObject(mapObjectConfig.prefab, transform).GetComponent<MapObjectBase>();
        mapObject.transform.position = mapObjectData.position;
        mapObject.Init(this, mapObjectData.id);
        mapObjectDict.Add(mapObjectData.id, mapObject);
    }

    // 当前chunk设置为可显示时需要显示出地图+地图中的对象(树, 石头...)
    public void SetActive(bool active) {
        if (isActive != active) {
            isActive = active;
            gameObject.SetActive(isActive);
            // 从对象池生成/回收所有对象
            if (isActive == true) {
                foreach (var mapObject in mapChunkData.mapObjectDict.dictionary.Values) {
                    InstantiateMapObject(mapObject);
                }
            } else {
                // 注意放回的时候放的时mapObjectList中的对象
                foreach (var mapObject in mapChunkData.mapObjectDict.dictionary) {
                    mapObject.Value.JKObjectPushPool();
                }
                mapObjectDict.Clear();
            }
        }
    }

    // 添加一个地图对象: 默认仅MapManager调用该方法
    public void AddMapObject(MapObjectData mapObjectData) {
        // 添加存档数据
        mapChunkData.mapObjectDict.dictionary.Add(mapObjectData.id, mapObjectData);
        if (mapObjectData.destoryDay > 0) {
            this.wantDestoryMapObjectDict.Add(mapObjectData.id, mapObjectData);
        }
        // 实例化物品
        if (isActive == true) {
            InstantiateMapObject(mapObjectData);
        }
    }

    // 移除一个地图对象
    public void RemoveMapObject(ulong mapObjectId) {
        // 数据层面移除(控制器层面)
        mapChunkData.mapObjectDict.dictionary.Remove(mapObjectId, out MapObjectData removeMapObjectData);
        removeMapObjectData.JKObjectPushPool();
        // 显示层面移除
        if (mapObjectDict.TryGetValue(mapObjectId, out MapObjectBase mapObjectBase)) {
            mapObjectBase.JKGameObjectPushPool();
            mapObjectDict.Remove(mapObjectId);
        }
        // UI层面移除
        MapManager.Instance.RemoveMapObject(mapObjectId);
    }

    // 当整个地图块数据被销毁时(例如关闭游戏), 需要将当前新的地图块数据保存到磁盘上
    private void OnDestroy() {
        ArchiveManager.Instance.SaveMapChunkData(chunkIndex, mapChunkData);    
    }

    #region 监听事件
    // 当早晨时需要触发事件刷新物体
    private void OnMorning() {
        // 遍历可能需要销毁的地图对象, 更新销毁时间
        foreach (var item in wantDestoryMapObjectDict.Values) {
            item.destoryDay -= 1;
            if (item.destoryDay == 0) {
                UnityEngine.Debug.Log("需要销毁物品id:" + item.id);
                UnityEngine.Debug.Log("需要销毁物品config id:" + item.configId);
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
            AddMapObject(mapObjectDatas[i]);
        }
    }
    #endregion
}
