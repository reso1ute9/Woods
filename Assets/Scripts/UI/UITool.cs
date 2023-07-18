using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using JKFrame;

public static class UITool
{
    // 设置绑定鼠标效果
    public static void BindMouseEffect(this Component component) {
        // 鼠标进入效果
        component.OnMouseEnter(MouseEffect, );
    }

    // arg1:
    private static void MouseEffect(PointerEventData arg1, object[] arg2) {
    
    }
}
