using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

// 玩家状态基类, 抽象出所有玩家状态所需要的共同字段/函数
public class PlayerStateBase : StateBase
{
    protected Player_Controller player;
    
    public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine) {
        base.Init(owner, stateType, stateMachine);
        player = owner as Player_Controller;
    }

    // 播放动画
    protected void PlayAnimation(string animationName, float fixedTime = 0.25f) {
        player.animator.CrossFadeInFixedTime(animationName, fixedTime);
    }

    // 修改状态
    protected void ChangeState(PlayerState playerState) {
        switch (playerState) {
            case PlayerState.Idle:
                stateMachine.ChangeState<Player_Idle>(0);
                break;
            case PlayerState.Move:
                stateMachine.ChangeState<Player_Move>(1);
                break;
            case PlayerState.Attack:
                stateMachine.ChangeState<Player_Move>(2);
                break;
            case PlayerState.BeAttack:
                stateMachine.ChangeState<Player_Move>(3);
                break;
            case PlayerState.Dead:
                stateMachine.ChangeState<Player_Move>(4);
                break;
        }
    }
}
