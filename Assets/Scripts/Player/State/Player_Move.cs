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
        // 玩家如果有任意移动相关按键就切去move装
        base.Update(); 
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // 1. 检查是否进入待机状态, 如果不移动则进入待机状态
        if (h == 0 && v == 0) {
            this.ChangeState(PlayerState.Idle);
            return;
        }
        // 2. 查看是否进行朝向计算
        Vector3 inputDir = new Vector3(h, 0, v);
        Quaternion targetQua = Quaternion.LookRotation(inputDir);
        
        // player.playerTransform.localRotation = targetQua;
        // 使用插值计算保证转向时动作的流畅性
        player.playerTransform.localRotation = Quaternion.Lerp(
            player.playerTransform.localRotation, 
            targetQua, 
            Time.deltaTime * player.rotateSpeed
        );
        // 3. 移动前查看是否超过边界
        if (player.playerTransform.position.x < player.positionXScope.x && h < 0 || 
            player.playerTransform.position.x > player.positionXScope.y && h > 0) {
            inputDir.x = 0;
        }
        if (player.playerTransform.position.z < player.positionZScope.x && v < 0 || 
            player.playerTransform.position.z > player.positionZScope.y && v > 0) {
            inputDir.z = 0;
        }
        characterController.Move(Time.deltaTime * player.moveSpeed * inputDir);
    }
}
