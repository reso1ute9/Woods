using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;


// 项目中常用的通用工具
public static class ProjectTool
{
    // 基于指定音效类型播放音效
    public static void PlayerAudio(AudioType audioType) {
        AudioClip audioClip = ConfigManager.Instance.GetConfig<PlayerConfig>(ConfigName.Player).AudioClipDict[audioType];
        AudioManager.Instance.PlayOnShot(audioClip, Vector3.zero, 1.0f, false);
    }
}
