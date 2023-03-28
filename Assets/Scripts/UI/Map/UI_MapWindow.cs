using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;

// UI地图窗口
// 使用JKFrame中的特性设置缓存/资源路径/UI层数
[UIElement(true, "UI/UI_MapWindow", 4)]
public class UI_MapWindow : UI_WindowBase
{
    [SerializeField] private RectTransform content;     // 所有地图块/icon显示的父物体
    private float contentSize;                          // 地图尺寸
    [SerializeField] private GameObject mapItemPrefab;  // 单个地图块在UI中的预制体
    [SerializeField] private GameObject mapIconPrefab;  // 单个icon在UI中的预制体
    [SerializeField] private RectTransform playerIcon;  // 玩家所在位置的icon

    private Dictionary<Vector2Int, Image> mapImageDict = new Dictionary<Vector2Int, Image>();   // 地图图片字典
    private float mapItemSize;      // UI地图块的尺寸
    private float mapSizeOnWorld;   // 3D地图在世界地图中的坐标
    private Sprite forestSprite;    // 森林地块的精灵

    private float minScale;         // 地图最小放大倍数
    private float maxScale;         // 地图最大放大倍数

    // 初始化地图
    // mapSize: 一行或者一列有多少个image/chunk
    // mapSizeOnWorld: 地图在世界中一行或者一列有多大
    // forestTexture: 森林贴图
    public void InitMap(float mapSize, float mapSizeOnWorld,Texture2D forestTexture) {
        this.mapSizeOnWorld = mapSizeOnWorld;
        this.forestSprite = CreateMapSprite(forestTexture);

        // content尺寸: 默认content尺寸要大于地图尺寸
        contentSize = mapSizeOnWorld * 10;
        content.sizeDelta = new Vector2(contentSize, contentSize);

        // 一个UI地图块尺寸
        // mapItemSize = 
    }

    // 生成地图块的Sprite
    private Sprite CreateMapSprite(Texture2D texture) {
        return Sprite.Create(
            texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }
}
