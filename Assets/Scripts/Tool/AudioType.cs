using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


// 音效类型
public enum AudioType
{
    [LabelText("玩家无法使用")] PlayerConnot, 
    [LabelText("装备武器")] TakeUpWeapon,
    [LabelText("卸载武器")] TakeDownWeapon,
    [LabelText("消耗品成功使用")] ConsumableOK,
    [LabelText("消耗品使用失败")] ConsumableFail,
    [LabelText("背包")] Bag,
    [LabelText("通用失败音效")] Fail,
}
