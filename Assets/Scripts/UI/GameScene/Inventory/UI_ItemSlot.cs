using System;
using System.Security.AccessControl;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JKFrame;


// 物品快捷栏中的格子
[Pool]
public class  UI_ItemSlot : MonoBehaviour
{
    [SerializeField] public Image bgImg;                    // 格子背景图片
    [SerializeField] Image iconImg;                         // 格子里图标
    [SerializeField] Text countText;                        // 格子中显示的数值
    
    public ItemData itemData { get; private set; }          // 格子中的数据
    public int index { get; private set; }                  // 格子编号

    private UI_InventoryWindowBase ownerWindow;             // 宿主窗口: 物品栏/仓库

    private Transform iconTransform;
    private Transform slotTransform;                        // 保存当前格子的父物体

    public static UI_ItemSlot currentMouseEnterSlot;        // 当前鼠标进入/出入的格子
    public static UI_ItemSlot weaponSlot;                   // 记录一下当前的武器栏
    
    private Func<int, AudioType> onUseAction;

    private void Start() {
        iconTransform = iconImg.transform;
        slotTransform = transform;
        // 鼠标交互事件
        this.OnMouseEnter(MouseEnter);
        this.OnMouseExit(MouseExit);
        this.OnBeginDrag(BeginDrag);                  // 开始拖拽
        this.OnDrag(Drag);                            // 拖拽过程中
        this.OnEndDrag(EndDrag);                      // 拖拽结束
        // 设置鼠标进入后放大鼠标指针
        UITool.BindMouseEffect(this);
    }

    private void OnEnable() {
        this.OnUpdate(CheckMouseRightClick);          // 检测鼠标右键是否点击
    }

    private void OnDisable() {
        this.RemoveUpdate(CheckMouseRightClick);        
    }

    // 检测鼠标右键是否可以使用物品
    private void CheckMouseRightClick() {
        if (itemData == null || onUseAction == null) return;
        if (isMouseStay && Input.GetMouseButtonDown(1)) {
            // 根据使用情况播放音效
            AudioType resultAudioType = onUseAction.Invoke(index);
            ProjectTool.PlayerAudio(resultAudioType);
        }
    }

    // 初始化格子
    public void Init(int index, UI_InventoryWindowBase ownerWindow, Func<int, AudioType> onUseAction = null) {
        this.index = index;
        this.ownerWindow = ownerWindow;
        this.onUseAction = onUseAction;
        // 设置物品快捷栏默认背景
        bgImg.sprite = ownerWindow.bgSprite[0];
    }

    // 初始化格子中的数据并刷新数值UI
    public void InitData(ItemData itemData = null) {
        this.itemData = itemData;
        // 如果数据为空则是空格子
        if (itemData == null) {
            // 外框设置为白色
            bgImg.color = Color.white;
            // 隐藏格子数值
            countText.gameObject.SetActive(false);
            // 隐藏格子图标
            iconImg.sprite = null;
            iconImg.gameObject.SetActive(false);
            return;
        }
        // 有数据时需要提前打开外框、数值、图标 
        countText.gameObject.SetActive(true);
        iconImg.gameObject.SetActive(true);
        iconImg.sprite = itemData.config.itemIcon;
        // 更新
        UpdateNumTextView();
    }

    public void UpdateNumTextView() {
        // 根据不同的物品类型显示不同的效果
        switch (itemData.config.itemType) {
            case ItemType.Weapon:
                bgImg.color = Color.white;
                countText.text = (itemData.itemTypeData as ItemWeaponData).durability.ToString() + "%";
                break;
            case ItemType.Consumable:
                // rgb通道+alpha值
                bgImg.color = new Color(0, 1, 0, 0.5f);
                countText.text = (itemData.itemTypeData as ItemConsumableData).count.ToString();
                break;
            case ItemType.Meterial:
                bgImg.color = Color.white;
                countText.text = (itemData.itemTypeData as ItemMaterialData).count.ToString();
                break;
            default:
                break;
        }
    }

    #region 鼠标交互事件
    private bool isMouseStay = false;
    private void MouseEnter(PointerEventData eventData, object[] arg2) {
        GameManager.Instance.SetCursorState(CursorState.Handle);
        bgImg.sprite = ownerWindow.bgSprite[1];
        isMouseStay = true;
        currentMouseEnterSlot = this;
    }

    private void MouseExit(PointerEventData eventData, object[] arg2) {
        GameManager.Instance.SetCursorState(CursorState.Normal);
        bgImg.sprite = ownerWindow.bgSprite[0];
        isMouseStay = false;
        currentMouseEnterSlot = null;
    }

