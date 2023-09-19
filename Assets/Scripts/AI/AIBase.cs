using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.AI;

public abstract class AIBase : MonoBehaviour, IStateMachineOwner
{
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent navMeshAgent;
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; }
    [SerializeField] MapVertexType mapVertexType;               // AI物体活动的地图类型
    private AIState currentAIState;                             // 当前动画状态
    protected MapChunkController mapChunk;                      // 当前所在的地图块控制器
    private MapObjectData aiData;                             // 当前地图物品id
    public MapObjectData AIData { get => aiData; }
    protected StateMachine stateMachine;
    public StateMachine StateMachine { 
        get {
            if (stateMachine == null) {
                stateMachine = PoolManager.Instance.GetObject<StateMachine>();
                StateMachine.Init(this);
            }
            return stateMachine;
        } 
    }

    public virtual void Init(MapChunkController mapChunk, MapObjectData aiData) {
        this.mapChunk = mapChunk;
        this.aiData = aiData;
        transform.position = aiData.position;
        ChangeState(AIState.Idle);
    }

    public virtual void ChangeState(AIState aiState) {
        currentAIState = aiState;
        switch (aiState) {
            case AIState.Idle:
                StateMachine.ChangeState<AI_Idle>((int)aiState);
                break;
            case AIState.Patrol:
                StateMachine.ChangeState<AI_Patrol>((int)aiState);
                break;
            case AIState.Hurt:
                break;
            case AIState.Pursue:
                break;
            case AIState.Attack:
                break;
            case AIState.Dead:
                break;
            default:
                break;
        }
    }

    // 播放动画
    public void PlayAnimation(string animationName, float fixedTime = 0.25f) {
        animator.CrossFadeInFixedTime(animationName, fixedTime);
    }

    // 获取AI可以到达的随机坐标
    public Vector3 GetAIRandomPotion() {
        return mapChunk.GetAIRandomPosition(mapVertexType);
    }

    // 保存坐标
    public void SavePosition() {
        aiData.position = transform.position;
    }

    public virtual void RemoveOnMap() {
        // 通知地图块控制器移除当前AI对象
    }

    public void Destroy() {
        this.JKGameObjectPushPool();
        currentAIState = AIState.None;
        stateMachine.Stop();
    }
}
