using System.Collections;
using UnityEngine;
using JKFrame;
using System;
using Sirenix.OdinInspector;

// 配置时间: 亮度/时间/颜色
[Serializable]
public class TimeStateData
{
    public float durationTime;          // 持续时间
    public float sunIntensity;          // 阳光强度
    public Color sunColor;              // 阳光颜色
    [OnValueChanged(nameof(SetRotation))]
    public Vector3 sunRotation;         // 阳光角度
    [HideInInspector]
    public Quaternion sunQuaternion;    // 阳光角度-四元数

    // 当阳光角度发生变化, 需要计算出四元数
    private void SetRotation() {
        sunQuaternion = Quaternion.Euler(sunRotation);
    }

    // 检测并计算下一个时间配置
    public bool CheckAndCalTime(float currTime, TimeStateData nextState, out Quaternion rotation, out Color color, out float sunIntensity) {
        float ratio = 1.0f - (currTime / durationTime);       // 计算当前时间比例
        rotation = Quaternion.Lerp(this.sunQuaternion, nextState.sunQuaternion, ratio);
        color = Color.Lerp(this.sunColor, nextState.sunColor, ratio);
        sunIntensity = UnityEngine.Mathf.Lerp(this.sunIntensity, nextState.sunIntensity, ratio);
        return currTime > 0;
    }
}


// 管理游戏时间
// 当在OnEnable/OnDisable时注册或者取消监听, 单例模式
public class TimeManager : LogicManagerBase<TimeManager>
{
    [SerializeField] private Light mainLight;   // 太阳   
    [SerializeField] private TimeStateData[] timeStateDatas;  // 游戏时间状态配置
    private int currentStateIndex = 0;          // 当前游戏时间状态配置
    private float currentTime = 0;              // 当前时间倒计时
    private int dayNum;

    [SerializeField,Range(0,30)] private float timeScale = 1;
    protected override void RegisterEventListener() {}

    protected override void CancelEventListener() {}

    private void Start()
    {
        StartCoroutine(UpdateTime());
    }

    private IEnumerator UpdateTime()
    {
        currentStateIndex = 0;
        int nextIndex = currentStateIndex + 1;
        currentTime = timeStateDatas[currentStateIndex].durationTime;
        dayNum = 0;
        while (true)
        {
            yield return null;
            currentTime -= Time.deltaTime * timeScale;  // 减掉每一帧的时间, 乘上缩放系数便于debug
            // 检查当前时间并且计算得到阳光相关设置
            if (!timeStateDatas[currentStateIndex].CheckAndCalTime(
                currentTime, timeStateDatas[nextIndex], 
                out Quaternion rotation, out Color color, out float sunIntensity
            )) {
                currentStateIndex = nextIndex;
                nextIndex = (currentStateIndex + 1 >= timeStateDatas.Length) ? 0 : currentStateIndex + 1;
                // 如果现在是早上, 则天数+1
                if (currentStateIndex == 0) {
                    dayNum += 1;
                }
                currentTime = timeStateDatas[currentStateIndex].durationTime;
            }
            // 设置阳光强度/角度/颜色
            mainLight.transform.rotation = rotation;
            mainLight.color = color;
            SetLight(sunIntensity);
        }
    }

    private void SetLight(float intensity) {
        // 设置强度
        mainLight.intensity = intensity;
        // 设置环境光亮度
        RenderSettings.ambientIntensity = intensity;
    }
}
