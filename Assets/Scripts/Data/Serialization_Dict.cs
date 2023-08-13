using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;

// 可序列化的字典
[Serializable]
public class Serialization_Dict <K, V>
{
    private List<K> keyList;
    private List<V> valList;

    [NonSerialized]
    private Dictionary<K, V> t_dictionary;    // 不需要进行序列化

    public Dictionary<K, V> dictionary { get => t_dictionary; }


    public Serialization_Dict() {
        t_dictionary = new Dictionary<K, V>();
    }

    public Serialization_Dict(Dictionary<K, V> dict) {
        t_dictionary = dict;
    }

    // 序列化: 将字典转化为可以存入磁盘的形式
    [OnSerializing]
    private void OnSerializing(StreamingContext streamingContext) {
        keyList = new List<K>(dictionary.Count);
        valList = new List<V>(dictionary.Count);
        foreach (var item in dictionary) {
            keyList.Add(item.Key);
            valList.Add(item.Value);
        }
    }

    // 反序列化: 恢复原始字典
    [OnDeserialized]
    private void OnDeserialized(StreamingContext streamingContext) {
        t_dictionary = new Dictionary<K, V>(keyList.Count);
        for (int i = 0; i < keyList.Count; i++) {
            t_dictionary.Add(keyList[i], valList[i]);
        }
    }
}
