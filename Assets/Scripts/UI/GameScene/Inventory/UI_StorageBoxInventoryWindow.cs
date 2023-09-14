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

    public override void Init() {
        base.Init();
        closButton.onClick.AddListener(OnClose);
    }

    // 初始化数据
    public void InitData(InventoryData inventoryData, Vector2Int windowSize) {
        this.inventoryData = inventoryData;
        // 设置窗口尺寸
        SetWindowSize(windowSize);
        // 生成窗口格子
        if (slots.Length != inventoryData.itemDatas.Length) {
            slots = new UI_ItemSlot[inventoryData.itemDatas.Length];
        }
        for (int i = 0; i < slots.Length; i++) {
            // 实例化格子
            slots[i] = ResManager.Load<UI_ItemSlot>("UI/UI_ItemSlot", itemParent);
            // 初始化每个格子中物品数据及对应的各种UI信息
            slots[i].Init(i, this);
            slots[i].InitData(inventoryData.itemDatas[i]);
        }
    }

    [Sirenix.OdinInspector.Button("SetWindowSize")]
    private void SetWindowSize(Vector2Int windowSize) {
        // 宽度 = 两边15 + 中间格子区域
        // 高度 = 顶部50 + 中间格子区域 + 底部15
        // 格子大小 = 100 * 100
        RectTransform rectTransform = transform as RectTransform;
        rectTransform.sizeDelta = new Vector2(15 * 2 + 100 * windowSize.x, 50 + 15 + 100 * windowSize.y);
    }

    public override void OnClose() {
        base.OnClose();
        // 清空额外数据并将格子数据放入对象池
        inventoryData = null;
        for (int i = 0; i < slots.Length; i++) {
            slots[i].JKGameObjectPushPool();
            slots[i] = null;
        }
    }
}
