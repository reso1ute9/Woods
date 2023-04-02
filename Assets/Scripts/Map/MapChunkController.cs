using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class MapChunkData
{
    public List<MapChunkMapObjectModel> mapObjectList = new List<MapChunkMapObjectModel>();
}

public class MapChunkMapObjectModel
{   
    public int configId;                    // 之前为prefab, 改为int后方便存档
    public Vector3 position;
}

public class MapChunkController : MonoBehaviour
{
    private List<GameObject> mapObjectList;         // 记录当前数据块中的对象
    public MapChunkData mapChunkData { get; private set; }

    public Vector3 centrePosition { get; private set; }
    public Vector2Int chunkIndex { get; private set; }
    public bool isAllForest { get; private set; }   
    private bool isActive = false;
    public bool isInitialized = false;              // 标记地图UI是否已经初始化

    public void Init(Vector2Int chunkIndex, Vector3 centrePosition, bool isAllForest, List<MapChunkMapObjectModel> mapObjectList) {
        this.centrePosition = centrePosition;
        this.chunkIndex = chunkIndex;
        // 初始化地图块数据
        this.mapChunkData = new MapChunkData();
        this.mapChunkData.mapObjectList = mapObjectList;
        this.mapObjectList = new List<GameObject>(mapObjectList.Count);
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
                for (int i = 0; i < mapChunkData.mapObjectList.Count; i++) {
                    // 找到目前地图块上所有mapObject id, 根据id去查找预制体
                    MapObjectConfig config = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.mapObject, mapChunkData.mapObjectList[i].configId);
                    GameObject t_object = PoolManager.Instance.GetGameObject(config.prefab, transform);
                    t_object.transform.position = mapChunkData.mapObjectList[i].position;
                    this.mapObjectList.Add(t_object);
                }
            } else {
                // 注意放回的时候放的时mapObjectList中的对象
                // for (int i = 0; i < mapObjectList.Count; i++) {
                for (int i = 0; i < mapChunkData.mapObjectList.Count; i++) {
                    PoolManager.Instance.PushGameObject(mapObjectList[i]);
                }
                mapObjectList.Clear();
            }
        }
    }
}
