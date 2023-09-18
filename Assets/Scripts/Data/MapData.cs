using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// 地图初始化数据
[Serializable]
public class MapInitData {
    public int mapSize;         // 地图大小(地图块数量)
    public int mapSeed;
    public int spawnSeed;
    public float marshLimit;
}

// 地图数据
[Serializable]                                 
public class MapData {
    // 当前地图对象id取值
    public ulong currentId = 1;
    // 地图块索引列表: 已经生成过的所有地图块
    public List<Serialization_Vector2> MapChunkIndexList = new List<Serialization_Vector2>();
}

// 地图块对象数据
[Serializable] 
public class MapObjectData {
    public ulong id;                                // 地图中地图对象id
    public int configId;                            // 地图物体配置id
    public int destoryDay;                          // 地图对象销毁天数, -1代表无效
    private Serialization_Vector3 sv_position;      // 坐标: sv_postion存档用, position外部调用用
    public Vector3 position { 
        get => sv_position.ConverToVector3(); 
        set => sv_position = value.ConverToSVector3();
    }
}

// 地图块数据
[Serializable]
public class MapChunkData {
    // 当前地图上的所有地图对象
    public Serialization_Dict<ulong, MapObjectData> mapObjectDataDict;
    public Serialization_Dict<ulong, MapObjectData> AIDataDict;
    // 记录当前地图块顶点数量
    [NonSerialized]
    public List<MapVertex> forestVertexList; 
    [NonSerialized]
    public List<MapVertex> marshVertexList; 
}