using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;


public enum CursorState {
    Normal = 0,
    Handle = 1
}

public class GameManager : SingletonMono<GameManager>
{
    private void Start() {
        Init();
    }

    // 整个游戏执行时首先执行逻辑
    private void  Init() {
        // 设置默认鼠标指针
        SetCursorState(CursorState.Normal);
        // 加载存档数据
    }

    #region 鼠标指针
    [SerializeField] Texture2D[] cursorTextures;
    private CursorState currentCursorState = CursorState.Normal;

    public void SetCursorState(CursorState cursorState) {
        if (currentCursorState == cursorState) {
            return;
        }
        currentCursorState = cursorState;
        Texture2D tex = cursorTextures[(int) cursorState];
        Cursor.SetCursor(tex, Vector2.zero, CursorMode.Auto);
    }
    #endregion
    
    // 跨场景
    #region 跨场景
    // 基于新存档进行游戏
    public void CreateNewArchive_EnterGame(int mapSize, int mapSeed, int spawnSeed, float marshLimit) {
        // 初始化新存档
        ArchiveManager.Instance.CreateNewArchive(mapSize, mapSeed, spawnSeed, marshLimit);
        // 加载场景
        SceneManager.LoadScene("GameScene");
    }

    // 使用当前存档进行游戏
    public void UseCurrentArchive_EnterGame() {
        ArchiveManager.Instance.LoadCurrentArchive();
        // 加载场景
        SceneManager.LoadScene("GameScene");
    }
    
    // 进入菜单场景
    public void EnterMenu() {
        SceneManager.LoadScene("MenuScene");
    }
    #endregion
}