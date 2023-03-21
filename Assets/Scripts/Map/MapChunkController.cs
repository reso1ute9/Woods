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
    public GameObject prefab;
    public Vector3 position;
}

public class MapChunkController : MonoBehaviour
{
    private List<GameObject> mapObjectList;         // 记录当前数据块中的对象
    public MapChunkData mapChunkData;               // 

    public Vector3 centrePosition { get; private set; }
    public Vector2Int chunkIndex { get; private set; }
    private bool isActive = false;

    public void Init(Vector2Int chunkIndex, Vector3 centrePosition, List<MapChunkMapObjectModel> mapObjectList) {
        this.centrePosition = centrePosition;
        this.chunkIndex = chunkIndex;
        // 初始化地图块数据
        this.mapChunkData = new MapChunkData();
        this.mapChunkData.mapObjectList = mapObjectList;
        this.mapObjectList = new List<GameObject>(mapObjectList.Count);
    }

    // 当前chunk设置为可显示时需要显示出地图+地图中的对象(树, 石头...)
    public void SetActive(bool active) {
        if (isActive != active) {
            isActive = active;
            gameObject.SetActive(isActive);
            // 从对象池生成/回收所有对象
            if (isActive == true) {
                for (int i = 0; i < mapChunkData.mapObjectList.Count; i++) {
                    GameObject t_object = PoolManager.Instance.GetGameObject(mapChunkData.mapObjectList[i].prefab, transform);
                    t_object.transform.position = mapChunkData.mapObjectList[i].position;
                    this.mapObjectList.Add(t_object);
                }
            } else {
                // 注意放回的时候放的时mapObjectList中的对象
                for (int i = 0; i < mapObjectList.Count; i++) {
                    PoolManager.Instance.PushGameObject(mapObjectList[i]);
                }
                mapObjectList.Clear();
            }
        }
    }
}
