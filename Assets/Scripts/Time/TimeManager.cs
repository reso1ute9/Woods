using System.Collections;
using UnityEngine;
using JKFrame;
using System;
using Sirenix.OdinInspector;


// 管理游戏时间
// 当在OnEnable/OnDisable时注册或者取消监听, 单例模式
public class TimeManager : LogicManagerBase<TimeManager>
{
    [SerializeField] private Light mainLight;   // 太阳(主要光照)
    private TimeData timeData;
    private TimeConfig timeConfig;
    private int nextIndex;
    public int currentDayNum { get => timeData.dayNum; }

    [SerializeField,Range(0,30)] public float timeScale = 1;
    protected override void RegisterEventListener() {}

    protected override void CancelEventListener() {}

    public void Init() {
        // 获取当前存档中时间数据
        timeData = ArchiveManager.Instance.timeData;
        timeConfig = ConfigManager.Instance.GetConfig<TimeConfig>(ConfigName.Time);
        // 触发事件
        InitState();
    }

    // 每次读取存档时需要进行的初始化状态设置
    private void InitState() {
        // 设置初始的迷雾效果: 直接使用配置中fog的值
        RenderSettings.fog = timeConfig.timeStateConfig[timeData.stateIndex].fog; 
        // 设置初始的背景音乐:
        if (timeConfig.timeStateConfig[timeData.stateIndex].bgAudioClip != null) {
            StartCoroutine(ChangeBGAudio(timeConfig.timeStateConfig[timeData.stateIndex].bgAudioClip));
        }
        // 设置初始的nextIndex
        nextIndex = (timeData.stateIndex + 1 >= timeConfig.timeStateConfig.Length) ? 0 : timeData.stateIndex + 1;
        // 触发是否为白天的状态
        EventManager.EventTrigger<bool>(EventName.UpdateTimeState, timeData.stateIndex <= 1);
        // 触发当前是否为第几天的状态
        EventManager.EventTrigger<int>(EventName.UpdateDayNum, timeData.dayNum);
    }

    private void Update() {
        if (GameSceneManager.Instance.IsInitialized == false) {
            return;
        }
        UpdateTime();
    }

    // 更新时间(当前阶段剩余时间、天数)、背景音乐、光照、迷雾等与时间相关的数据或者效果
    private void UpdateTime() {
        timeData.calcTime -= Time.deltaTime * timeScale;  // 减掉每一帧的时间, 乘上缩放系数便于debug
        // 检查当前时间并且计算得到阳光相关设置
        if (!timeConfig.timeStateConfig[timeData.stateIndex].CheckAndCalTime(
            timeData.calcTime, timeConfig.timeStateConfig[nextIndex], 
            out Quaternion rotation, out Color color, out float sunIntensity
        )) {
            EnterNextState();
        }
        SetLight(rotation, color, sunIntensity);
    }

    // 进入时间配置表中的下一个状态需要做的事情
    private void EnterNextState() {
        // 更新状态索引、天数、当前状态剩余时间
        timeData.stateIndex = nextIndex;
        // 触发是否为白天的状态
        EventManager.EventTrigger<bool>(EventName.UpdateTimeState, timeData.stateIndex <= 1);
        nextIndex = (timeData.stateIndex + 1 >= timeConfig.timeStateConfig.Length) ? 0 : timeData.stateIndex + 1;
        if (timeData.stateIndex == 0) {
            timeData.dayNum += 1;
            // 触发当前是否为第几天的状态
            EventManager.EventTrigger<int>(EventName.UpdateDayNum, timeData.dayNum);
            // 触发地图块刷新机制
            EventManager.EventTrigger(EventName.OnMorning);
        }
        timeData.calcTime = timeConfig.timeStateConfig[timeData.stateIndex].durationTime;
        // 迷雾效果: 直接使用配置中fog的值
        RenderSettings.fog = timeConfig.timeStateConfig[timeData.stateIndex].fog; 
        // 背景音乐:
        if (timeConfig.timeStateConfig[timeData.stateIndex].bgAudioClip != null) {
            StartCoroutine(ChangeBGAudio(timeConfig.timeStateConfig[timeData.stateIndex].bgAudioClip));
        }
        }

    private void SetLight(Quaternion rotation, Color color, float intensity) {
        // 设置阳光强度/角度/颜色
        mainLight.transform.rotation = rotation;
        mainLight.color = color;
        // 设置强度
        mainLight.intensity = intensity;
        // 设置环境光亮度
        RenderSettings.ambientIntensity = intensity;
    }

    // 使用协程去切换背景音乐
    private IEnumerator ChangeBGAudio(AudioClip audioClip) {
        float old_volume = AudioManager.Instance.BGVolume;
        // 音量<=0则不需要播放声音
        if (old_volume <= 0) {
            yield break;
        }
        // 降低之前曲子的音量
        float current_volume = old_volume;
        while (current_volume > 0) {
            yield return null;
            current_volume -= Time.deltaTime / 2;
            AudioManager.Instance.BGVolume = current_volume;
        }
        AudioManager.Instance.PlayBGAudio(audioClip);
        // 恢复到之前的音量
        while (current_volume < old_volume) {
            yield return null;
            current_volume += Time.deltaTime / 2;
            AudioManager.Instance.BGVolume = current_volume;
        }
        AudioManager.Instance.BGVolume = old_volume;
    }

    // 当游戏关闭时将时间数据写入磁盘即可
    private void OnDestroy() {
        ArchiveManager.Instance.SaveTimeData();
    }
}