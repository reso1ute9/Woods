using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public enum PlayerState
{
    Idle,
    Move,
    Attack,
    BeAttack,
    Dead
}


public class Player_Controller : SingletonMono<Player_Controller>, IStateMachineOwner
{
    [SerializeField] public Player_Model playerModel;
    public Animator animator;
    public CharacterController characterController;

    private StateMachine stateMachine;
    private PlayerConfig playerConfig;
    public float rotateSpeed { get => playerConfig.rotateSpeed; }
    public float moveSpeed { get => playerConfig.moveSpeed; }
    public Transform playerTransform { get; private set; }
    public Vector2 positionXScope { get; private set; }               // 相机能移动的X轴范围
    public Vector2 positionZScope { get; private set; }               // 相机能移动的Y轴范围

    

    #region 存档相关数据
    private PlayerTransformData playerTransformData;
    private PlayerMainData playerMainData;
    #endregion

    public void Init(float mapSizeOnWorld) {
        // 确定角色配置
        playerConfig = ConfigManager.Instance.GetConfig<PlayerConfig>(ConfigName.Player);
        
        // 确定存档位置
        playerTransformData = ArchiveManager.Instance.playerTransformData;
        playerMainData = ArchiveManager.Instance.playerMainData;
        
        // 触发角色数据初始化事件改变UI填充比例
        TriggerUpdateHPEvent();
        TriggerUpdateHungryEvent();

        // 初始化音效、位置、状态机
        playerModel.Init(PlayAudioOnFootstep);
        playerTransform = transform;
        stateMachine = ResManager.Load<StateMachine>();
        stateMachine.Init(this);
        
        // 设置初始状态: 待机
        stateMachine.ChangeState<Player_Idle>((int)PlayerState.Idle);
        InitPositionScope(mapSizeOnWorld);
        
        // 初始化角色位置相关数据
        playerTransform.localPosition = playerTransformData.position;
        playerTransform.localRotation = Quaternion.Euler(playerTransformData.rotation);
    }

    private void Update() {
        if (GameSceneManager.Instance.IsInitialized == false) return;
        CalculateHungryOnUpdate();
    }

    // 传入游戏内3D地图大小初始化相机移动范围, 需要注意由于有Y轴高度, 所以相机移动
    // 范围需要适当的缩小, 可通过提前在scene中测量得到合适的值
    private void InitPositionScope(float mapSizeOnWorld) {
        positionXScope = new Vector2(1, mapSizeOnWorld - 1);
        positionZScope = new Vector2(1, mapSizeOnWorld - 1);
    }

    private void PlayAudioOnFootstep(int index) {
        AudioManager.Instance.PlayOnShot(playerConfig.footstepAudioClips[index], playerTransform.position, playerConfig.footstepVolume);
    }

    // 计算当前角色饱食度
    private void CalculateHungryOnUpdate() {
        if (playerMainData.hungry > 0) {
            playerMainData.hungry -= Time.deltaTime * playerConfig.hungryReduceSpeed;
            playerMainData.hungry = playerMainData.hungry > 0 ? playerMainData.hungry : 0;
            TriggerUpdateHungryEvent();
        } else {
            if (playerMainData.hp > 0) {
                playerMainData.hp -= Time.deltaTime * playerConfig.hpReduceSpeedOnHungryIsZero;
                playerMainData.hp = playerMainData.hp > 0 ? playerMainData.hp : 0;
                TriggerUpdateHPEvent();
            } else {
                UIManager.Instance.AddTips("玩家死亡");
            }
        }
    }

    private void TriggerUpdateHPEvent() {
        EventManager.EventTrigger(EventName.UpdatePlayerHP, playerMainData.hp);
    }

    private void TriggerUpdateHungryEvent() {
        EventManager.EventTrigger(EventName.UpdatePlayerHungry, playerMainData.hungry);
    }

    // 场景切换或关闭时将存档数据写入磁盘
    private void OnDestroy() {
        playerTransformData.position = playerTransform.localPosition;
        playerTransformData.rotation = playerTransform.localRotation.eulerAngles;
        ArchiveManager.Instance.SavePlayerTransformData();
        ArchiveManager.Instance.SavePlayerMainData();
        // 存储当前物品栏数据信息
        ArchiveManager.Instance.SaveInventoryData();
    }
}
