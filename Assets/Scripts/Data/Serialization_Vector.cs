using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public struct Serialization_Vector2
{
    public float x, y;

    public Serialization_Vector2(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public override string ToString() {
        return $"({x}, {y})";
    }
}

[Serializable]
public struct Serialization_Vector3
{
    public float x, y, z;

    public Serialization_Vector3(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override string ToString() {
        return $"({x}, {y}, {z})";
    }
}


public static class SerializationExtensions {
    // 将可序列化的vector2转化为unity的Vector2
    public static Vector2 ConverToVector2(this Serialization_Vector2 vec2) {
        return new Vector2(vec2.x, vec2.y);
    }

    // 将可序列化的vector2转化为unity的Vector2Int
    public static Vector2Int ConverToVector2Int(this Serialization_Vector2 vec2) {
        return new Vector2Int((int)vec2.x, (int)vec2.y);
    }

    // 将unity的Vector2转化为可序列化的vector2
    public static Vector2Int ConverToSVector2Init(this Serialization_Vector2 vec2) {
        return new Vector2Int((int)vec2.x, (int)vec2.y);
    }

    // 将unity的Vector2转化为可序列化的vector2
    public static Serialization_Vector2 ConverToSVector2(this Vector2Int vec2) {
        return new Serialization_Vector2(vec2.x, vec2.y);
    }
    
    // 将可序列化的vector3转化为unity的Vector3
    public static Vector3 ConverToVector3(this Serialization_Vector3 vec3) {
        return new Vector3(vec3.x, vec3.y, vec3.z);
    }

    // 将可序列化的vector3转化为unity的Vector3Int
    public static Vector3Int ConverToVector3Int(this Serialization_Vector3 vec3) {
        return new Vector3Int((int)vec3.x, (int)vec3.y, (int)vec3.z);
    }

    // 将unity的Vector3转化为可序列化的vector3
    public static Serialization_Vector3 ConverToSVector3(this Vector3 vec3) {
        return new Serialization_Vector3(vec3.x, vec3.y, vec3.z);
    }
}





