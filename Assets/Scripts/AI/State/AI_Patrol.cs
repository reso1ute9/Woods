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
        // 检查是否进入警戒范围
        if (AI.HostileDistance > 0 && GameSceneManager.Instance.IsGameOver == false) {
            // 判断敌对距离
            if (Vector3.Distance(AI.transform.position, Player_Controller.Instance.playerTransform.position) <
                AI.HostileDistance) {
                // 进入追击状态
                AI.ChangeState(AIState.Pursue);
                return;
            }
        }
        // 检测是否到达目标
        if (Vector3.Distance(AI.transform.position, target) < 0.5f) {
            AI.ChangeState(AIState.Idle);
        }
    }
    
    private void FootStep() {
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
