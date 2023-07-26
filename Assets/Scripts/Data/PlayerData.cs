using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using System;

[Serializable]
public class PlayerTransformData
{
    // 坐标: sv_postion存档用, position外部调用用
    private Serialization_Vector3 sv_position;
    public Vector3 position { 
        get => sv_position.ConverToVector3(); 
        set => value.ConverToSVector3();
    }

    // 旋转
    private Serialization_Vector3 sv_rotation;
    public Vector3 rotation { 
        get => sv_rotation.ConverToVector3(); 
        set => value.ConverToSVector3();
    }
}
