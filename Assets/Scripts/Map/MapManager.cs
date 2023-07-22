using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;


// 地图管理器需要调用MapGenerator去生成地图上的信息
public class MapManager : SingletonMono<MapManager>
{

    // 地图UI相关变量
    private bool mapUIInitialized = false;
    private bool isShowMaping = false;
    private List<Vector2Int> mapUIUpdateChunkIndexList = new List<Vector2Int>();        // 地图UI待更新列表
    private UI_MapWindow mapUI;

    // 运行时逻辑
    #region 运行时逻辑
    private MapGenerator mapGenerator;  // 地图生成器
    public Transform viewer;            // 观察者
    private Vector3 lastViewerPosition = Vector3.one * -1;          // 观察者最后的位置, 用以控制地图是否进行刷新
    public Dictionary<Vector2Int, MapChunkController> mapChunkDict; // 全部已有的地图块
    public float mapSizeOnWorld;   // 在世界中世界的地图尺寸
    private float chunkSizeOnWorld; // 在世界中实际的地图块尺寸    
    public float UpdateVisibleChunkTime = 1.0f;                     // 地图更新最小时间
    private bool canUpdateChunk = true;                             // 地图是否能进行更新
    private List<MapChunkController> lastVisibleChunkList = new List<MapChunkController>();
    #endregion
    
    // 配置
    #region 配置
    public MapConfig mapConfig;     // 地图配置
    private Dictionary<MapVertexType, List<int>> spawnConfigDict;   // 地图配置数据
    #endregion

    // 存档
    #region  存档
    // public MapChunkData mapData;
    public MapInitData mapInitData;
    #endregion
    
    // 需要注意: 使用单例方式的时候需要在awke中进行初始化, 否则后续用到单例instance时
    // 可能会报错
    protected override void Awake() {
        base.Awake();
        Init();
    }

    private void Init() {
        // 确定配置
        mapConfig = ConfigManager.Instance.GetConfig<MapConfig>(ConfigName.Map);
        Dictionary<int, ConfigBase> temp_dict = ConfigManager.Instance.GetConfigs(ConfigName.mapObject);
        spawnConfigDict = new Dictionary<MapVertexType, List<int>>();
        spawnConfigDict.Add(MapVertexType.Forest, new List<int>());
        spawnConfigDict.Add(MapVertexType.Marsh, new List<int>());
        foreach (var item in temp_dict) {
            MapVertexType mapVertexType = (item.Value as MapObjectConfig).mapVertexType;
            spawnConfigDict[mapVertexType].Add(item.Key);
        }

        // 初始化地图生成器
        mapGenerator = new MapGenerator(mapConfig, mapInitData, spawnConfigDict);
        mapGenerator.GenerateMapData();
        mapChunkDict = new Dictionary<Vector2Int, MapChunkController>();
        chunkSizeOnWorld = mapConfig.mapChunkSize * mapConfig.cellSize;
        mapSizeOnWorld = chunkSizeOnWorld * mapInitData.mapSize;
    }

    private void Update() {
        UpdateVisibleChunk();

        if (Input.GetKeyDown(KeyCode.M)) {
            if (isShowMaping) {
                CloseMapUI();
            } else {
                ShowMapUI();
            }
            isShowMaping = !isShowMaping;
        }

        if (isShowMaping) {
            UpdateMapUI();
        }
    }

