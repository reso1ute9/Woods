using UnityEngine;
using UnityEngine.UI;
using JKFrame;

[UIElement(false, "UI/UI_GameLoadingWindow", 4)]
public class UI_GameLoadingWindow : UI_WindowBase
{
    [SerializeField]
    private Text progress_Text;
    [SerializeField]
    private Image fill_Image;
    public override void OnShow() {
        base.OnShow();
        UpdateProgress(0);
    }

    public void UpdateProgress(float progressValue) {
        progress_Text.text = (int)(progressValue) + "%";
        fill_Image.fillAmount = (int)(progressValue);
    }
}