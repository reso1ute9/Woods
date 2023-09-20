using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;
using System;

// 物品类型
public enum ItemType {
    [LabelText("装备")] Weapon,
    [LabelText("消耗品")] Consumable,
    [LabelText("材料")] Meterial,
}

// 武器类型
public enum WeaponType {
    [LabelText("斧头")] Axe,
    [LabelText("镐")] PickAxe,
    [LabelText("镰刀")] Sickle,
}

// 物品配置
[CreateAssetMenu(menuName = "Config/物品配置")]
public class ItemConfig : ConfigBase
{
    [LabelText("类型"), OnValueChanged(nameof(OnItemTypeChanged))] public ItemType itemType;
    [LabelText("名称")] public string itemName; 
    [LabelText("地图物品Id")] public int mapObjectConfigId;
    [LabelText("描述"), MultiLineProperty] public string descript;
    [LabelText("图标")] public Sprite itemIcon;
    [LabelText("类型专属信息")] public IItemTypeInfo itemTypeInfo;

    // 当类型修改时自动生成同等类型应有的专属信息
    private void OnItemTypeChanged() {
        switch (itemType) {
            case ItemType.Weapon:
                itemTypeInfo = new ItemWeaponInfo();
                break;
            case ItemType.Consumable:
                itemTypeInfo = new ItemConsumableInfo();
                break;
            case ItemType.Meterial:
                itemTypeInfo = new ItemMaterialInfo();
                break;
            default:
                break;
        }
    }
}

// 物品类型信息接口
public interface IItemTypeInfo {}

// 设置抽象类对可堆放物品进行进一步抽象
[Serializable]
public abstract class PileItemTypeInfoBase {
    [LabelText("堆叠上限")] public int maxCount;
}

// 武器类型信息
[Serializable]
public class ItemWeaponInfo: IItemTypeInfo {
    [LabelText("武器类型")] public WeaponType weaponType;
    [LabelText("玩家手中的预制体")] public GameObject prefabOnPlayer;
    [LabelText("玩家手中武器坐标")] public Vector3 positionOnPlayer;
    [LabelText("玩家手中武器旋转")] public Vector3 rotationOnPlayer;
    [LabelText("地图中的预制体")] public GameObject prefabOnMap;
    [LabelText("动画状态机")] public AnimatorOverrideController animatorController;
    [LabelText("攻击力")] public float attackValue;
    [LabelText("每次攻击损耗耐久度")] public float attackDurabilityCost;
    [LabelText("攻击距离")] public float attackDistance;
    // TODO: 攻击音效
}

// 消耗品类型信息
[Serializable]
public class ItemConsumableInfo: PileItemTypeInfoBase, IItemTypeInfo {
    [LabelText("恢复生命值")] public float recoverHP;
    [LabelText("恢复饱食度")] public float recoverHungry;
}

// 材料类型信息
[Serializable]
public class ItemMaterialInfo: PileItemTypeInfoBase, IItemTypeInfo {
}
