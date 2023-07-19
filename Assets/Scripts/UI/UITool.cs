using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using JKFrame;

public static class UITool
{
    // 设置绑定鼠标效果
    public static void BindMouseEffect(this Component component) {
        // 鼠标进入进入后要放大
        component.OnMouseEnter(MouseEffect, component, false, component.transform.localScale);
        // 鼠标离开后要恢复正常
        component.OnMouseExit(MouseEffect, component, true, component.transform.localScale);
    }

    // arg1:
    private static void MouseEffect(PointerEventData arg1, object[] args) {
        
        Component component = (Component)args[0];
        bool isNormal = (bool)args[1];              // 是否进行缩放
        Vector3 normalScale = (Vector3)args[2];     // button正常尺寸
        // 设置鼠标指针外观
        if (isNormal == true) {
            GameManager.Instance.SetCursorState(CursorState.Normal);
        } else {
            GameManager.Instance.SetCursorState(CursorState.Handle);
        }
        component.StartCoroutine(DoMouseEffect(component, isNormal, normalScale));
    }

    public static void RemoveMosueEffect(this Component component) {
        component.RemoveMouseEnter(MouseEffect);
        component.RemoveMouseExit(MouseEffect);
        component.StopAllCoroutines();
        // 强制将指针改成默认形状
        GameManager.Instance.SetCursorState(CursorState.Normal);
    }


    private static IEnumerator DoMouseEffect(Component component, bool targetIsNormal, Vector3 normalScale) {
        // 预先获取transform
        Transform transform = component.transform;
        Vector3 currentScale = transform.localScale;
        if (targetIsNormal == true) {
            // 缩小
            Vector3 targetScale = normalScale;
            while (transform.localScale.x > targetScale.x) {
                // 停留一帧
                yield return null;  
                currentScale -= Time.deltaTime * 2 * Vector3.one;
                transform.localScale = currentScale;
            }
            transform.localScale = targetScale;
        } else {
            // 放大
            Vector3 targetScale = normalScale * 1.2f;
            while (transform.localScale.x < targetScale.x) {
                // 停留一帧
                yield return null;  
                currentScale += Time.deltaTime * 2 * Vector3.one;
                transform.localScale = currentScale;
            }
            transform.localScale = targetScale;
        }
    }
}
