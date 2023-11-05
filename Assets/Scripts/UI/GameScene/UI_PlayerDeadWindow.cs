using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;

[UIElement(false, "UI/UI_PlayerDeadWindow", 4)]
public class UI_PlayerDeadWindow : UI_WindowBase
{
    [SerializeField] Button quitButton;

    public override void Init() {
        quitButton.onClick.AddListener(QuitButtonClick);
    }

    private void QuitButtonClick()
    {
        GameSceneManager.Instance.EnterMenuScene();
    }
}
