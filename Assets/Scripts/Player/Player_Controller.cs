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
    [SerializeField] AudioClip[] foodstepAudioClips;                // 脚步音效

    private StateMachine stateMachine;
    public Transform playerTransform { get; private set; }
    // 玩家转向速度
    public float rotateSpeed { get; private set; } = 10;
    public float moveSpeed { get; private set; } = 10;
    public Vector2 positionXScope { get; private set; }               // 相机能移动的X轴范围
    public Vector2 positionZScope { get; private set; }               // 相机能移动的Y轴范围


    private void Start() {
        Init();
    }

    private void Update() {
        // transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 3.5f);
    }

    public void Init() {
        playerModel.Init(PlayAudioOnFootstep);
        playerTransform = transform;
        stateMachine = ResManager.Load<StateMachine>();
        stateMachine.Init(this);
        // 设置初始状态: 待机
        stateMachine.ChangeState<Player_Idle>((int)PlayerState.Idle);
        InitPositionScope(MapManager.Instance.mapSizeOnWorld);
    }

    // 传入游戏内3D地图大小初始化相机移动范围, 需要注意由于有Y轴高度, 所以相机移动
    // 范围需要适当的缩小, 可通过提前在scene中测量得到合适的值
    private void InitPositionScope(float mapSizeOnWorld) {
        positionXScope = new Vector2(1, mapSizeOnWorld - 1);
        positionZScope = new Vector2(1, mapSizeOnWorld - 1);
    }

    private void PlayAudioOnFootstep(int index) {
        AudioManager.Instance.PlayOnShot(foodstepAudioClips[index], playerTransform.position, 0.5f);
    }
}
