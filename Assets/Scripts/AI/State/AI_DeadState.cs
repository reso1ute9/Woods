using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;


// AI死亡状态
public class AI_DeadState : AIStateBase
{
    public override void Enter() {
        // 死亡状态进入时关闭碰撞体防止玩家继续攻击
        AI.InputCheckCollider.enabled = false;
        // 播放死亡动画
        AI.PlayAnimation("Dead");
        AI.AddAnimationEvent("DeadOver", DeadOver);
    }

    private void DeadOver() {
        AI.Dead();
    }

    public override void Exit() {
        AI.RemoveAnimationEvent("DeadOver", DeadOver);
        // 死亡状态结束时恢复碰撞体
        AI.InputCheckCollider.enabled = true;
    }
}
