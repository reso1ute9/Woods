using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Dead : PlayerStateBase
{
    public override void Enter() {
        // 防止AI在死亡动画演示过程中出现多次攻击导致出现其他问题
        player.CloseCollider.enabled = false;
        player.PlayAnimation("Dead");
    }
}
