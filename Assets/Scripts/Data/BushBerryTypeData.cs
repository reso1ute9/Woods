using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BushBerryTypeData : IMapObjectTypeData
{
    public int lastPickUpDayNum = -1;       // 浆果最后一次被采摘的天数
}
