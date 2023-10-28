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
    #region 组件
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent navMeshAgent;
    [SerializeField] protected Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; }
    [SerializeField] protected Collider inputCheckCollider;               // 输入检测碰撞体
    public Collider InputCheckCollider { get => inputCheckCollider; }
    [SerializeField] protected Transform weapon;
    public Transform Weapon { get => weapon; }
    
    protected StateMachine stateMachine;
    public StateMachine StateMachine {
        get {
            if (stateMachine == null)
            {
                stateMachine = PoolManager.Instance.GetObject<StateMachine>();  
                StateMachine.Init(this);
            }
            return stateMachine;
        }
    }
    #endregion
    
    #region 重要参数
    private float hp;
    [SerializeField] public float maxHP;
    [SerializeField] protected float attackDistance = 0.5f;
    public float AttackDistance { get => attackDistance; }
    [SerializeField] protected float attackValue = 10.0f;
    public float AttackValue { get => attackValue; }
    [SerializeField] protected MapVertexType mapVertexType;                 // AI物体活动的地图类型
    [SerializeField] protected float radius;                                // 交互半径
    public float Radius { get => radius; }
    [SerializeField] private int lootObjectConfigId = -1;
    [SerializeField] private float hostileDistance = -1;                    // 敌对距离, -1代表无效
    public float HostileDistance { get => hostileDistance; }
    #endregion
    
    #region 数据
    protected AIState currentAIState;                           // 当前动画状态
    private MapChunkController mapChunk;                      // 当前所在的地图块控制器
    public MapChunkController MapChunk { get => mapChunk; }
    private MapObjectData aiData;                             // 当前地图物品id
    public MapObjectData AIData { get => aiData;}
    #endregion

    public virtual void Init(MapChunkController mapChunk, MapObjectData aiData) {
        this.mapChunk = mapChunk;
        this.aiData = aiData;
        this.hp = maxHP;
        transform.position = aiData.position;
        ChangeState(AIState.Idle);
    }

    // 迁移时的初始化方法
    public virtual void InitOnTransfer(MapChunkController mapChunk) {
        this.mapChunk = mapChunk;
    }
    
    // 切换状态
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
                StateMachine.ChangeState<AI_PursueState>((int)aiState);
                break;
            case AIState.Attack:
                StateMachine.ChangeState<AI_AttackState>((int)aiState);
                break;
            case AIState.Dead:
                StateMachine.ChangeState<AI_DeadState>((int)aiState);
                break;
        }
    }
    
    // AI物体受伤方法
    public virtual void Hurt(float damage) {
        if (hp <= 0) {
            return;
        }
        hp -= damage;
        if (hp <= 0) {
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

    // 仅考虑自身游戏物体的销毁, 不考虑存档层面的问题
    public void Destroy() {
        this.JKGameObjectPushPool();
        currentAIState = AIState.None;
        stateMachine.Stop();
    }

    // 死亡逻辑: 数据、游戏物体层面都需要销毁
    public void Dead() {
        // 告知地图块移除数据
        mapChunk.RemoveAIObject(AIData.id);
        // 掉落物品
        if (lootObjectConfigId == -1) {
            return;
        }
        LootConfig lootConfig = ConfigManager.Instance.GetConfig<LootConfig>(ConfigName.Loot, lootObjectConfigId);
        if (lootConfig != null) {
            lootConfig.GenerateMapObject(mapChunk, transform.position);
        }
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
