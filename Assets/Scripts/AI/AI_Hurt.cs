using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class AI_Hurt : AIStateBase
{
    public override void Enter() {
        // 播放受伤动画
        AI.PlayAnimation("Hurt");
        // 播放受伤音效
        AI.PlayAudio("Hurt", 0.5f);
        // 添加受伤结束的动画事件
        AI.AddAnimationEvent("HurtOver", HurtOver);
        
    }

    // 受伤结束的动画事件
    public void HurtOver() {
        // TODO: 将状态转变为追击
        AI.ChangeState(AIState.Idle);
    }

    // 状态退出逻辑
    public override void Exit() {
        AI.RemoveAnimationEvent("HurtOver", HurtOver);
    }
}