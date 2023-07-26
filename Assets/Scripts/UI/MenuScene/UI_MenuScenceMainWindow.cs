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

        new_Button.BindMouseEffect();
        continue_Button.BindMouseEffect();
        quit_Button.BindMouseEffect();
    }

    public override void OnClose()
    {
        base.OnClose();
        new_Button.RemoveMosueEffect();
        continue_Button.RemoveMosueEffect();
        quit_Button.RemoveMosueEffect();    
    }

    public override void OnShow() {
        // 当前如果不存在存档则不显示游戏按钮
        base.OnShow();
        if (ArchiveManager.Instance.haveArchive == false) {
            continue_Button.gameObject.SetActive(false);
        }
    }

    private void NewGame() {
        // 打开新游戏窗口
        UIManager.Instance.Show<UI_NewGameWindow>();
        Close();
    }

    private void ContinueGame() {
        // 基于之前存档进行游戏
        GameManager.Instance.UseCurrentArchive_EnterGame();
        Close();
    }

    private void QuitGame() {
        // 退出游戏
        Application.Quit();
    }
}
