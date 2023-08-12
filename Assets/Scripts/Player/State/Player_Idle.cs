using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 玩家待机状态
public class Player_Idle : PlayerStateBase
{
    public override void Enter() {
        PlayAnimation("Idle");
    }

    public override void Exit() {
        base.Exit();
    }

    public override void Update() {
        // 玩家有任何移动相关的按键就切换到Move状态
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0) {
            player.ChangeState(PlayerState.Move);
        }
    }
}
