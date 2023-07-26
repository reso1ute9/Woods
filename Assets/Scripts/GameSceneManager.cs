using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;


// 游戏场景管理器
public class GameSceneManager : LogicManagerBase<GameSceneManager>
{
    public bool IsInitialized { get; private set; }

    protected override void CancelEventListener() {}
    protected override void RegisterEventListener() {}

    private void Start() {
        UIManager.Instance.CloseAll();
        StartGame();
    }

    private void StartGame() {
        // 如果运行到这里一定所有存档都准备好了
        IsInitialized = false;
        // TODO: 加载进度条

        // 确定地图初始化配置数据
        MapConfig mapConfig = ConfigManager.Instance.GetConfig<MapConfig>(ConfigName.Map);
        float mapSizeOnWorld = ArchiveManager.Instance.mapInitData.mapSize * mapConfig.mapChunkSize * mapConfig.cellSize;
        // 初始化角色
        Player_Controller.Instance.Init(mapSizeOnWorld);
        // 初始化相机
        Camera_Controller.Instance.Init(mapSizeOnWorld);
        // 初始化地图+更新观察者位置
        MapManager.Instance.Init();
        MapManager.Instance.UpdateView(Player_Controller.Instance.playerTransform);
    }
}
