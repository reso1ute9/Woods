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
    // 设置默认鼠标指针
    private void  Start() {
        SetCursorState(CursorState.Normal);
    }

    #region 鼠标指针
    [SerializeField] Texture2D[] cursorTextures;

    public void SetCursorState(CursorState cursorState) {
        Texture2D tex = cursorTextures[(int) cursorState];
        Cursor.SetCursor(tex, Vector2.zero, CursorMode.Auto);
    }
    #endregion
    
}