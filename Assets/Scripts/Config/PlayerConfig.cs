using UnityEngine;
using Sirenix.OdinInspector;
using JKFrame;
using System.Collections.Generic;


// 玩家杂七杂八的配置
[CreateAssetMenu(fileName = "角色配置", menuName = "Config/角色配置")]
public class PlayerConfig : ConfigBase
{
    #region 角色属性配置
    [FoldoutGroup("角色配置"), LabelText("玩家转向速度")]
    public float rotateSpeed = 10;              // 玩家转向速度
    [FoldoutGroup("角色配置"), LabelText("玩家移动速度")]
    public float moveSpeed = 4;                 // 玩家移动速度
    [FoldoutGroup("角色配置"), LabelText("玩家最大生命值")]
    public float maxHP = 100;
    [FoldoutGroup("角色配置"), LabelText("玩家最大饱食度")]
    public float maxHungry = 100;
    [FoldoutGroup("角色配置"), LabelText("玩家饱食度衰减速度")]
    public float hungryReduceSpeed = 0.2f;        // 初设为5s掉1滴血
    [FoldoutGroup("角色配置"), LabelText("饱食度为0时玩家生命值衰减速度")]
    public float hpReduceSpeedOnHungryIsZero = 2.0f;
    #endregion

    #region 角色音效资源
    [FoldoutGroup("角色音效资源"), LabelText("脚本音效")]    
    public AudioClip[] footstepAudioClips;                // 脚步音效
    [FoldoutGroup("角色音效资源"), LabelText("脚步音量")]
    public float footstepVolume = 0.5f;                   // 脚步音效音量
    #endregion

    #region 杂项
    [FoldoutGroup("杂项"), LabelText("音效配置")]    
    public Dictionary<AudioType, AudioClip> AudioClipDict; // 音效配置字典
    #endregion
}
