using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 玩家攻击状态
public class Player_Attack : PlayerStateBase
{
    public Quaternion attackDirection;

    public override void Enter() {
        // 进入攻击动作时需要确定攻击方向和播放攻击动画
        attackDirection = player.attackDirection;
        PlayAnimation("Attack");
    }

    public override void Update() {
        // 旋转到攻击方向
        player.playerTransform.localRotation = Quaternion.Slerp(
            player.playerTransform.localRotation, 
            attackDirection, 
            Time.deltaTime * player.rotateSpeed * 2.0f
        );
    }
}

