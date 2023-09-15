using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

// 科技管理器
public class ScienceManager : SingletonMono<ScienceManager>
{
    private ScienceData scienceData;

    public void Init() {
        scienceData = ArchiveManager.Instance.scienceData;
    }

    // 检测科技是否解锁
    public bool CheckUnlock(ulong configId) {
        return scienceData.CheckUnlock(configId);
    }

    // 添加科技
    public void AddScience(ulong configId) {
        scienceData.AddScience(configId);
        for (int i = 0; i < scienceData.UnlockScienceList.Count; i++) {
            UnityEngine.Debug.Log("scienceData.UnlockScienceList:" + scienceData.UnlockScienceList[i]);
        }
    }
}
