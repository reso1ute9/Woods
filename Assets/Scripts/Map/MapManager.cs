using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;


// 地图管理器需要调用MapGenerator去生成地图上的信息
public class MapManager : MonoBehaviour
{
    // 地图尺寸
    public int mapSize;             // 地图大小
    public int mapChunkSize;        // 地图块大小
    public float cellSize;          // 网格大小
    private float chunkSizeOnWorld; // 在世界中实际的地图块尺寸
    // 地图随机参数
    public float noiseLacunarity;   // 噪声图采样间隔大小
    public int mapSeed;             // 地图随机种子
    public int spawnSeed;           // 地图对象种子
    public float marshLimit;        // 沼泽高度阈值
    // 地图美术资源
    public Texture2D forestTexture;    // 森林贴图    
    public Texture2D[] marshTextures;  // 沼泽贴图
    public Material mapMaterial;       // 地图材质
    // 地图生成器
    private MapGenerator mapGenerator;  // 地图生成器
    public int viewDistance;            // 玩家可视距离, 单位为ChunkSize
    public Transform viewer;            // 观察者
    private Vector3 lastViewerPosition = Vector3.one * -1;          // 观察者最后的位置, 用以控制地图是否进行刷新
    public float UpdateVisibleChunkTime = 1.0f;                     // 地图更新最小时间
    private bool canUpdateChunk = true;                             // 地图是否能进行更新
    public Dictionary<Vector2Int, MapChunkController> mapChunkDict; // 全部已有的地图块
    private List<MapChunkController> lastVisibleChunkList = new List<MapChunkController>();

    private Dictionary<MapVertexType, List<int>> spawnConfigDict;   // 地图配置数据

    private void Start() {
        // 确定配置
        Dictionary<int, ConfigBase> temp_dict = ConfigManager.Instance.GetConfigs(ConfigName.mapObject);
        spawnConfigDict = new Dictionary<MapVertexType, List<int>>();
        spawnConfigDict.Add(MapVertexType.Forest, new List<int>());
        spawnConfigDict.Add(MapVertexType.Marsh, new List<int>());
        foreach (var item in temp_dict) {
            MapVertexType mapVertexType = (item.Value as MapObjectConfig).mapVertexType;
            spawnConfigDict[mapVertexType].Add(item.Key);
        }

        // 初始化地图生成器
        mapGenerator = new MapGenerator(
            mapSize, mapChunkSize, cellSize, 
            noiseLacunarity, mapSeed, spawnSeed, marshLimit, 
            forestTexture, marshTextures, mapMaterial, spawnConfigDict
        );
        mapGenerator.GenerateMapData();
        mapChunkDict = new Dictionary<Vector2Int, MapChunkController>();
        chunkSizeOnWorld = mapChunkSize * cellSize;
    }

    private void Update() {
        UpdateVisibleChunk();
    }

    // 根据观察者的位置去刷新地图块
    private void UpdateVisibleChunk() {
        // 如果观察者没有移动或者时间没到则不需要刷新
        if (viewer.position == lastViewerPosition || canUpdateChunk == false) {
            return;
        }
        // 当前观察者所在地图块
        Vector2Int currChunkIndex = GetMapChunkIndexByWorldPosition(viewer.position);
        // 关闭不需要显示的地图块
        for (int i = lastVisibleChunkList.Count - 1; i >= 0; i--) {
            Vector2Int chunkIndex = lastVisibleChunkList[i].chunkIndex;
            // 查看当前chunk是否需要显示, 只需要检查x,y是否在viewDistance范围里
            if (Mathf.Abs(chunkIndex.x - currChunkIndex.x) > viewDistance || 
                Mathf.Abs(chunkIndex.y - currChunkIndex.y) > viewDistance) {
                lastVisibleChunkList[i].SetActive(false);
                lastVisibleChunkList.RemoveAt(i);
            }
        }
        // 开启需要显示的地图块
        int startX = currChunkIndex.x - viewDistance;
        int startY = currChunkIndex.y - viewDistance;
        for (int x = 0; x < 2 * viewDistance + 1; x++) {
            for (int y = 0; y < 2 * viewDistance + 1; y++) {
                // 控制地图块刷新时间, 当时间间隔>=UpdateVisibleChunkTime时才可进行刷新
                canUpdateChunk = false;
                Invoke("ResetCanUpdateChunkFlag", UpdateVisibleChunkTime);
                Vector2Int chunkIndex = new Vector2Int(startX + x, startY + y);
                // 之前加载过则直接使用dict中cache的结果, 否则需要重新绘制
                if (mapChunkDict.TryGetValue(chunkIndex, out MapChunkController chunk)) {
                    // 是否在显示列表中
                    if (lastVisibleChunkList.Contains(chunk) == false) {
                        chunk.SetActive(true);
                        lastVisibleChunkList.Add(chunk);
                    }
                } else {
                    chunk = GenerateMapChunk(chunkIndex);
                    if (chunk != null) {
                        chunk.SetActive(true);
                        lastVisibleChunkList.Add(chunk);
                    }
                }
            }
        }
    }
    
    // 通过世界坐标去获取地图块索引
    private Vector2Int GetMapChunkIndexByWorldPosition(Vector3 worldIndex) {
        int x = Mathf.Clamp(Mathf.RoundToInt(worldIndex.x / chunkSizeOnWorld), 1, mapSize);
        int z = Mathf.Clamp(Mathf.RoundToInt(worldIndex.z / chunkSizeOnWorld), 1, mapSize);
        return new Vector2Int(x, z);
    }

    private MapChunkController GenerateMapChunk(Vector2Int index) {
        if (index.x > mapSize-1 || index.y > mapSize-1) return null;
        if (index.x < 0 || index.y < 0) return null;
        MapChunkController chunk = mapGenerator.GenerateMapChunk(index, transform);
        mapChunkDict.Add(index, chunk);
        return chunk;
    }

    // 重置更新chunk的标志
    private void ResetCanUpdateChunkFlag() {
        canUpdateChunk = true;
    }
}
