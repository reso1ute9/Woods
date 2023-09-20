using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

// AI追击状态
public class AI_PursueState : AIStateBase
{
    private Vector3 target;         // 巡逻目标点

    public override void Enter() {
        AI.NavMeshAgent.enabled = true;
        AI.PlayAnimation("Move");
        // 添加脚步声事件
        AI.AddAnimationEvent("FootStep", FootStep);
    }

    public override void Update() {
        if (GameSceneManager.Instance.IsGameOver == false) {
            float distance = Vector3.Distance(AI.transform.position, Player_Controller.Instance.playerTransform.position);
            if (distance <= AI.Radius + AI.attackDistance) {
                // TODO: 转化为攻击状态
                UnityEngine.Debug.Log("到达目标附近");
                AI.ChangeState(AIState.Idle);
            } else {
                AI.NavMeshAgent.SetDestination(Player_Controller.Instance.playerTransform.position);
            }
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
