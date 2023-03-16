using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapGenerator : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public int mapHeight;
    public int mapWidth;
    public float cellSize;
    public float lacunarity;
    public int seed;

    private MapGrid grid;
    // 生成地图
    [Button("生成地图")]
    public void GenerateMap() {
        // 生成mesh
        meshFilter.mesh = GenerateMapMesh(mapHeight, mapWidth, cellSize);
        // 生成网格贴图
        grid = new MapGrid(mapHeight, mapWidth, cellSize);
        // 生成噪声图
        float[,] noiseMap = GenerateNoiseMap(mapWidth, mapHeight, lacunarity, seed);
    }

    public GameObject testObject;
    [Button("测试顶点")] 
    public void TestVertex() {
        print(grid.GetVertexByWorldPosition(testObject.transform.position).position);
    }

    [Button("测试格子")] 
    public void TestCell(Vector2Int index) {
        print("LT:" + grid.GetLeftTopMapCell(index).position);
        print("LB:" + grid.GetLeftBottomMapCell(index).position);
        print("RT:" + grid.GetRightTopMapCell(index).position);
        print("RB:" + grid.GetRightBottomMapCell(index).position);
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
    public float[,] GenerateNoiseMap(int width, int height, float lacunarity, int seed) {
        UnityEngine.Random.InitState(seed);
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
}
