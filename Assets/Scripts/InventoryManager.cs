using System.Collections;
using System.Collections.Generic;
using JKFrame;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : SingletonMono<InventoryManager>
{
    private UI_InventoryWindow mainInventoryWindow;     // 快捷栏窗口

    public void Init() {
        mainInventoryWindow = UIManager.Instance.Show<UI_InventoryWindow>();
    }

    #region 快捷窗口栏
    // 获取某个物品的数量
    public int GetItemCount(int configId) {
        return mainInventoryWindow.GetItemCount(configId);
    }

    // 添加物品并播放音效
    public bool AddItemAndPlayAudio(int configId) {
        return mainInventoryWindow.AddItemAndPlayAudio(configId);
    }

    // 使用合成/建造功能时更新物品数量
    public void UpdateItemsForBuild(BuildConfig buildConfig) {
        mainInventoryWindow.UpdateItemsForBuild(buildConfig);
    }

    // 逻辑层面添加物品
    public bool AddItem(int configId) {
        return mainInventoryWindow.AddItem(configId);
    }
    #endregion
}
