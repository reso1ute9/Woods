using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;
using Sirenix.OdinInspector;

// 储物箱UI窗口
[UIElement(true, "UI/UI_StorageBoxInventoryWindow", 1)]
public class UI_StorageBoxInventoryWindow : UI_InventoryWindowBase
{
    [SerializeField] Button closButton;
    [SerializeField] Transform itemParent;
    private StorageBox_Controller storageBox;

    public override void Init() {
        base.Init();
        slots = new List<UI_ItemSlot>(20);
        closButton.onClick.AddListener(Close);
    }

    // 初始化数据
    public void Init(StorageBox_Controller storageBox, InventoryData inventoryData, Vector2Int windowSize) {
        this.storageBox = storageBox;
        this.inventoryData = inventoryData;
        // 设置窗口尺寸
        SetWindowSize(windowSize);
        // 生成窗口格子
        for (int i = 0; i < inventoryData.itemDatas.Length; i++) {
            // 实例化格子
            UI_ItemSlot itemSlot = ResManager.Load<UI_ItemSlot>("UI/UI_ItemSlot", itemParent);
            // 初始化每个格子中物品数据及对应的各种UI信息
            itemSlot.Init(i, this);
            itemSlot.InitData(inventoryData.itemDatas[i]);
            slots.Add(itemSlot);
        }
    }

    // 设置窗口大小
    private void SetWindowSize(Vector2Int windowSize) {
        // 宽度 = 两边15 + 中间格子区域
        // 高度 = 顶部50 + 中间格子区域 + 底部15
        // 格子大小 = 100 * 100
        RectTransform rectTransform = transform as RectTransform;
        rectTransform.sizeDelta = new Vector2(15 * 2 + 100 * windowSize.x, 50 + 15 + 100 * windowSize.y);
    }

    // 储物箱关闭逻辑
    public override void OnClose() {
        base.OnClose();
        // 清空额外数据并将格子数据放入对象池
        for (int i = 0; i < slots.Count; i++) {
            slots[i].JKGameObjectPushPool();
        }
        slots.Clear();
        inventoryData = null;
    }
}