    private void BeginDrag(PointerEventData eventData, object[] arg2) {
        // 格子中没有物体
        if (itemData == null) return;
        // 将拖拽图标设置到DragLayer上保证不会被其他UI图层覆盖
        iconTransform.SetParent(UIManager.Instance.DragLayer);
    }

    private void Drag(PointerEventData eventData, object[] arg2) {
        // 格子中没有物体
        if (itemData == null) return;
        // 拖拽过程: 1. 将事件数据arg1的位置赋值给图标位置; 2. 保持鼠标形状
        GameManager.Instance.SetCursorState(CursorState.Handle);
        iconTransform.position = eventData.position;
    }

    private void EndDrag(PointerEventData eventData, object[] arg2) {
        // 格子中没有物体
        if (itemData == null) return;
        // 当前是否拖拽结束时没有到其他格子里
        if (currentMouseEnterSlot == null) {
            // 鼠标形状恢复
            GameManager.Instance.SetCursorState(CursorState.Normal);
            // 物品图标复原
            iconTransform.SetParent(slotTransform);
            iconTransform.localScale = Vector3.one;
            iconTransform.localPosition = Vector3.zero;
            // 如果目标没有格子, 但是是UI物体, 可以无视
            if (InputManager.Instance.CheckMouseOnUI()) {
                return;
            }
            // 使用射线检测防止玩家将物品扔到大型物体身上
            if (InputManager.Instance.CheckMouseOnBigMapObject()) {
                return;
            }
            // 如果当前鼠标未点击到地面直接无视
            if (InputManager.Instance.GetMousePositionOnGround(eventData.position, out Vector3 worldPosition) == false) {
                return;
            }
            // 从存档里去除这份数据
            // 物品掉落在地上: 在地面生成物品, 并将y值设置为1
            worldPosition.y = 1;
            MapManager.Instance.GenerateMapObject(itemData.config.mapObjectConfigId, worldPosition, false);
            UnityEngine.Debug.Log("物品掉落在地上" + itemData.config.itemName);
            // 丢弃一件物品, 注意数据/UI/音效需要同步
            ProjectTool.PlayerAudio(AudioType.Bag);
            ownerWindow.DiscardItem(index);
            return;
        } else {
            // 拖拽到格子后的图标需要复原
            iconTransform.SetParent(slotTransform);
            iconTransform.localScale = Vector3.one;
            iconTransform.localPosition = Vector3.zero;
            // 如果拖拽到自己原本格子中
            if (currentMouseEnterSlot == this) return;
            // 判断拖入的格子类型
            if (currentMouseEnterSlot == weaponSlot) {
                // 当前进入的是武器格子
                if (itemData.config.itemType != ItemType.Weapon) {
                    ProjectTool.PlayerAudio(AudioType.Fail);
                    UIManager.Instance.AddTips("只能放入武器");
                } else {
                    ProjectTool.PlayerAudio(AudioType.TakeUpWeapon);
                    UnityEngine.Debug.Log("可以装备物品:" + itemData.config.itemName);
                    SwapSlotItem(this, currentMouseEnterSlot);
                }
            } else {
                // 当前进入是普通格子
                if (itemData.config.itemType != ItemType.Weapon) {
                    ProjectTool.PlayerAudio(AudioType.Bag);
                    UnityEngine.Debug.Log("交换物品:" + itemData.config.itemName);
                    SwapSlotItem(this, currentMouseEnterSlot);
                } else {
                    if (currentMouseEnterSlot.itemData == null) {
                        ProjectTool.PlayerAudio(AudioType.TakeDownWeapon);
                        UnityEngine.Debug.Log("脱下装备:" + itemData.config.itemName);
                        SwapSlotItem(this, currentMouseEnterSlot);
                    }
                    else {
                        ProjectTool.PlayerAudio(AudioType.Fail);
                        UnityEngine.Debug.Log("装备无法放入已经存在物品的格子中:" + itemData.config.itemName);
                    }
                }
            }
        }
        // 更新存档
        ArchiveManager.Instance.SaveMainInventoryData();
    }

    // 交换物品槽中的数据并刷新物品槽UI
    public static void SwapSlotItem(UI_ItemSlot slot1, UI_ItemSlot slot2) {
        ItemData itemData1 = slot1.itemData;
        ItemData itemData2 = slot2.itemData;
        slot1.ownerWindow.SetItem(slot1.index, itemData2);
        slot2.ownerWindow.SetItem(slot2.index, itemData1);
    }
    #endregion 
}
