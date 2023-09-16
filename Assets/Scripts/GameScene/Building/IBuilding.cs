using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilding
{
    public GameObject gameObject { get; }
    public Collider Collider { get; }
    public List<Material> materialList { get; set; }

    public static Color Red = new Color(1, 0, 0, 0.5f);
    public static Color Green = new Color(0, 1, 0, 0.5f);

    // 初始化预览方法
    public virtual void InitOnPreview() {
        // 预览时关闭碰撞体, 如果开启的话可能会存在问题
        Collider.enabled = false;
        // 初始化材质球和MeshRender
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        materialList = new List<Material>();
        for (int i = 0; i < meshRenderers.Length; i++) {
            materialList.AddRange(meshRenderers[i].materials);
        }
        for (int i = 0; i < materialList.Count; i++) {
            // 默认为红色
            materialList[i].color = Red;
            // 设置成支持更改透明通道的rendering mode
            ProjectTool.SetMaterialRenderingMode(materialList[i], ProjectTool.RenderingMode.Fade);
        }
        OnPreView();
    }

    public void OnPreView() {}

    // 设置预览颜色
    public void SetColorOnPreview(bool isRed) {
        for (int i = 0; i < materialList.Count; i++) {
            materialList[i].color = isRed == true ? Red : Green;
        }
    }
}
