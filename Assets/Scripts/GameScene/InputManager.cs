using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.EventSystems;

// 输入管理器
public class InputManager : SingletonMono<InputManager> {
    private static List<RaycastResult> raycastResults = new List<RaycastResult>(); // 记录鼠标与UI碰撞的结果
    [SerializeField] LayerMask bigMapObjectLayer;           // 大型地图对象层(e.g. 树/石头)
    [SerializeField] LayerMask mouseMapObjectLayer;         // 鼠标可交互的地图对象层: 非建筑物层
    [SerializeField] LayerMask BuildingObjectLayer;         // 鼠标可交互的地图对象层: 建筑物层
    [SerializeField] LayerMask groundLayer;                 // 地面层
    private bool wantCheck = false;                         // 是否需要检测

    public void Init() {
        SetCheckState(true);
    }

    public void Update() {
        if (GameSceneManager.Instance.IsInitialized == false) return;
        CheckSelectMapObject();
    }

    public void SetCheckState(bool wantCheck) {
        this.wantCheck = wantCheck;
    }

    // 检查鼠标选中地图对象是否可以进行互动
    private void CheckSelectMapObject() {
        if (wantCheck == false) return;
        // 如果鼠标一直按下
        bool mouseButton = Input.GetMouseButton(0);
        bool mouseButtonDown = Input.GetMouseButtonDown(0);
        if (mouseButton || mouseButtonDown) {
            if (CheckMouseOnUI()) return;
            // 射线检测地图上的3d物体
            RaycastHit hitInfo;
            Ray ray = Camera_Controller.Instance.Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, 100, mouseMapObjectLayer)) {
                // 发送给玩家处理
                Player_Controller.Instance.OnSelectMapObject(hitInfo, mouseButtonDown);
            }
            // 特殊处理: 建筑物点击逻辑
            if (mouseButtonDown && Physics.Raycast(ray, out hitInfo, 100, BuildingObjectLayer)) {
                BuildingBase building = hitInfo.collider.GetComponent<BuildingBase>();
                // 检查当前玩家和箱子的距离是否符合要求
                if (building.TouchDistance > 0) {
                    if (Vector3.Distance(Player_Controller.Instance.playerTransform.position, building.transform.position) < building.TouchDistance) {
                        building.OnSelect();
                    } else {
                        UIManager.Instance.AddTips("距离建筑物太远");
                        ProjectTool.PlayerAudio(AudioType.Fail);
                    }
                }
            }
        }
    }   

    // 检查鼠标是否在UI上
    public bool CheckMouseOnUI() {
        PointerEventData pointEventData = new PointerEventData(EventSystem.current);
        pointEventData.position = Input.mousePosition;
        // 使用射线去检测是否放到了地面上
        EventSystem.current.RaycastAll(pointEventData, raycastResults);
        for (int i = 0; i < raycastResults.Count; i++) {
            RaycastResult raycastResult = raycastResults[i];
            // 如果是UI但是不是Mask模块则返回
            if (raycastResult.gameObject.name != "Mask" && 
                raycastResult.gameObject.TryGetComponent<RectTransform>(out var _temp)) {
                raycastResults.Clear();
                return true;
            }
        }
        raycastResults.Clear();
        return false;
    }

    // 检查鼠标是否在大型地图对象上
    public bool CheckMouseOnBigMapObject() {
        return Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 1000, bigMapObjectLayer);
    }

    // 获取鼠标在地面上的世界坐标
    public bool GetMousePositionOnGround(Vector3 mousePosition, out Vector3 worldPosition) {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out RaycastHit hitInfo, 1000, groundLayer)) {
            worldPosition = hitInfo.point;
            return true;
        }
        worldPosition = Vector3.zero;
        return false;
    }

    // 检查当停止拖拽格子上的物品时是否点击到了建筑物身上
    public bool CheckSlotEndDragOnBuilding(int itemId) {
        // 射线检测地图上的3d物体
        RaycastHit hitInfo;
        Ray ray = Camera_Controller.Instance.Camera.ScreenPointToRay(Input.mousePosition);
        // 特殊处理: 建筑物点击逻辑
        if (Physics.Raycast(ray, out hitInfo, 100, BuildingObjectLayer)) {
            BuildingBase building = hitInfo.collider.GetComponent<BuildingBase>();
            return building.OnSlotEndDragSelect(itemId);
        }
        return false;
    }
}
