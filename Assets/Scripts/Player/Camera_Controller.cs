using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class Camera_Controller : SingletonMono<Camera_Controller>
{
    private Transform mTransform;
    [SerializeField] private Transform target;    // 相机目标
    [SerializeField] private Vector3 offset;      // 便宜跟随量
    [SerializeField] private float moveSpeed;     // 相机跟随速度

    private Vector2 positionXScope;               // 相机能移动的X轴范围
    private Vector2 positionZScope;               // 相机能移动的Y轴范围

    protected override void Awake() {
        base.Awake();
    }

    // 由于MapManager.Instance是在Awake中进行初始化, 所以需要将camera的初始化
    // 顺序调整到Start中
    private void Start() {
        Init();
    }

    public void Init() {
        mTransform = transform;
        InitPositionScope(MapManager.Instance.mapSizeOnWorld);
    }

    // 传入游戏内3D地图大小初始化相机移动范围, 需要注意由于有Y轴高度, 所以相机移动
    // 范围需要适当的缩小, 可通过提前在scene中测量得到合适的值
    private void InitPositionScope(float mapSizeOnWorld) {
        positionXScope = new Vector2(5, mapSizeOnWorld - 5);
        positionZScope = new Vector2(-1, mapSizeOnWorld - 10);
    }

    // Update是立即触发, LateUpdate是随后触发
    private void LateUpdate() {
        if (target != null) {
            Vector3 targetPosition = target.position + offset;
            targetPosition.x = Mathf.Clamp(targetPosition.x, positionXScope.x, positionXScope.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, positionZScope.x, positionZScope.y);
            mTransform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        }
    }
}
