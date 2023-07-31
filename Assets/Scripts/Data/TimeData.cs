using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 时间数据
[Serializable]
public class TimeData
{
    public int stateIndex = 0;          // 游戏时间状态配置
    public float calcTime = 0;          // 时间倒计时
    public int dayNum;                  // 天数
}
