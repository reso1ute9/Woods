using System.Runtime.InteropServices.ComTypes;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // 生成顶点数据和格子数据
        for (int x = 1; x < this.mapWidth; x++) {
            for (int z = 1; z < this.mapHeight; z++) {
                AddVertext(x, z);
                AddCell(x, z);
            }
        }
        for (int x = 1; x <= this.mapWidth; x++) {
            AddCell(x, mapHeight);
        }
        for (int z = 1; z < this.mapHeight; z++) {
            AddCell(mapWidth, z);
        }

        # region TestCode-1
        foreach (var item in vertexDict.Values) {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.transform.position = item.position;
            temp.transform.localScale = Vector3.one * 0.25f;
        }
        foreach (var item in cellDict.Values) {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            temp.transform.position = item.position - new Vector3(0, 0.49f, 0);
            temp.transform.localScale = new Vector3(cellSize, 1, cellSize);
        }
        # endregion 
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
    # endregion

    # region MapCellCode
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
        Vector2Int lt_index = new Vector2Int(vertexIndex.x, vertexIndex.y + 1);
        return cellDict[lt_index];
    }

    public MapCell GetLeftBottomMapCell(Vector2Int vertexIndex) {
        Vector2Int lb_index = vertexIndex;
        return cellDict[lb_index];
    }

    public MapCell GetRightTopMapCell(Vector2Int vertexIndex) {
        Vector2Int rt_index = new Vector2Int(vertexIndex.x + 1, vertexIndex.y + 1);
        return cellDict[rt_index];
    }

    public MapCell GetRightBottomMapCell(Vector2Int vertexIndex) {
        Vector2Int rb_index = new Vector2Int(vertexIndex.x + 1, vertexIndex.y);
        return cellDict[rb_index];
    }

    # endregion
}


// 地图顶点
public class MapVertex
{
    public Vector3 position;
}

// 地图格子
public class MapCell
{
    public Vector3 position;
}