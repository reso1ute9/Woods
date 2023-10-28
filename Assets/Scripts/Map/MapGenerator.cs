using System.Security.AccessControl;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using JKFrame;

public class MapGenerator
{   
    // 层级: 地图 -> 地图块 -> 网格 -> 像素
    // 约定: 地图/地图块/网格/像素均为正方形
    #region 运行时的逻辑
    private MapGrid mapGrid;            // 地图逻辑网格/顶点数据   
    private Material marshMaterial;     // 沼泽材质(非配置材质)
    private Mesh mapChunkMesh;          // 地图块mesh
    private int mapObjectForestWeightTotal;    
    private int mapObjectMarshWeightTotal;
    private int AIObjectForestWeightTotal;    
    private int AIObjectMarshWeightTotal;
    #endregion    

    #region 配置
    private Dictionary<MapVertexType, List<int>> spawnMapObjectConfigDict;      // 地图配置数据
    private Dictionary<MapVertexType, List<int>> spawnAIObjectConfigDict;       // AI配置数据: 某个类型可以生成哪些AI对象Id
    private MapConfig mapConfig;
    
    #endregion
    
    #region 存档
    private MapInitData mapInitData;
    private MapData mapData;
    #endregion

    public MapGenerator(
        MapConfig mapConfig, MapInitData mapInitData, MapData mapData, 
        Dictionary<MapVertexType, List<int>> spawnMapObjectConfigDict,
        Dictionary<MapVertexType, List<int>> spawnAIObjectConfigDict
    ) {
        this.mapConfig = mapConfig;
        this.mapInitData = mapInitData;
        this.mapData = mapData;
        this.spawnMapObjectConfigDict = spawnMapObjectConfigDict;
        this.spawnAIObjectConfigDict = spawnAIObjectConfigDict;
    }
    
