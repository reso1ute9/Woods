using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;
using System;


[CreateAssetMenu(fileName = "时间配置", menuName = "Config/时间配置")]
public class TimeConfig : ConfigBase
{
    [LabelText("时间状态数据")] 
    // 默认index = 0时为白天
    public TimeStateConfig[] timeStateConfig;     // 时间配置
}


// 配置时间: 亮度/时间/颜色
[Serializable]
public class TimeStateConfig
{
    public float durationTime;          // 持续时间
    public float sunIntensity;          // 阳光强度
    public Color sunColor;              // 阳光颜色
    [OnValueChanged(nameof(SetRotation))]
    public Vector3 sunRotation;         // 阳光角度
    [HideInInspector]
    public Quaternion sunQuaternion;    // 阳光角度-四元数

    public bool fog;                    // 迷雾
    public AudioClip bgAudioClip;       // 背景音乐

    // 当阳光角度发生变化, 需要计算出四元数
    private void SetRotation() {
        sunQuaternion = Quaternion.Euler(sunRotation);
    }

    // 检测并计算下一个时间配置
    public bool CheckAndCalTime(float currTime, TimeStateConfig nextState, out Quaternion rotation, out Color color, out float sunIntensity) {
        float ratio = 1.0f - (currTime / durationTime);       // 计算当前时间比例
        rotation = Quaternion.Lerp(this.sunQuaternion, nextState.sunQuaternion, ratio);
        color = Color.Lerp(this.sunColor, nextState.sunColor, ratio);
        sunIntensity = UnityEngine.Mathf.Lerp(this.sunIntensity, nextState.sunIntensity, ratio);
        if (fog == true) {
            // 迷雾强度随时间递减, 没有考虑多个时间点都有雾的情况(无过渡状态)
            RenderSettings.fogDensity = 0.1f * (1 - ratio);
        }
        return currTime > 0;
    }
}