using System.Collections;
using UnityEngine;
using JKFrame;
using Unity.VisualScripting;

// AI巡逻逻辑
public class AI_Patrol : AIStateBase
{
    private Vector3 target;         // 巡逻目标点

    public override void Enter() {
        AI.NavMeshAgent.enabled = true;
        target = AI.GetAIRandomPotion();
        AI.PlayAnimation("Move");
        AI.NavMeshAgent.SetDestination(target);
    }

    public override void Update() {
        AI.SavePosition();
        // 检测是否到达目标
        if (Vector3.Distance(AI.transform.position, target) < 0.5f) {
            AI.ChangeState(AIState.Idle);
        }
    }

    public override void Exit() {
        AI.SavePosition();
        AI.NavMeshAgent.enabled = false;
    }
}
