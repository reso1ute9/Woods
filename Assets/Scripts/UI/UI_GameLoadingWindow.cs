using UnityEngine;
using UnityEngine.UI;
using JKFrame;

[UIElement(false, "UI/UI_GameLoadingWindow", 4)]
public class UI_GameLoadingWindow : UI_WindowBase
{
    [SerializeField]
    private Text progressText;
    [SerializeField]
    private Image fillImage;
    public override void OnShow() {
        base.OnShow();
        UpdateProgress(0);
    }

    public void UpdateProgress(float progressValue) {
        progressText.text = (int)(progressValue) + "%";
        fillImage.fillAmount = (int)(progressValue);
    }
}