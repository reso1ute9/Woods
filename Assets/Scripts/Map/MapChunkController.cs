using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;



public class MapChunkController : MonoBehaviour
{
    private Dictionary<ulong, MapObjectBase> mapObjectDict;         // 记录当前数据块中的对象
    public MapChunkData mapChunkData { get; private set; }

    public Vector3 centrePosition { get; private set; }
    public Vector2Int chunkIndex { get; private set; }
    public bool isAllForest { get; private set; }   
    private bool isActive = false;
    public bool isInitialized = false;                              // 标记地图UI是否已经初始化

    public void Init(Vector2Int chunkIndex, Vector3 centrePosition, bool isAllForest, MapChunkData mapChunkData) {
        this.centrePosition = centrePosition;
        this.chunkIndex = chunkIndex;
        // 初始化地图块数据
        this.mapChunkData = mapChunkData;
        this.mapObjectDict = new Dictionary<ulong, MapObjectBase>(mapChunkData.mapObjectDict.dictionary.Count);
        this.isAllForest = isAllForest;
        this.isInitialized = true;
    }

    // 当前chunk设置为可显示时需要显示出地图+地图中的对象(树, 石头...)
    public void SetActive(bool active) {
        if (isActive != active) {
            isActive = active;
            gameObject.SetActive(isActive);
            // 从对象池生成/回收所有对象
            if (isActive == true) {
                foreach (var mapObject in mapChunkData.mapObjectDict.dictionary) {
                    // 找到目前地图块上所有mapObject id, 根据id去查找预制体
                    MapObjectConfig config = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.mapObject, mapObject.Value.configId);
                    GameObject t_object = PoolManager.Instance.GetGameObject(config.prefab, transform);
                    t_object.transform.position = mapObject.Value.position;
                    // 临时测试逻辑, 因为目前只有树木继承自MapObjectBase, 其他地图对象未设置继承关系
                    if (t_object.TryGetComponent<MapObjectBase>(out MapObjectBase temp)) {
                        temp.Init(this, mapObject.Key);
                        mapObjectDict.Add(mapObject.Key, temp);
                    }
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

    // 移除一个地图对象
    public void RemoveMapObject(ulong mapObjectId) {
        // 显示层面移除
        mapObjectDict.Remove(mapObjectId);
        // 数据层面移除(控制器层面)
        mapChunkData.mapObjectDict.dictionary.Remove(mapObjectId);
        // UI层面移除
        MapManager.Instance.RemoveMapObject(mapObjectId);
    }

    // 当整个地图块数据被销毁时(例如关闭游戏), 需要将当前新的地图块数据保存到磁盘上
    private void OnDestroy() {
        ArchiveManager.Instance.SaveMapChunkData(chunkIndex, mapChunkData);    
    }
}
