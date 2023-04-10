using System.Collections;
using System.Collections.Generic;
using JKFrame;
using UnityEngine;

// 玩家移动状态
public class Player_Move : PlayerStateBase
{
    private CharacterController characterController;
    public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine)
    {
        base.Init(owner, stateType, stateMachine);
        characterController = player.characterController;
    }

    public override void Enter() {
        this.PlayAnimation("Move");
    }

    public override void Update() {
        base.Update(); 
    }
}