    // 根据观察者的位置去刷新地图块
    private void UpdateVisibleChunk() {
        // 如果观察者没有移动或者时间没到则不需要刷新
        if (viewer.position == lastViewerPosition || canUpdateChunk == false) {
            return;
        }
        // 更新地图UI坐标
        if (isShowMaping) {
            mapUI.UpdatePivot(viewer.position);
        }

        // 当前观察者所在地图块
        Vector2Int currChunkIndex = GetMapChunkIndexByWorldPosition(viewer.position);
        // 关闭不需要显示的地图块
        for (int i = lastVisibleChunkList.Count - 1; i >= 0; i--) {
            Vector2Int chunkIndex = lastVisibleChunkList[i].chunkIndex;
            // 查看当前chunk是否需要显示, 只需要检查x,y是否在viewDistance范围里
            if (Mathf.Abs(chunkIndex.x - currChunkIndex.x) > mapConfig.viewDistance || 
                Mathf.Abs(chunkIndex.y - currChunkIndex.y) > mapConfig.viewDistance) {
                lastVisibleChunkList[i].SetActive(false);
                lastVisibleChunkList.RemoveAt(i);
            }
        }
        // 开启需要显示的地图块
        int startX = currChunkIndex.x - mapConfig.viewDistance;
        int startY = currChunkIndex.y - mapConfig.viewDistance;
        for (int x = 0; x < 2 * mapConfig.viewDistance + 1; x++) {
            for (int y = 0; y < 2 * mapConfig.viewDistance + 1; y++) {
                // 控制地图块刷新时间, 当时间间隔>=UpdateVisibleChunkTime时才可进行刷新
                canUpdateChunk = false;
                Invoke("ResetCanUpdateChunkFlag", UpdateVisibleChunkTime);
                Vector2Int chunkIndex = new Vector2Int(startX + x, startY + y);
                // 之前加载过则直接使用dict中cache的结果, 否则需要重新绘制, 需要注意由于
                // 贴图是在协程中进行生成, 所以执行完毕后才算初始化完毕
                if (mapChunkDict.TryGetValue(chunkIndex, out MapChunkController chunk)) {
                    // 是否在显示列表中, 需要检查是否完成初始化
                    if (!lastVisibleChunkList.Contains(chunk) && chunk.isInitialized) {
                        chunk.SetActive(true);
                        lastVisibleChunkList.Add(chunk);
                    }
                } else {
                    chunk = GenerateMapChunk(chunkIndex);
                    // if (chunk != null) {
                    //     chunk.SetActive(true);
                    //     lastVisibleChunkList.Add(chunk);
                    // }
                }
            }
        }
    }
    
    // 通过世界坐标去获取地图块索引
    private Vector2Int GetMapChunkIndexByWorldPosition(Vector3 worldIndex) {
        int x = Mathf.Clamp(Mathf.RoundToInt(worldIndex.x / chunkSizeOnWorld), 1, mapInitData.mapSize);
        int z = Mathf.Clamp(Mathf.RoundToInt(worldIndex.z / chunkSizeOnWorld), 1, mapInitData.mapSize);
        return new Vector2Int(x, z);
    }

    private MapChunkController GenerateMapChunk(Vector2Int index) {
        if (index.x > mapInitData.mapSize-1 || index.y > mapInitData.mapSize-1) return null;
        if (index.x < 0 || index.y < 0) return null;
        // 利用回调+lambda表达式实现协程结束后将协程中生成的地图块索引加入到UI地图块显示索引列表中
        MapChunkController chunk = mapGenerator.GenerateMapChunk(index, transform, () => mapUIUpdateChunkIndexList.Add(index));
        mapChunkDict.Add(index, chunk);
        return chunk;
    }

    // 重置更新chunk的标志
    private void ResetCanUpdateChunkFlag() {
        canUpdateChunk = true;
    }

    #region 地图UI相关
    // 显示地图UI
    private void ShowMapUI() {
        mapUI = UIManager.Instance.Show<UI_MapWindow>();
        // 初始化地图UI
        if (!mapUIInitialized) {
            mapUI.InitMap(mapInitData.mapSize, mapConfig.mapChunkSize, mapSizeOnWorld, mapConfig.forestTexture);
            mapUIInitialized = true;
        }
        UpdateMapUI();
    }

    private void UpdateMapUI() {
        for (int i = 0; i < mapUIUpdateChunkIndexList.Count; i++) {
            Vector2Int chunkIndex = mapUIUpdateChunkIndexList[i];
            Texture2D texture = null;
            MapChunkController mapChunk = mapChunkDict[chunkIndex];
            if (mapChunk.isAllForest == false) {
                texture = (Texture2D)mapChunk.GetComponent<MeshRenderer>().material.mainTexture;
            }
            mapUI.AddMapChunk(chunkIndex, mapChunk.mapChunkData.mapObjectList, texture);
        }
        // 当更新到UI中后就不需要当前存储的index了
        mapUIUpdateChunkIndexList.Clear();
        // 更新玩家UI坐标
        mapUI.UpdatePivot(viewer.position);
    }

    // 关闭地图UI
    private void CloseMapUI() {
        UIManager.Instance.Close<UI_MapWindow>();
    }
    #endregion
}
