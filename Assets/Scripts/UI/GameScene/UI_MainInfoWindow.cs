using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;


[UIElement(false, "UI/UI_MainInfoWindow", 0)]
public class UI_MainInfoWindow : UI_WindowBase
{
    [SerializeField] public Image timeStateImg;
    [SerializeField] public Sprite[] timeStateSprite;
    [SerializeField] public Text dayNumText;
    [SerializeField] public Image HPImg;
    [SerializeField] public Image HungryImg;

    public override void Init() {
        playerConfig = ConfigManager.Instance.GetConfig<PlayerConfig>(ConfigName.Player);
    }

    // 注册事件监听
    protected override void RegisterEventListener() {
        base.RegisterEventListener();
        // 监听时间状态类事件
        EventManager.AddEventListener<bool>(EventName.UpdateTimeState, UpdateTimeState);
        EventManager.AddEventListener<int>(EventName.UpdateDayNum, UpdateDayNum);
        // 监听角色事件
        EventManager.AddEventListener<float>(EventName.UpdatePlayerHP, UpdatePlayerHP);
        EventManager.AddEventListener<float>(EventName.UpdatePlayerHungry, UpdatePlayerHungry);
    }

    // 取消事件监听
    protected override void CancelEventListener() {
        base.CancelEventListener();
        // 取消时间类事件监听
        EventManager.RemoveEventListener<bool>(EventName.UpdateTimeState, UpdateTimeState);
        EventManager.RemoveEventListener<int>(EventName.UpdateDayNum, UpdateDayNum);
        // 取消角色事件监听
        EventManager.RemoveEventListener<float>(EventName.UpdatePlayerHP, UpdatePlayerHP);
        EventManager.RemoveEventListener<float>(EventName.UpdatePlayerHungry, UpdatePlayerHungry);
    }

    #region 时间状态类UI更新逻辑
    // 更新时间状态UI(太阳/月亮)
    private void UpdateTimeState(bool isSun) {
        timeStateImg.sprite = timeStateSprite[isSun ? 0 : 1];
    }

    // 更新天数显示UI
    private void UpdateDayNum(int dayNum) {
        dayNumText.text = "Day " + dayNum.ToString();
    }
    #endregion
    
    #region 角色相关UI更新逻辑
    private PlayerConfig playerConfig;

    private void UpdatePlayerHP(float hp) {
        HPImg.fillAmount = hp / playerConfig.maxHP;
    }

    private void UpdatePlayerHungry(float hungry) {
        HungryImg.fillAmount = hungry / playerConfig.maxHungry;
    }
    #endregion
}
