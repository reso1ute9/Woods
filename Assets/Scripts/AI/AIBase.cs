using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using System;

// AI基类
public abstract class AIBase : SerializedMonoBehaviour, IStateMachineOwner
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent navMeshAgent;
    [SerializeField] protected Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; }
    protected float hp;
    [SerializeField] public float maxHP;
    [SerializeField] protected MapVertexType mapVertexType;               // AI物体活动的地图类型
    protected AIState currentAIState;                             // 当前动画状态
    protected MapChunkController mapChunk;                      // 当前所在的地图块控制器
    protected MapObjectData aiData;                               // 当前地图物品id
    public MapObjectData AIData { get => aiData; }
    [SerializeField] protected float radius;             // 交互半径
    public float Radius { get => radius; }

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
        this.hp = maxHP;
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
                StateMachine.ChangeState<AI_Hurt>((int)aiState);
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

    public virtual void Hurt(float damage) {
        UnityEngine.Debug.Log("damage:" + damage);
        if (hp <= 0) {
            return;
        }
        hp -= damage;
        UnityEngine.Debug.Log("hp:" + hp);
        if (hp <= 0) {
            // TODO: 死亡逻辑
            ChangeState(AIState.Dead);
        } else {
            ChangeState(AIState.Hurt);
        }
    }

    // 播放动画
    public void PlayAnimation(string animationName, float fixedTime = 0.25f) {
        animator.CrossFadeInFixedTime(animationName, fixedTime);
    }

    // 播放音效
    public void PlayAudio(string audioName, float volumeScale = 1) {
        if (audioClipDict.TryGetValue(audioName, out AudioClip audioClip)) {
            AudioManager.Instance.PlayOnShot(audioClip, transform.position, volumeScale);
        }
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

    #region 动画事件
    private Dictionary<string, Action> animationEventDict = new Dictionary<string, Action>(5);
    
    private void AnimationEvent(string eventName) {
        if (animationEventDict.TryGetValue(eventName, out Action action)) {
            action?.Invoke();
        }
    }

    // 添加动画事件
    public void AddAnimationEvent(string eventName, Action action) {
        if (animationEventDict.TryGetValue(eventName, out Action _action)) {
            action += _action;
        } else {
            animationEventDict.Add(eventName, action);
        }
    }

    // 移除动画事件
    public void RemoveAnimationEvent(string eventName, Action action) {
        if (animationEventDict.TryGetValue(eventName, out Action _action)) {
            _action -= action;
        }
    }

    // 移除动画事件
    public void RemoveAnimationEvent(string eventName) {
        animationEventDict.Remove(eventName);
    }

    // 删除所有动画事件
    public void CleanAllAnimationEvent() {
        animationEventDict.Clear();
    }
    #endregion
}
