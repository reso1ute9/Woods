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
    public bool canUseItem { get; private set; } = true;              // 玩家当前是否能使用物品
    

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
    

    #region 核心数值
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

    // 恢复生命值
    public void RecoverHP(float value) {
        playerMainData.hp = Mathf.Clamp(playerMainData.hp + value, 0, playerConfig.maxHP);
        TriggerUpdateHPEvent();
    }

    // 恢复饱食度
    public void RecoverHungry(float value) {
        playerMainData.hungry = Mathf.Clamp(playerMainData.hungry + value, 0, playerConfig.maxHungry);
        TriggerUpdateHungryEvent();
    }

    // 触发更新生命值事件, 当生命值发生变动时需要触发更新事件
    private void TriggerUpdateHPEvent() {
        EventManager.EventTrigger(EventName.UpdatePlayerHP, playerMainData.hp);
    }

    // 触发更新饱食度事件, 当饱食度发生变动时需要触发更新事件
    private void TriggerUpdateHungryEvent() {
        EventManager.EventTrigger(EventName.UpdatePlayerHungry, playerMainData.hungry);
    }
    #endregion

    #region 武器相关
    private ItemData currentWeaponItemData;
    private GameObject currentWeaponGameObject;
    // 修改武器: 武器数值、动画、图标等
    public void ChangeWeapon(ItemData newWeapon) {
        // 如果没有切换武器
        if (currentWeaponItemData == newWeapon) {
            return;
        }
        // 旧武器如果有数据, 则需要放回对象池进行回收
        if (currentWeaponItemData != null) {
            currentWeaponGameObject.JKGameObjectPushPool();     // 放进对象池时是基于GameObject.name的, 因此不能重名
        }
        // 新武器如果!=null则需要更新武器模型, 否则角色应该切换为空手状态
        currentWeaponItemData = newWeapon;
        if (newWeapon != null) {
            ItemWeaponInfo itemWeaponInfo = newWeapon.config.itemTypeInfo as ItemWeaponInfo;
            // 设置新武器模型: 武器位置、角度、动画
            currentWeaponGameObject = PoolManager.Instance.GetGameObject(itemWeaponInfo.prefabOnPlayer, playerModel.weaponRoot);
            currentWeaponGameObject.transform.localPosition = itemWeaponInfo.positionOnPlayer;
            currentWeaponGameObject.transform.localRotation = Quaternion.Euler(itemWeaponInfo.rotationOnPlayer);
            animator.runtimeAnimatorController = itemWeaponInfo.animatorController;
            // 需要重新激活一次动画, 动画会出错, 例如在移动中突然切换AnimatorController会不播放动画
            stateMachine.ChangeState<Player_Idle>((int)PlayerState.Idle, true);
        } else {
            animator.runtimeAnimatorController = playerConfig.normalAnimatorController;
            stateMachine.ChangeState<Player_Idle>((int)PlayerState.Idle, true);
        }
    }
    #endregion

    #region 战斗/伐木/采摘
    // 当选择地图对象时
    public void OnSelectMapObject(RaycastHit hitInfo) {
        if (hitInfo.collider.TryGetComponent<MapObjectBase>(out MapObjectBase mapObject)) {
            // TODO: 根据玩家选中的地图对象类型以及当前角色的武器来判断做什么
            print("选中:" + mapObject.name);
        }
    }
    #endregion

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
