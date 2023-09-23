using UnityEngine;
using JKFrame;

public class Spider_PursueState : AI_PursueState 
{
    private Spider_Controller spider;

    public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine) {
        base.Init(owner, stateType, stateMachine);
        spider = owner as Spider_Controller;
    }

    public override void Enter() {
        AI.NavMeshAgent.enabled = true;
        AI.PlayAnimation("Run");
        // 添加脚步声事件
        AI.AddAnimationEvent("FootStep", FootStep);
        // 修改移动速度
        AI.NavMeshAgent.speed = spider.RunSpeed;
    }
    
    public override void Update() {
        if (GameSceneManager.Instance.IsGameOver) {
            return;
        }
        float distance = Vector3.Distance(AI.transform.position, Player_Controller.Instance.playerTransform.position);
        if (distance <= AI.Radius + AI.AttackDistance) {
            AI.ChangeState(AIState.Attack);
        } else {
            AI.SavePosition();
            AI.NavMeshAgent.SetDestination(Player_Controller.Instance.playerTransform.position);
            // 如果当前AI物体
            if (distance >= spider.RetreatDistance) {
                AI.ChangeState(AIState.Idle);
                return;
            }
            // 检测AI归属地图块
            CheckAndTransferMapChunk();
        }
    }
    
    public override void Exit() {
        base.Exit();
        // 恢复移动速度
        AI.NavMeshAgent.speed = spider.WalkSpeed;
    }
}
