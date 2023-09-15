using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 科技存档数据
[Serializable]
public class ScienceData
{
    public List<ulong> UnlockScienceList = new List<ulong>(10);
    
    // 检测科技是否解锁
    public bool CheckUnlock(ulong configId) {
        return UnlockScienceList.Contains(configId);
    }

    // 添加科技
    public void AddScience(ulong configId) {
        if (UnlockScienceList.Contains(configId) == false) {
            UnlockScienceList.Add(configId);
        }
    }
}
