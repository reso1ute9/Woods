using System.Collections;
using UnityEngine;
using JKFrame;
using System.Collections.Generic;

public class BuildManager : SingletonMono<BuildManager>
{
    [SerializeField]
    private float virtualCellSize = 0.25f;
    [SerializeField]
    private LayerMask buildLayerMask;
    private Dictionary<string, BuildingBase> buildPreviewDict = new Dictionary<string, BuildingBase>();     // 预览游戏物体字典

    public void Init() {
        // 初始化建造UI面板
        UIManager.Instance.Show<UI_BuildWindow>();
        // 添加建造事件
        EventManager.AddEventListener<BuildConfig>(EventName.BuildBuilding, BuildBuiding);
    }

    private void BuildBuiding(BuildConfig buildConfig) {
        // 进入建造状态
        StartCoroutine(DoBuildBuilding(buildConfig));
        // 开启鼠标与地图对象交互功能
        // InputManager.Instance.SetCheckState(true);
    }

    IEnumerator DoBuildBuilding(BuildConfig buildConfig) {
        // 关闭鼠标与地图对象交互功能
        InputManager.Instance.SetCheckState(false);
        // 关闭游戏UI交互功能
        UIManager.Instance.DisableUIRaycaster();
        // 生成预览物体
        GameObject prefab = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, buildConfig.targetId).prefab;
        if (buildPreviewDict.TryGetValue(prefab.name, out BuildingBase previewBuilding) == false) {
            previewBuilding = GameObject.Instantiate(prefab, transform).GetComponent<BuildingBase>();
            previewBuilding.InitOnPreview();
            buildPreviewDict.Add(prefab.name, previewBuilding);
        }
        // 开启预览预制体
        previewBuilding.gameObject.SetActive(true);
        // 预览物体跟随鼠标, 跟随时物体需要在格子内
        while (true) {
            // 检查是否取消建造(鼠标右键)
            if (Input.GetMouseButtonDown(1)) {
                // 关闭预览物体
                previewBuilding.gameObject.SetActive(false);
                // 开启鼠标与地图对象交互功能
                InputManager.Instance.SetCheckState(true);
                // 开启游戏UI交互功能
                UIManager.Instance.EnableUIRaycaster();
                yield break;
            }
            if (InputManager.Instance.GetMousePositionOnGround(Input.mousePosition, out Vector3 worldPosition)) {
                // 将鼠标坐标转化为虚拟格子坐标, 需要对格子坐标进行取整
                Vector3 virtualCellPosition = worldPosition;
                virtualCellPosition.x = Mathf.RoundToInt(worldPosition.x / virtualCellSize) * virtualCellSize;
                virtualCellPosition.z = Mathf.RoundToInt(worldPosition.z / virtualCellSize) * virtualCellSize;
                previewBuilding.transform.position = virtualCellPosition;
            }
            // 碰撞检测, 如果可以建造, 材质球设置为绿色, 否则为红色
            bool isOverlap = true;
            if (previewBuilding.Collider is BoxCollider) {
                BoxCollider boxCollider = (BoxCollider)previewBuilding.Collider;
                isOverlap = Physics.CheckBox(
                    boxCollider.transform.position + boxCollider.center, 
                    boxCollider.size / 2, 
                    boxCollider.transform.rotation, 
                    buildLayerMask
                );
            } else if (previewBuilding.Collider is CapsuleCollider) {
                CapsuleCollider capsuleCollider = (CapsuleCollider)previewBuilding.Collider;
                Vector3 colliderCenterPosition = capsuleCollider.transform.position + capsuleCollider.center;
                Vector3 startPosition = colliderCenterPosition;
                Vector3 endPosition = colliderCenterPosition;
                startPosition.y = colliderCenterPosition.y - (capsuleCollider.height / 2) + capsuleCollider.radius;
                endPosition.y = colliderCenterPosition.y + (capsuleCollider.height / 2) - capsuleCollider.radius;                
                isOverlap = Physics.CheckCapsule(
                    startPosition, 
                    endPosition, 
                    capsuleCollider.radius,
                    buildLayerMask
                );
            } else if (previewBuilding.Collider is SphereCollider) {
                SphereCollider sphereCollider = (SphereCollider)previewBuilding.Collider;
                isOverlap = Physics.CheckSphere(
                    sphereCollider.transform.position + sphereCollider.center,
                    sphereCollider.radius,
                    buildLayerMask
                );
            }
            if (isOverlap == true) {
                previewBuilding.SetColorOnPreview(true);
            } else {
                previewBuilding.SetColorOnPreview(false);
                // 建造建筑物并移除对应资源
                if (Input.GetMouseButtonDown(0)) {
                    // 关闭预览物体
                    previewBuilding.gameObject.SetActive(false);
                    // 开启鼠标与地图对象交互功能
                    InputManager.Instance.SetCheckState(true);
                    // 开启游戏UI交互功能
                    UIManager.Instance.EnableUIRaycaster();
                    // 放置建筑物
                    MapManager.Instance.GenerateMapObject(buildConfig.targetId, previewBuilding.transform.position);              
                    // 根据建造配置减少背包中的物品
                    UI_InventoryWindow.Instance.UpdateItemsForBuild(buildConfig);
                    yield break;
                }
            }
            yield return null;
        }   
    }
}
