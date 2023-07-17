using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;

[UIElement(false, "UI/UI_MenuScenceMainWindow", 1)]
public class UI_MenuScenceMainWindow : UI_WindowBase
{
    [SerializeField] Button new_Button;
    [SerializeField] Button continue_Button;
    [SerializeField] Button quit_Button;

    public override void Init()
    {
        new_Button.onClick.AddListener(NewGame);
        continue_Button.onClick.AddListener(ContinueGame);
        quit_Button.onClick.AddListener(QuitGame);
    }

    public override void OnShow() {
        // 当前如果不存在存档则不显示游戏按钮
        base.OnShow();
    }

    private void NewGame() {
        // 打开新游戏窗口
    }

    private void ContinueGame() {
        // 基于之前存档进行游戏
    }

    private void QuitGame() {
        // 退出游戏
        Application.Quit();
    }
}
