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
        // 添加脚步声事件
        AI.AddAnimationEvent("FootStep", FootStep);
        AI.NavMeshAgent.SetDestination(target);
    }

    public override void Update() {
        AI.SavePosition();
        // 检测是否到达目标
        if (Vector3.Distance(AI.transform.position, target) < 0.5f) {
            AI.ChangeState(AIState.Idle);
        }
    }
    
    public void FootStep() {
        int index = Random.Range(1, 3);
        AI.PlayAudio("FootStep" + index.ToString(), 0.15f);
    }

    public override void Exit() {
        // AI.SavePosition();
        AI.NavMeshAgent.enabled = false;
        // 移除脚步声事件
        AI.RemoveAnimationEvent("FoodStep", FootStep);
    }
}
