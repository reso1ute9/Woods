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
    private int mapSize;             // 地图大小
    private int mapChunkSize;        // 地图块大小
    private float cellSize;          // 网格大小

    private float noiseLacunarity;   // 噪声图采样间隔大小
    private int mapSeed;             // 地图随机种子
    private int spawnSeed;           // 地图对象种子
    private float marshLimit;        // 沼泽高度阈值
    private MapGrid mapGrid;         // 地图逻辑网格/顶点数据    

    private Texture2D forestTexture;    // 森林贴图    
    private Texture2D[] marshTextures;  // 沼泽贴图
    private Material mapMaterial;       // 森林材质(默认)
    private Material marshMaterial;     // 沼泽材质
    private Mesh mapChunkMesh;          // 地图块mesh

    private Dictionary<MapVertexType, List<int>> spawnConfigDict;   // 地图配置数据
    private int forestSpawnWeightTotal;
    private int marshSpawnWeightTotal;

    public MapGenerator(
        int mapSize, int mapChunkSize, float cellSize, 
        float noiseLacunarity, int mapSeed, int spawnSeed, float marshLimit, 
        Texture2D forestTexture, Texture2D[] marshTextures, Material mapMaterial,
        Dictionary<MapVertexType, List<int>> spawnConfigDict
    ) {
        this.mapSize = mapSize;
        this.mapChunkSize = mapChunkSize; 
        this.cellSize = cellSize;

        this.noiseLacunarity = noiseLacunarity;
        this.mapSeed = mapSeed;
        this.spawnSeed = spawnSeed;
        this.marshLimit = marshLimit;

        this.forestTexture = forestTexture;
        this.marshTextures = marshTextures;
        this.mapMaterial = mapMaterial;

        this.spawnConfigDict = spawnConfigDict;

        this.GenerateMapData();
    }
    
    // 生成通用地图块数据
    public void GenerateMapData() {
        // 生成网格/顶点数据
        mapGrid = new MapGrid(mapSize * mapChunkSize, mapSize * mapChunkSize, cellSize);
        // 生成perlin噪声图, 需要提前设定地图noise生成种子
        UnityEngine.Random.InitState(mapSeed);
        float[,] noiseMap = GenerateNoiseMap(mapSize * mapChunkSize, mapSize * mapChunkSize, noiseLacunarity);
        // 确定各个顶点的类型以及计算周围网格贴图的索引数字
        mapGrid.CalculateMapVertexType(noiseMap, marshLimit);
        // 实例化森林默认材质尺寸
        mapMaterial.mainTexture = forestTexture;
        mapMaterial.SetTextureScale("_MainTex", new Vector2(cellSize * mapChunkSize, cellSize * mapChunkSize));
        // 实例化一个沼泽材质
        marshMaterial = new Material(mapMaterial);
        marshMaterial.SetTextureScale("_MainTex", Vector2.one);
        // 生成地图块mesh
        mapChunkMesh = GenerateMapMesh(mapChunkSize, mapChunkSize, cellSize);
        // 设定地图物品随机种子
        UnityEngine.Random.InitState(spawnSeed);
        // 计算地图物品配置总权重
        this.forestSpawnWeightTotal = 0;
        List<int> temp = spawnConfigDict[MapVertexType.Forest];
        for (int i = 0; i < temp.Count; i++) {
            this.forestSpawnWeightTotal += ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.mapObject, temp[i]).probability;   
        }
        this.marshSpawnWeightTotal = 0;
        temp = spawnConfigDict[MapVertexType.Forest];
        for (int i = 0; i < temp.Count; i++) {
            this.marshSpawnWeightTotal += ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.mapObject, temp[i]).probability;   
        }
    }

    // 在指定位置生成地图块
    public MapChunkController GenerateMapChunk(Vector2Int chunkIndex, Transform parent, Action callBackForMapTexture) {
        // 生成地图块物体
        GameObject mapChunkObj = new GameObject("Chunk_" + chunkIndex.ToString());
        MapChunkController mapChunk = mapChunkObj.AddComponent<MapChunkController>();
        
        // 生成mesh
        mapChunkObj.AddComponent<MeshFilter>().mesh = mapChunkMesh;
        // 添加碰撞体: 会自动将过滤器的网格碰撞体抓过来作为mapChunkObj的MeshCollider
        mapChunkObj.AddComponent<MeshCollider>();

        // 生成地图块的贴图, 性能优化-分帧执行
        Texture2D mapTexture;
        this.StartCoroutine(GenerateMapTexture(chunkIndex, (texture, isAllForset) => {
            // 如果当前地图块全部是森林, 则不需要实例化一个材质球
            if (isAllForset == true) {
                mapChunkObj.AddComponent<MeshRenderer>().sharedMaterial = mapMaterial;
            } else {
                mapTexture = texture;
                Material material = new Material(marshMaterial);
                material.mainTexture = texture;
                mapChunkObj.AddComponent<MeshRenderer>().material = material;
            }
            callBackForMapTexture?.Invoke();
            // 确定坐标
            Vector3 position = new Vector3(
                chunkIndex.x * mapChunkSize * cellSize, 
                0, 
                chunkIndex.y * mapChunkSize * cellSize
            );
            mapChunk.transform.position = position;
            mapChunkObj.transform.SetParent(parent);
            // 生成场景物体数据
            List<MapChunkMapObjectModel> mapObjectList = SpawnMapObject(chunkIndex);
            mapChunk.Init(
                chunkIndex,
                position + new Vector3((mapChunkSize * cellSize) / 2, 0, (mapChunkSize * cellSize) / 2),
                isAllForset, mapObjectList
            );
        }));
        return mapChunk;
    }

    public Mesh GenerateMapMesh(int height, int width, float cellSize) {
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
    public float[,] GenerateNoiseMap(int width, int height, float lacunarity) {
        lacunarity += 0.1f;
        float[,] noiseMap = new float[width - 1, height - 1];
        
        float offsetX = UnityEngine.Random.Range(-10000f, 10000f);
        float offsetY = UnityEngine.Random.Range(-10000f, 10000f);

        for (int x = 0; x < width - 1; x++) {
            for (int z = 0; z < height - 1; z++) {
                noiseMap[x, z] = Mathf.PerlinNoise(
                    x * lacunarity + offsetX, z * lacunarity + offsetY
                );
            }
        }
        return noiseMap;
    }

    // 分帧生成地图块贴图
    // Returns: 如果贴图块全是森林则直接返回森林
    private IEnumerator GenerateMapTexture(Vector2Int chunkIndex, System.Action<Texture2D, bool> callBack) {
        // 当前地图块的偏移量, 找到这个地图块每块具体的格子位置
        int cellOffsetX = chunkIndex.x * mapChunkSize + 1;
        int cellOffsetZ = chunkIndex.y * mapChunkSize + 1;
        // 确定是否都为森林, 如果都为森林则不进行重复渲染
        bool isAllForest = true;
        for (int z = 0; z < mapChunkSize; z++) {
            for (int x = 0; x < mapChunkSize; x++) {
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
            int textureCellSize = forestTexture.width;
            int textureSize = mapChunkSize * textureCellSize;
            mapTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);
            // 遍历格子并绘制格子中的每个像素点
            for (int z = 0; z < mapChunkSize; z++) {
                // 利用协程实现分帧绘制像素, 一帧只绘制一列像素, 利用多帧时间完成整个格子内的像素绘制
                yield return null;
                int pixelOffsetZ = z * textureCellSize;
                for (int x = 0; x < mapChunkSize; x++) {
                    int pixelOffsetX = x * textureCellSize;
                    int textureIndex = mapGrid.GetCell(x + cellOffsetX, z + cellOffsetZ).textureIndex - 1; // -1代表forestTexture, >=0代表marshTextures
                    // 绘制每一个格子内的像素
                    for (int z1 = 0; z1 < textureCellSize; z1++) {
                        for (int x1 = 0; x1 < textureCellSize; x1++) {
                            // 设置像素点颜色
                            // 格子是森林 || 格子是沼泽但是像素点是透明的也需要绘制forestTexture同位置的像素颜色
                            // 需要注意半透明也需要重新绘制地图
                            if (textureIndex == -1 || marshTextures[textureIndex].GetPixel(x1, z1).a < 1) {
                                Color color = forestTexture.GetPixel(x1, z1);
                                mapTexture.SetPixel(x1 + pixelOffsetX, z1 + pixelOffsetZ, color);
                            } else {
                                Color color = marshTextures[textureIndex].GetPixel(x1, z1);
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

    // 生成各种地图对象, 需要根据配置和地图网格信息确定生成对象位置
    private List<MapChunkMapObjectModel> SpawnMapObject(Vector2Int chunkIndex) {
        List<MapChunkMapObjectModel> mapObjectList = new List<MapChunkMapObjectModel>();
        
        int offsetX = chunkIndex.x * mapChunkSize;
        int offsetZ = chunkIndex.y * mapChunkSize;

        for (int x = 1; x < mapChunkSize; x++) {
            for (int z = 1; z < mapChunkSize; z++) {
                MapVertex mapVertex = mapGrid.GetVertex(x + offsetX, z + offsetZ);
                // 根据权重配置随机生成物品
                List<int> configIds = spawnConfigDict[mapVertex.vertexType];
                // 确定权重总和
                int weightTotal = mapVertex.vertexType == MapVertexType.Forest ? forestSpawnWeightTotal : marshSpawnWeightTotal;
                // 确定生成物品索引
                int randValue = UnityEngine.Random.Range(1, weightTotal + 1);
                float prob_sum = 0.0f;
                int spawnConfigIndex = 0;
                for (int i = 0; i < configIds.Count; i++) {
                    prob_sum += ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.mapObject, configIds[i]).probability;
                    if (prob_sum > randValue) {
                        spawnConfigIndex = i;
                        break;
                    }
                }
                // 确定生成物品
                int configId = configIds[spawnConfigIndex];
                MapObjectConfig spawnModel = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.mapObject, configId);
                if (spawnModel.isEmpty == false) {
                    // 实例化物品
                    Vector3 offset = new Vector3(
                        UnityEngine.Random.Range(-cellSize/2, cellSize/2), 
                        0,
                        UnityEngine.Random.Range(-cellSize/2, cellSize/2)
                    );
                    Vector3 position = mapVertex.position + offset;
                    mapObjectList.Add(new MapChunkMapObjectModel { configId = configId, position = position });                
                }
            }
        }
        return mapObjectList;
    }
}