    // 生成通用地图数据
    public void GenerateMapData() {
        // 生成网格/顶点数据
        mapGrid = new MapGrid(
            mapInitData.mapSize * mapConfig.mapChunkSize, 
            mapInitData.mapSize * mapConfig.mapChunkSize, mapConfig.cellSize
        );
        // 生成perlin噪声图, 需要提前设定地图noise生成种子
        UnityEngine.Random.InitState(mapInitData.mapSeed);
        float[,] noiseMap = GenerateNoiseMap(
            mapInitData.mapSize * mapConfig.mapChunkSize, 
            mapInitData.mapSize * mapConfig.mapChunkSize, mapConfig.noiseLacunarity
        );
        // 确定各个顶点的类型以及计算周围网格贴图的索引数字
        mapGrid.CalculateMapVertexType(noiseMap, mapInitData.marshLimit);
        // 实例化森林默认材质尺寸
        mapConfig.mapMaterial.mainTexture = mapConfig.forestTexture;
        mapConfig.mapMaterial.SetTextureScale(
            "_MainTex", 
            new Vector2(mapConfig.cellSize * mapConfig.mapChunkSize, mapConfig.cellSize * mapConfig.mapChunkSize)
        );
        // 实例化一个沼泽材质
        marshMaterial = new Material(mapConfig.mapMaterial);
        marshMaterial.SetTextureScale("_MainTex", Vector2.one);
        // 生成地图块mesh
        mapChunkMesh = GenerateMapChunkMesh(mapConfig.mapChunkSize, mapConfig.mapChunkSize, mapConfig.cellSize);
        // 设定地图物品随机种子
        UnityEngine.Random.InitState(mapInitData.spawnSeed);
        // 计算地图物品配置总权重
        this.mapObjectForestWeightTotal = 0;
        List<int> temp = spawnMapObjectConfigDict[MapVertexType.Forest];
        for (int i = 0; i < temp.Count; i++) {
            this.mapObjectForestWeightTotal += ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, temp[i]).probability;   
        }
        this.mapObjectMarshWeightTotal = 0;
        temp = spawnMapObjectConfigDict[MapVertexType.Marsh];
        for (int i = 0; i < temp.Count; i++) {
            this.mapObjectMarshWeightTotal += ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, temp[i]).probability;   
        }
        // 计算AI对象配置总权重
        this.AIObjectForestWeightTotal = 0;
        temp = spawnAIObjectConfigDict[MapVertexType.Forest];
        for (int i = 0; i < temp.Count; i++) {
            this.AIObjectForestWeightTotal += ConfigManager.Instance.GetConfig<AIConfig>(ConfigName.AI, temp[i]).probability;   
        }
        this.AIObjectMarshWeightTotal = 0;
        temp = spawnAIObjectConfigDict[MapVertexType.Marsh];
        for (int i = 0; i < temp.Count; i++) {
            this.AIObjectMarshWeightTotal += ConfigManager.Instance.GetConfig<AIConfig>(ConfigName.AI, temp[i]).probability;   
        }
    }

    // 在指定位置生成地图块控制器
    public MapChunkController GenerateMapChunk(
        Vector2Int chunkIndex, Transform parent, MapChunkData mapChunkData, Action callBackForMapTexture
    ) {
        // 生成地图块物体
        GameObject mapChunkObj = new GameObject("Chunk_" + chunkIndex.ToString());
        MapChunkController mapChunk = mapChunkObj.AddComponent<MapChunkController>();
        // 生成mesh
        mapChunkObj.AddComponent<MeshFilter>().mesh = mapChunkMesh;

        // 生成地图块的贴图, 性能优化-分帧执行
        Texture2D mapTexture;
        this.StartCoroutine(GenerateMapChunkTexture(chunkIndex, (texture, isAllForset) => {
            // 如果当前地图块全部是森林, 则不需要实例化一个材质球
            if (isAllForset == true) {
                mapChunkObj.AddComponent<MeshRenderer>().sharedMaterial = mapConfig.mapMaterial;
            } else {
                mapTexture = texture;
                Material material = new Material(marshMaterial);
                material.mainTexture = texture;
                mapChunkObj.AddComponent<MeshRenderer>().material = material;
            }
            callBackForMapTexture?.Invoke();
            // 确定坐标
            Vector3 position = new Vector3(
                chunkIndex.x * mapConfig.mapChunkSize * mapConfig.cellSize, 
                0, 
                chunkIndex.y * mapConfig.mapChunkSize * mapConfig.cellSize
            );
            mapChunk.transform.position = position;
            mapChunkObj.transform.SetParent(parent);

            // 如果没有指定地图块数据则说明是新建的, 需要生成默认数据
            if (mapChunkData == null) {
                // 生成场景物体数据
                mapChunkData = GenerateMapChunkDataOnMapChunkInit(chunkIndex);
                // 生成以后进行持久化保存
                ArchiveManager.Instance.AddAndSaveMapChunkData(chunkIndex, mapChunkData);
            } else {
                // 恢复mapChunkData中的VertexList数据
                RecoverMapChunkData(chunkIndex, mapChunkData);
            }
            mapChunk.Init(
                chunkIndex, isAllForset, mapChunkData
            );
            // 如果目前游戏没有完成初始化需要告知GameSceneManager更新进度
            if (GameSceneManager.Instance.IsInitialized == false) {
                GameSceneManager.Instance.OnGenerateMapChunkSucceed();
            }
        }));
        return mapChunk;
    }

    // 生成地图Mesh: height/width = mapChunkSize, cellSize = cellSize
    public Mesh GenerateMapChunkMesh(int height, int width, float cellSize) {
        Mesh mesh = new Mesh();
        // 确定顶点在哪里
        mesh.vertices = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, height * cellSize),
            new Vector3(width * cellSize, 0, height * cellSize),
            new Vector3(width * cellSize, 0, 0)
        };
        // 确定哪些点形成三角形
        mesh.triangles = new int[] {
            0, 1, 2,
            0, 2, 3
        };
        mesh.uv = new Vector2[] {
            new Vector3(0, 0),
            new Vector3(0, 1),
            new Vector3(1, 1),
            new Vector3(1, 0)
        };
        // 重新计算法线
        mesh.RecalculateNormals();
        return mesh;
    }

    // 生成perlin噪声图, 该噪声图是为确定顶点对应的位置是否为森林/沼泽
    // width: mapInitData.mapSize * mapConfig.mapChunkSize, 横着看所有cell的个数
    // height: mapInitData.mapSize * mapConfig.mapChunkSize, 竖着看所有cell的个数
    public float[,] GenerateNoiseMap(int width, int height, float lacunarity) {
        lacunarity += 0.1f;
        float[,] noiseMap = new float[width, height];
        
        float offsetX = UnityEngine.Random.Range(-10000f, 10000f);
        float offsetY = UnityEngine.Random.Range(-10000f, 10000f);

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                noiseMap[x, z] = Mathf.PerlinNoise(
                    x * lacunarity + offsetX, z * lacunarity + offsetY
                );
            }
        }
        return noiseMap;
    }

    // 分帧生成地图块纹理
    // Returns: 如果贴图块全是森林则直接返回森林
    private IEnumerator GenerateMapChunkTexture(Vector2Int chunkIndex, System.Action<Texture2D, bool> callBack) {
        // 当前地图块的偏移量, 找到这个地图块每块具体的格子位置
        int cellOffsetX = chunkIndex.x * mapConfig.mapChunkSize + 1;
        int cellOffsetZ = chunkIndex.y * mapConfig.mapChunkSize + 1;
        // 确定是否都为森林, 如果都为森林则不进行重复渲染
        bool isAllForest = true;
        for (int z = 0; z < mapConfig.mapChunkSize; z++) {
            for (int x = 0; x < mapConfig.mapChunkSize; x++) {
                MapCell cell = mapGrid.GetCell(x + cellOffsetX, z + cellOffsetZ);
                if (cell != null && cell.textureIndex != 0) {
                    isAllForest = false;
                    break;
                }
            }
            if (isAllForest == false) break;
        }
        Texture2D mapTexture = null;
        if (isAllForest == false) {
            // 约定好贴图都是矩形, 计算整个地图块texture的宽高
            // 注意: mapConfig.mapChunkSize为当前chunk内cell的数量
            int textureCellSize = mapConfig.forestTexture.width;
            int textureSize = mapConfig.mapChunkSize * textureCellSize;
            mapTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);
            // 遍历格子并绘制格子中的每个像素点
            for (int z = 0; z < mapConfig.mapChunkSize; z++) {
                // 利用协程实现分帧绘制像素, 一帧只绘制一列像素, 利用多帧时间完成整个格子内的像素绘制
                yield return null;
                int pixelOffsetZ = z * textureCellSize;
                 for (int x = 0; x < mapConfig.mapChunkSize; x++) {
                    int pixelOffsetX = x * textureCellSize;
                    int textureIndex = mapGrid.GetCell(x + cellOffsetX, z + cellOffsetZ).textureIndex - 1; // -1代表forestTexture, >=0代表marshTextures
                    // 绘制每一个格子内的像素
                    for (int z1 = 0; z1 < textureCellSize; z1++) {
                        for (int x1 = 0; x1 < textureCellSize; x1++) {
                            // 设置像素点颜色
                            // 格子是森林 || 格子是沼泽但是像素点是透明的也需要绘制forestTexture同位置的像素颜色
                            // 需要注意半透明也需要重新绘制地图
                            if (textureIndex == -1 || mapConfig.marshTextures[textureIndex].GetPixel(x1, z1).a < 1) {
                                Color color = mapConfig.forestTexture.GetPixel(x1, z1);
                                mapTexture.SetPixel(x1 + pixelOffsetX, z1 + pixelOffsetZ, color);
                            } else {
                                Color color = mapConfig.marshTextures[textureIndex].GetPixel(x1, z1);
                                mapTexture.SetPixel(x1 + pixelOffsetX, z1 + pixelOffsetZ, color);
                            }
                        }
                    }
                }
            }
            mapTexture.filterMode = FilterMode.Point;
            mapTexture.wrapMode = TextureWrapMode.Clamp;
            // 必须进行apply
            mapTexture.Apply();
        }
        // 回调: 如果结果不等于Null则执行
        callBack?.Invoke(mapTexture, isAllForest);
    }

    // 通过地图权重配置得到一个地图物品id
    private int GetMapObjectConfigIdForWeight(MapVertexType mapVertexType) {
        // 根据权重配置随机生成物品
        List<int> configIds = spawnMapObjectConfigDict[mapVertexType];
        // 确定地图对象权重总和
        int mapObjectWeightTotal = mapVertexType == MapVertexType.Forest ? mapObjectForestWeightTotal : mapObjectMarshWeightTotal;
        // 确定生成物品索引
        int randValue = UnityEngine.Random.Range(1, mapObjectWeightTotal + 1);
        float prob_sum = 0.0f;
        int spawnConfigIndex = 0;
        for (int i = 0; i < configIds.Count; i++) {
            prob_sum += ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, configIds[i]).probability;
            if (prob_sum > randValue) {
                spawnConfigIndex = i;
                break;
            }
        }
        return configIds[spawnConfigIndex];
    }

    // 通过AI权重配置得到一个AI对象id
    private int GetAIObjectConfigIdForWeight(MapVertexType mapVertexType) {
        // 根据权重配置随机生成物品
        List<int> configIds = spawnAIObjectConfigDict[mapVertexType];
        // 确定地图对象权重总和
        int AIObjectWeightTotal = mapVertexType == MapVertexType.Forest ? AIObjectForestWeightTotal : AIObjectMarshWeightTotal;
        // 确定生成物品索引
        int randValue = UnityEngine.Random.Range(1, AIObjectWeightTotal + 1);
        float prob_sum = 0.0f;
        int spawnConfigIndex = 0;
        for (int i = 0; i < configIds.Count; i++) {
            prob_sum += ConfigManager.Instance.GetConfig<AIConfig>(ConfigName.AI, configIds[i]).probability;
            if (prob_sum > randValue) {
                spawnConfigIndex = i;
                break;
            }
        }
        return configIds[spawnConfigIndex];
    }

    // 生成一个地图对象的地图数据
    private MapObjectData GenerateMapObjectData(int mapObjectConfigId, Vector3 position, int destoryDay) {
        MapObjectData mapObjectData = PoolManager.Instance.GetObject<MapObjectData>();
        mapObjectData.id = mapData.currentId;
        mapData.currentId += 1;
        mapObjectData.configId = mapObjectConfigId;
        mapObjectData.position = position;
        mapObjectData.destoryDay = destoryDay;
        return mapObjectData;
    }
    
    // 生成一个地图对象数据
    public MapObjectData GenerateMapObjectData(int mapObjectConfigId, Vector3 position) {
        MapObjectData mapObjectData = null;
        MapObjectConfig mapObjectConfig = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, mapObjectConfigId);
        if (mapObjectConfig.isEmpty == false) {
            return GenerateMapObjectData(mapObjectConfigId, position, mapObjectConfig.destoryDay);
        }
        return mapObjectData;
    }

    // 生成地图块数据, 需要根据配置和地图网格信息确定生成对象位置
    private MapChunkData GenerateMapChunkDataOnMapChunkInit(Vector2Int chunkIndex) {
        MapChunkData mapChunkData = new MapChunkData();
        mapChunkData.mapObjectDataDict = new Serialization_Dict<ulong, MapObjectData>();
        mapChunkData.AIDataDict = new Serialization_Dict<ulong, MapObjectData>();
        mapChunkData.forestVertexList = new List<MapVertex>();
        mapChunkData.marshVertexList = new List<MapVertex>();

        int offsetX = chunkIndex.x * mapConfig.mapChunkSize;
        int offsetZ = chunkIndex.y * mapConfig.mapChunkSize;
        // 生成地图对象数据
        for (int x = 1; x < mapConfig.mapChunkSize; x++) {
            for (int z = 1; z < mapConfig.mapChunkSize; z++) {
                MapVertex mapVertex = mapGrid.GetVertex(x + offsetX, z + offsetZ);
                if (mapVertex.vertexType == MapVertexType.Forest) {
                    mapChunkData.forestVertexList.Add(mapVertex);
                } else if (mapVertex.vertexType == MapVertexType.Marsh) {
                    mapChunkData.marshVertexList.Add(mapVertex);
                }
                // 确定生成数据
                int configId = GetMapObjectConfigIdForWeight(mapVertex.vertexType);
                MapObjectConfig mapObjectConfig = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, configId);
                if (mapObjectConfig.isEmpty == false) {
                    Vector3 offset = new Vector3(
                        UnityEngine.Random.Range(-mapConfig.cellSize/2, mapConfig.cellSize/2), 
                        0,
                        UnityEngine.Random.Range(-mapConfig.cellSize/2, mapConfig.cellSize/2)
                    );
                    Vector3 position = mapVertex.position + offset;
                    mapVertex.mapObjectId = mapData.currentId;
                    mapChunkData.mapObjectDataDict.dictionary.Add(
                        mapData.currentId, 
                        GenerateMapObjectData(configId, position, mapObjectConfig.destoryDay)
                    );
                }
            }
        }
        // 生成AI对象数据: 一个地图块森林或沼泽的顶点数要超过配置的才能生成AI对象
        List<MapObjectData> aiObjectDataList = GenerateAIObjectData(mapChunkData);
        for (int i = 0; i < aiObjectDataList.Count; i++) {
            mapChunkData.AIDataDict.dictionary.Add(aiObjectDataList[i].id, aiObjectDataList[i]);
        }
        return mapChunkData;
    }

    // 在地图块刷新时生成地图对象列表
    List<MapObjectData> mapObjectDataList = new List<MapObjectData>();
    public List<MapObjectData> GenerateMapObjectDataOnMapChunkRefresh(Vector2Int chunkIndex) {
        mapObjectDataList.Clear();
        int offsetX = chunkIndex.x * mapConfig.mapChunkSize;
        int offsetZ = chunkIndex.y * mapConfig.mapChunkSize;
        for (int x = 1; x < mapConfig.mapChunkSize; x++) {
            for (int z = 1; z < mapConfig.mapChunkSize; z++) {
                // 查看顶点随机值是否满足当前地图刷新概率要求
                if (UnityEngine.Random.Range(1, 101) > mapConfig.mapChunkRefreshProbability) {
                    continue;
                }
                // 如果当前网格顶点存在地图对象则不进行刷新
                MapVertex mapVertex = mapGrid.GetVertex(x + offsetX, z + offsetZ);
                if (mapVertex.mapObjectId != 0) {
                    continue;
                }
                // 确定生成物品
                int configId = GetMapObjectConfigIdForWeight(mapVertex.vertexType);
                MapObjectConfig mapObjectConfig = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, configId);
                if (mapObjectConfig.isEmpty == false) {
                    // 实例化物品
                    Vector3 offset = new Vector3(
                        UnityEngine.Random.Range(-mapConfig.cellSize/2, mapConfig.cellSize/2), 
                        0,
                        UnityEngine.Random.Range(-mapConfig.cellSize/2, mapConfig.cellSize/2)
                    );
                    Vector3 position = mapVertex.position + offset;
                    mapVertex.mapObjectId = mapData.currentId;
                    mapObjectDataList.Add(
                        GenerateMapObjectData(configId, position, mapObjectConfig.destoryDay)
                    );
                }
            }
        }
        return mapObjectDataList;
    }

    // 生成一个AI对象数据列表
    public List<MapObjectData> GenerateAIObjectData(MapChunkData mapChunkData) {
        mapObjectDataList.Clear();
        // 生成AI对象数据: 一个地图块森林或沼泽的顶点数要超过配置的才能生成AI对象
        int maxCount = mapConfig.maxAIOnChunk - mapChunkData.AIDataDict.dictionary.Count;
        if (mapChunkData.forestVertexList.Count > mapConfig.generateAIMinVertexCount) {
            for (int i = 0; i < maxCount; i++) {
                MapObjectData mapObjectData = GenerateMapAIData(MapVertexType.Forest);
                if (mapObjectData != null) {
                    mapObjectDataList.Add(mapObjectData);
                    maxCount -= 1;
                }
            }
        }
        if (mapChunkData.marshVertexList.Count > mapConfig.generateAIMinVertexCount) {
            for (int i = 0; i < maxCount; i++) {
                MapObjectData mapObjectData = GenerateMapAIData(MapVertexType.Marsh);
                if (mapObjectData != null) {
                    mapObjectDataList.Add(mapObjectData);
                }
            }
        }
        return mapObjectDataList;
    }

    // 生成一个AI数据
    public MapObjectData GenerateMapAIData(MapVertexType mapVertexType) {
        int configId = GetAIObjectConfigIdForWeight(mapVertexType);
        AIConfig aiConfig = ConfigManager.Instance.GetConfig<AIConfig>(ConfigName.AI, configId);
        if (aiConfig.isEmpty == false) {
            return GenerateMapObjectData(configId, Vector3.zero, aiConfig.destoryDay);
        }
        return null;
    }
    
    // 恢复地图块数据: 目前只恢复VertexList结果, 其他数据都通过序列化/反序列化的方式获得了
    private void RecoverMapChunkData(Vector2Int chunkIndex, MapChunkData mapChunkData) {
        mapChunkData.forestVertexList = new List<MapVertex>();
        mapChunkData.marshVertexList = new List<MapVertex>();

        int offsetX = chunkIndex.x * mapConfig.mapChunkSize;
        int offsetZ = chunkIndex.y * mapConfig.mapChunkSize;
        
        for (int x = 1; x < mapConfig.mapChunkSize; x++) {
            for (int z = 1; z < mapConfig.mapChunkSize; z++) {
                MapVertex mapVertex = mapGrid.GetVertex(x + offsetX, z + offsetZ);
                if (mapVertex.vertexType == MapVertexType.Forest) {
                    mapChunkData.forestVertexList.Add(mapVertex);
                } else if (mapVertex.vertexType == MapVertexType.Marsh) {
                    mapChunkData.marshVertexList.Add(mapVertex);
                }
            }
        }
    }
}
