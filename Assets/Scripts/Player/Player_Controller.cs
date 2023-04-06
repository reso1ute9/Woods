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
    public Animator animator;
    public CharacterController characterController;
    private StateMachine stateMachine;
    public Transform playerTransform { get; private set; }

    void Update() {
        // transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 3.5f);
    }

    public void Init() {
        playerTransform = transform;
        stateMachine = ResManager.Load<StateMachine>();
        stateMachine.Init(this);
        // 设置初始状态: 待机
        stateMachine.ChangeState<Player_Idle>((int)PlayerState.Idle);
    }
}
