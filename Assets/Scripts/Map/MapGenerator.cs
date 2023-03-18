using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapGenerator : MonoBehaviour
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
    private Texture2D groundTexture;
    private Texture2D[] marshTextures;
    private MapConfig mapConfig;     // 地图配置数据

    // 生成通用地图块数据
    public void GenerateMapData() {
        // 生成网格/顶点数据
        mapGrid = new MapGrid(mapSize * mapChunkSize, mapSize * mapChunkSize, cellSize);
        // 生成perlin噪声图
        float[,] noiseMap = GenerateNoiseMap(mapSize * mapChunkSize, mapSize * mapChunkSize, noiseLacunarity, mapSeed);
        // 确定各个顶点的类型以及计算周围网格贴图的索引数字
        mapGrid.CalculateMapVertexType(noiseMap, marshLimit);
    }

    // 在指定位置生成地图块
    public MapChunkController GenerateMapChunk(Vector2Int chunkIndex, Transform parent) {
        // 生成地图块物体
        GameObject mapChunkObj = new GameObject("Chunk_" + chunkIndex.ToString());
        MapChunkController mapChunk = mapChunkObj.AddComponent<MapChunkController>();
        // 生成mesh
        mapChunkObj.AddComponent<MeshFilter>().mesh = GenerateMapMesh(mapChunkSize, mapChunkSize, cellSize);
        // 生成地图块的贴图
        // TODO: 性能优化-分帧执行
        Texture2D mapTexture = GenerateMapTexture(cellTextureIndexMap, groundTexture, marshTextures);
        mapChunkObj.AddComponent<MeshRenderer>().sharedMaterial.mainTexture = mapTexture;
        // 确定坐标
        Vector3 position = new Vector3(
            chunkIndex.x * mapChunkSize * cellSize, 
            0, 
            chunkIndex.y * mapChunkSize * cellSize
        );
        mapChunk.transform.position = position;
        mapChunkObj.transform.SetParent(parent);
        mapChunk.Init(position + new Vector3((mapChunkSize * cellSize) / 2, 0, (mapChunkSize * cellSize) / 2));
        // 生成游戏场景物体
        // TODO: 后续处理
        SpawnMapObject(mapGrid, mapConfig, spawnSeed);
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
    public float[,] GenerateNoiseMap(int width, int height, float lacunarity, int mapSeed) {
        UnityEngine.Random.InitState(mapSeed);
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

    // 生成地图贴图
    private Texture2D GenerateMapTexture(int[,] cellTextureIndexMap, Texture2D groundTexture, Texture2D[] marshTextures) {
        // 地图宽高
        int mapWidth = cellTextureIndexMap.GetLength(0);
        int mapHeight = cellTextureIndexMap.GetLength(1);
        // 约定好贴图都是矩形
        int textureCellSize = groundTexture.width;
        Texture2D mapTexture = new Texture2D(mapWidth * textureCellSize, mapHeight * textureCellSize, TextureFormat.RGB24, false);
        // 遍历格子
        for (int z = 0; z < mapHeight; z++) {
            int offsetZ = z * textureCellSize;
            for (int x = 0; x < mapWidth; x++) {
                int offsetX = x * textureCellSize;
                int textureIndex = cellTextureIndexMap[x, z] - 1; // -1代表groundTexture, >=0代表marshTextures
                // 绘制每一个格子内的像素
                for (int z1 = 0; z1 < textureCellSize; z1++) {
                    for (int x1 = 0; x1 < textureCellSize; x1++) {
                        // 设置像素点颜色
                        // 格子是森林 || 格子是沼泽但是像素点是透明的也需要绘制groundTexture同位置的像素颜色
                        if (textureIndex == -1 || marshTextures[textureIndex].GetPixel(x1, z1).a == 0) {
                            Color color = groundTexture.GetPixel(x1, z1);
                            mapTexture.SetPixel(x1 + offsetX, z1 + offsetZ, color);
                        } else {
                            Color color = marshTextures[textureIndex].GetPixel(x1, z1);
                            mapTexture.SetPixel(x1 + offsetX, z1 + offsetZ, color);
                        }
                    }
                }
            }
        }
        mapTexture.filterMode = FilterMode.Point;
        mapTexture.wrapMode = TextureWrapMode.Clamp;
        // 必须进行apply
        mapTexture.Apply();
        return mapTexture;
    }

    // 生成各种地图对象, 需要根据配置和地图网格信息确定生成对象位置
    private void SpawnMapObject(MapGrid mapGrid, MapConfig spawnConfig, int spawnSeed) {
        # region 
        // 移除生成的游戏对象
        for (int i = 0; i < mapObjects.Count; i++) {
            DestroyImmediate(mapObjects[i]);
        }
        mapObjects.Clear();
        # endregion 
        // 设定随机种子进行随机生成
        UnityEngine.Random.InitState(spawnSeed);
        int mapHeight = mapGrid.mapHeight;
        int mapWidth = mapGrid.mapWidth;
        for (int x = 0; x < mapWidth; x++) {
            for (int z = 0; z < mapHeight; z++) {
                MapVertex mapVertex = mapGrid.GetVertex(x, z);
                Debug.Log("x:" + x + " z:" + z + " mapVertex.vertexType:" + mapVertex.vertexType);
                // 根据概率配置随机
                List<MapObjectSpawnConfigModel> configModels = spawnConfig.mapObjectConfig[mapVertex.vertexType];
                int randValue = UnityEngine.Random.Range(1, 101);
                // 前提是在配置文件中所有物品出现的可能性之和为100
                float prob_sum = 0.0f;
                int spawnConfigIndex = 0;
                for (int i = 0; i < configModels.Count; i++) {
                    prob_sum += configModels[i].probability;
                    if (prob_sum > randValue) {
                        spawnConfigIndex = i;
                        break;
                    }
                }
                Debug.Log("x:" + x + " z:" + z + " spawnConfigIndex:" + spawnConfigIndex);
                // 确定到底生成什么物品
                MapObjectSpawnConfigModel spawnModel = configModels[spawnConfigIndex];
                if (spawnModel.isEmpty == false) {
                    // 实例化物品
                    Vector3 offset = new Vector3(
                        UnityEngine.Random.Range(-cellSize/2, cellSize/2), 
                        0,
                        UnityEngine.Random.Range(-cellSize/2, cellSize/2)
                    );
                    GameObject temp = GameObject.Instantiate(spawnModel.prefab, mapVertex.position + offset, Quaternion.identity, transform);
                    mapObjects.Add(temp);   
                }
            }
        }
    }
}
