using UnityEngine;

public class Spider_Controller : AIBase {
    [SerializeField] private float walkSpeed = 3.0f;        // 移动速度
    [SerializeField] private float runSpeed = 4.0f;         // 奔跑速度
    [SerializeField] private float retreatDistance = 3.0f;  // 撤退距离

    public float RunSpeed { get => runSpeed; }          
    public float WalkSpeed { get => walkSpeed; }
    public float RetreatDistance { get => retreatDistance; }
    
    
    public override void ChangeState(AIState aiState) {
        currentAIState = aiState;
        switch (aiState) {
            case AIState.Idle:
                StateMachine.ChangeState<AI_Idle>((int)aiState);
                break;
            case AIState.Patrol:
                StateMachine.ChangeState<AI_Patrol>((int)aiState);
                break;
            case AIState.Hurt:
                StateMachine.ChangeState<AI_Hurt>((int)aiState, true);
                break;
            case AIState.Pursue:
                StateMachine.ChangeState<Spider_PursueState>((int)aiState);
                break;
            case AIState.Attack:
                StateMachine.ChangeState<AI_AttackState>((int)aiState);
                break;
            case AIState.Dead:
                StateMachine.ChangeState<AI_DeadState>((int)aiState);
                break;
        }
    }
}