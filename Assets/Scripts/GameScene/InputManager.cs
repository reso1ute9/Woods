using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.EventSystems;

// 输入管理器
public class InputManager : SingletonMono<InputManager> {
    private static List<RaycastResult> raycastResults = new List<RaycastResult>(); // 记录鼠标与UI碰撞的结果
    [SerializeField] LayerMask mapObjectLayer;      // 地图对象层
    [SerializeField] LayerMask groundLayer;         // 地面层
    private bool wantCheck = false;                 // 是否需要检测

    public void Init() {
        SetCheckState(true);
    }

    public void Update() {
        if (GameSceneManager.Instance.IsInitialized == false) return;
        if (wantCheck == false) return;
        CheckSelectMapObject();
    }

    public void SetCheckState(bool wantCheck) {
        this.wantCheck = wantCheck;
    }

    // 检查选中地图对象
    private void CheckSelectMapObject() {
        // 如果鼠标一直按下
        bool mouseButton = Input.GetMouseButton(0);
        bool mouseButtonDown = Input.GetMouseButtonDown(0);
        if (mouseButton || mouseButtonDown) {
            if (CheckMouseOnUI()) return;
            // 射线检测地图上的3d物体
            Ray ray = Camera_Controller.Instance.Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, mapObjectLayer)) {
                // 发送给玩家控制器去处理
                Player_Controller.Instance.OnSelectMapObject(hitInfo, mouseButtonDown);
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

    // 获取鼠标在地面上的世界坐标
    public bool GetMousePositionOnGround(Vector3 mousePosition, out Vector3 worldPosition) {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out RaycastHit hitInfo, 1000, groundLayer)) {
            worldPosition = hitInfo.point;
            return true;
        }
        worldPosition = Vector3.zero;
        return false;
    }
}
