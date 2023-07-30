using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;


// 游戏场景管理器
public class GameSceneManager : LogicManagerBase<GameSceneManager>
{
    #region 测试逻辑
    public bool isTest = true;
    public bool isCreatNewArchive = true;
    #endregion

    public bool IsInitialized { get; private set; }
    protected override void CancelEventListener() {}
    protected override void RegisterEventListener() {}

    private void Start() {
        
        #region 测试逻辑
        if (isTest) {
            if (isCreatNewArchive) {
                ArchiveManager.Instance.CreateNewArchive(10, 1, 1, 0.75f);
            } else {
                ArchiveManager.Instance.LoadCurrentArchive();
            }
        }
        #endregion 

        UIManager.Instance.CloseAll();
        StartGame();
    }

    private void StartGame() {
        // 如果运行到这里一定所有存档都准备好了
        IsInitialized = false;
        // 加载进度条
        loadingWindow = UIManager.Instance.Show<UI_GameLoadingWindow>();
        loadingWindow.UpdateProgress(0);
        // 确定地图初始化配置数据
        MapConfig mapConfig = ConfigManager.Instance.GetConfig<MapConfig>(ConfigName.Map);
        float mapSizeOnWorld = ArchiveManager.Instance.mapInitData.mapSize * mapConfig.mapChunkSize * mapConfig.cellSize;
        // 初始化角色
        Player_Controller.Instance.Init(mapSizeOnWorld);
        // 初始化相机
        Camera_Controller.Instance.Init(mapSizeOnWorld);
        // 初始化地图+更新观察者位置
        MapManager.Instance.UpdateView(Player_Controller.Instance.transform);
        MapManager.Instance.Init();
        // 初始化物品快捷栏
        UIManager.Instance.Show<UI_InventoryWindow>();
    }

    #region  加载进度
    private UI_GameLoadingWindow loadingWindow;

    // 更新地图进度
    public void UpdateMapProgress(int current, int max) {
        float currentProgress = current * 100.0f / max;
        if (currentProgress == 100.0f) {
            loadingWindow.UpdateProgress(100);
            IsInitialized = true;
            loadingWindow.Close();
            loadingWindow = null;
        } else {
            loadingWindow.UpdateProgress(currentProgress);
        }
    }
    #endregion
}
