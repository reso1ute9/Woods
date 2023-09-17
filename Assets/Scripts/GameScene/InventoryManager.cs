using System.Collections;
using System.Collections.Generic;
using JKFrame;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : SingletonMono<InventoryManager>
{
    private UI_MainInventoryWindow mainInventoryWindow;     // 快捷栏窗口

    public void Init() {
        // 初始化快捷栏窗口
        mainInventoryWindow = UIManager.Instance.Show<UI_MainInventoryWindow>();
        mainInventoryWindow.InitData();
    }

    #region 快捷窗口栏
    // 获取某个物品的数量
    public int GetItemCount(int configId) {
        return mainInventoryWindow.GetItemCount(configId);
    }

    // 添加物品并播放音效
    public bool AddMainInventoryWindowItemAndPlayAudio(int configId) {
        return mainInventoryWindow.AddItemAndPlayAudio(configId);
    }

    // 使用合成/建造功能时更新物品数量
    public void UpdateItemsForBuild(BuildConfig buildConfig) {
        mainInventoryWindow.UpdateItemsForBuild(buildConfig);
    }

    // 逻辑层面添加物品
    public bool AddMainInventoryWindowItem(int configId) {
        return mainInventoryWindow.AddItem(configId);
    }
    #endregion

    #region 储物箱
    public void OpenStorageBoxWindow(StorageBox_Controller storageBox, InventoryData inventoryData, Vector2Int windowSize) {
        // 先关闭窗口, 关闭窗口时会调用OnClose处理关闭逻辑
        ProjectTool.PlayerAudio(AudioType.Bag);
        UIManager.Instance.Close<UI_StorageBoxInventoryWindow>();
        UIManager.Instance.Show<UI_StorageBoxInventoryWindow>().Init(storageBox, inventoryData, windowSize); 
    }
    #endregion

    private void OnDestroy() {
        // 存储当前物品快捷栏数据信息
        ArchiveManager.Instance.SaveMainInventoryData();
    }
}
