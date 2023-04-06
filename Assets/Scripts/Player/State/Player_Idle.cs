using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 玩家待机状态
public class Player_Idle : PlayerStateBase
{
    public override void Enter() {
        base.Enter();
        // this.PlayAnimation("Idle");
    }

    public override void Exit() {
        base.Exit();
    }

    public override void Update() {
        base.Update();
    }
}
