using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class BuildManager : SingletonMono<BuildManager>
{
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
        // 生成预览物体
        GameObject prefab = ConfigManager.Instance.GetConfig<MapObjectConfig>(ConfigName.MapObject, buildConfig.targetId).prefab;
        if (buildPreviewDict.TryGetValue(prefab.name, out BuildingBase previewBuilding) == false) {
            previewBuilding = GameObject.Instantiate(prefab, transform).GetComponent<BuildingBase>();
            previewBuilding.InitOnPreview();
            buildPreviewDict.Add(prefab.name, previewBuilding);
        }
        // TOOD: 预览物体跟随鼠标, 跟随时物体需要在格子内
        // TODO: 碰撞检测, 如果可以建造, 材质球设置为绿色, 否则为红色
        // TODO: 检查是否进行建造, 确定建造需要判断物品是否满足要求
        yield return null;
    }
}
