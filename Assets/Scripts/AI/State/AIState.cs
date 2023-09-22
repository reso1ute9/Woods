using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 包含所有AI物体的所有状态
public enum AIState
{
    None,
    Idle,
    Patrol,
    Hurt,
    Pursue,
    Attack,
    Dead
}