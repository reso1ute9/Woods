using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;

// 物品快捷栏中的格子
public class UI_ItemSlots : MonoBehaviour
{
    [SerializeField] Image bgImg;                   // 格子背景图片
    [SerializeField] Image iconImg;                 // 格子里图标
    [SerializeField] Text countText;                // 格子中显示的数值
    
    public ItemData itemData { get; private set; }  // 格子中的数据
    public int index { get; private set; }          // 格子编号

    private UI_InventoryWindow ownerWindow;         // 宿主窗口: 物品栏/仓库

    // 初始化格子
    public void Init(int index, UI_InventoryWindow ownerWindow) {
        this.index = index;
        this.ownerWindow = ownerWindow;
    }

    // 初始化格子中的数据
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
        // 根据不同的物品类型显示不同的效果
        switch (itemData.config.itemType)
        {
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
}
