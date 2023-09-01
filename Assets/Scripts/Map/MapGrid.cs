using System.Runtime.InteropServices.ComTypes;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

// 网格: 主要包含顶点和格子
public class MapGrid
{
    // 顶点数据
    public Dictionary<Vector2Int, MapVertex> vertexDict = new Dictionary<Vector2Int, MapVertex>();
    // 格子数据
    public Dictionary<Vector2Int, MapCell> cellDict = new Dictionary<Vector2Int, MapCell>();
    // 地图信息
    public int mapHeight { get; private set; }
    public int mapWidth { get; private set; }
    public float cellSize { get; private set; }

    public MapGrid(int mapHeight, int mapWidth, float cellSize) {
        this.mapHeight = mapHeight;
        this.mapWidth = mapWidth;
        this.cellSize = cellSize;

        // // 生成顶点数据和格子数据
        for (int x = 0; x < this.mapWidth; x++) {
            for (int z = 0; z < this.mapHeight; z++) {
                AddVertext(x, z);
                AddCell(x, z);
            }
        }
    }

    # region MapVertextCode
    // 根据输入的地图坐标点添加地图网格顶点
    private void AddVertext(int x, int z) {
        vertexDict.Add(
            new Vector2Int(x, z), 
            new MapVertex() { position = new Vector3(x * cellSize, 0, z * cellSize) }
        );
    }

    // 获取顶点时找不到则返回null
    public MapVertex GetVertex(Vector2Int index) {
        MapVertex res = null;
        vertexDict.TryGetValue(index, out res);
        return res;
    }

    public MapVertex GetVertex(int x, int y) {
        return GetVertex(new Vector2Int(x, y));
    }

    public MapVertex GetVertexByWorldPosition(Vector3 position) {
        int x = Mathf.Clamp(Mathf.RoundToInt(position.x / cellSize), 1, mapWidth);
        int z = Mathf.Clamp(Mathf.RoundToInt(position.z / cellSize), 1, mapHeight);
        return GetVertex(x, z);
    }

    private void SetVertexType(Vector2Int index, MapVertexType vertexType) {
        MapVertex vertex = GetVertex(index);
        if (vertex.vertexType != vertexType) {
            vertex.vertexType = vertexType;
            // 计算附近的贴图权重
            if (vertex.vertexType == MapVertexType.Marsh) {
                MapCell tempCell = GetLeftBottomMapCell(index);
                if (tempCell != null) tempCell.textureIndex += 1; 

                tempCell = GetRightBottomMapCell(index);
                if (tempCell != null) tempCell.textureIndex += 2;

                tempCell = GetLeftTopMapCell(index);
                if (tempCell != null) tempCell.textureIndex += 4; 

                tempCell = GetRightTopMapCell(index);
                if (tempCell != null) tempCell.textureIndex += 8; 
            } else {
                // TODO: 由于森林不需要计算贴图或者是随机从后面选择贴图, 所以暂时忽略
            }
        }
    }

    private void SetVertexType(int x, int z, MapVertexType vertexType) {
        SetVertexType(new Vector2Int(x, z), vertexType);
    }
    # endregion

    # region MapCellCode
    // 为顶点添加一个格子
    private void AddCell(int x, int z) {
        float offset = cellSize / 2;
        cellDict.Add(
            new Vector2Int(x, z), 
            new MapCell() { position = new Vector3(x * cellSize - offset, 0, z * cellSize - offset) }
        );
    }

    // 获取格子时找不到则返回null
    public MapCell GetCell(Vector2Int index) {
        MapCell res = null;
        cellDict.TryGetValue(index, out res);
        return res;
    }

    public MapCell GetCell(int x, int z) {
        return GetCell(new Vector2Int(x, z));
    }

    public MapCell GetLeftTopMapCell(Vector2Int vertexIndex) {
        return GetCell(vertexIndex.x, vertexIndex.y + 1);
    }

    public MapCell GetLeftBottomMapCell(Vector2Int vertexIndex) {
        return GetCell(vertexIndex.x, vertexIndex.y);
    }

    public MapCell GetRightTopMapCell(Vector2Int vertexIndex) {
        return GetCell(vertexIndex.x + 1, vertexIndex.y + 1);
    }

    public MapCell GetRightBottomMapCell(Vector2Int vertexIndex) {
        return GetCell(vertexIndex.x + 1, vertexIndex.y);
    }
    # endregion

    // 计算格子贴图的索引数字
    public void CalculateMapVertexType(float[,] noiseMap, float limit) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                // 根据噪声图的值确认网格类型, 大于边界是沼泽
                // noiseMap是从0开始的
                if (noiseMap[x, z] >= limit) {
                    SetVertexType(x, z, MapVertexType.Marsh);
                } else {
                    SetVertexType(x, z, MapVertexType.Forest);
                }
            }
        }
    }
}

// 顶点类型
public enum MapVertexType {
    None,   // 默认类型
    Forest, // 森林
    Marsh,  // 沼泽 
}

// 地图顶点
public class MapVertex
{
    public Vector3 position;
    public MapVertexType vertexType;
    public ulong mapObjectId;           // 当前地图顶点上的物体对象, 0代表为空
}

// 地图格子
public class MapCell
{
    public Vector3 position;
    public int textureIndex;
}