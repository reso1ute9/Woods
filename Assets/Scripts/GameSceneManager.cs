using System;
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
    
    private bool isGameOver = false;
    public bool IsGameOver { get => isGameOver; }
    private bool isPause = false;
    public bool IsInitialized { get; private set; }
    protected override void CancelEventListener() {}
    protected override void RegisterEventListener() {}

    private void Start() {
        #region 测试逻辑
        if (isTest) {
            if (isCreatNewArchive) {
                ArchiveManager.Instance.CreateNewArchive(10, 1, 1, 0.6f);
            } else {
                ArchiveManager.Instance.LoadCurrentArchive();
            }
        }
        #endregion 

        UIManager.Instance.CloseAll();
        StartGame();
    }

    private void Update() {
        if (IsInitialized == false) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            isPause = !isPause;
            if (isPause) {
                PauseGame();
            } else {
                UnPauseGame();
            }
        }
    }

    private void PauseGame() {
        isPause = true;
        UIManager.Instance.Show<UI_PauseWindow>();
        TimeManager.Instance.timeScale = 0;
    }

    public void UnPauseGame() {
        isPause = false;
        UIManager.Instance.Close<UI_PauseWindow>();
        TimeManager.Instance.timeScale = 1;
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
        
        // 显示游戏页面中昼夜/血量/饥饿值显示UI
        UIManager.Instance.Show<UI_MainInfoWindow>();
        
        // 初始化时间管理器
        TimeManager.Instance.Init();
        
        // 初始化角色
        Player_Controller.Instance.Init(mapSizeOnWorld);
        // 初始化相机
        Camera_Controller.Instance.Init(mapSizeOnWorld);
        
        // 初始化地图管理器
        MapManager.Instance.UpdateView(Player_Controller.Instance.transform);
        MapManager.Instance.Init();
        
        // 初始化背包管理器
        InventoryManager.Instance.Init();
        
        // 初始化输入管理器
        InputManager.Instance.Init();
        
        // 初始化建造管理器
        BuildManager.Instance.Init();
        
        // 初始化科技管理器
        ScienceManager.Instance.Init();
    }    

    #region  加载进度
    private UI_GameLoadingWindow loadingWindow;

    // 初始化地图块数量所需要的当前值和最大值
    private int currentMapChunkCount = 0;
    private int maxMapChunkCount = 0;    

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

    public void SetProgressMapChunkCount(int max) {
        maxMapChunkCount = max;
    }

    public void OnGenerateMapChunkSucceed() {
        currentMapChunkCount += 1;
        UpdateMapProgress(currentMapChunkCount, maxMapChunkCount);
    }
    #endregion

    #region 存档管理

    public void EnterMenuScene() {
        // 恢复时间
        TimeManager.Instance.timeScale = 1;
        // 回收场景资源
        MapManager.Instance.OnCloseGameScene();
        // 清空所有事件
        EventManager.Clear();
        // 停止所有协程
        MonoManager.Instance.StopAllCoroutines();
        // 关闭所有UI
        UIManager.Instance.CloseAll();
        // 进入新场景
        GameManager.Instance.EnterMenu();
    }
    
    // 游戏结束
    public void GameOver() {
        isGameOver = true;
        // 删除存档
        ArchiveManager.Instance.CleanArchive();
        // 延迟进入菜单场景
        Invoke(nameof(EnterMenuScene), 2.0f);
    }
    
    // 关闭游戏场景并保存存档后进入菜单场景
    public void CloseAndSave() {
        // 存档
        EventManager.EventTrigger(EventName.SaveGame);
        // 进入菜单场景
        EnterMenuScene();
    }
    
    // 意外监听
    private void OnApplicationQuit() {
        // 当玩家死亡需要紧急存档
        if (isGameOver) {
            EventManager.EventTrigger(EventName.SaveGame);
        }
    }
    #endregion
}
