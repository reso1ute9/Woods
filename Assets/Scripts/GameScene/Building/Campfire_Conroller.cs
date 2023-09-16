using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Unity.VisualScripting;

public class Campfire_Conroller : BuildingBase
{
    [SerializeField] new Light light;
    [SerializeField] GameObject fire;
    private CampfireConfig campfireConfig;
    private float currentFeulValue;
    private bool isOnGround;

    public override void Init(MapChunkController mapChunk, ulong mapObjectId, bool isFromBuild) {
        base.Init(mapChunk, mapObjectId, isFromBuild);
        campfireConfig = ConfigManager.Instance.GetConfig<CampfireConfig>(ConfigName.Campfire);
        // TODO: 保存数据
        currentFeulValue = campfireConfig.defaultFuelValue;
        SetLight(currentFeulValue);
        isOnGround = true;
    }

    private void Update() {
        if (GameSceneManager.Instance.IsInitialized == false) {
            return;
        }
        if (isOnGround == true) {
            UpdateFeulValue();
        }
    }

    // 更新篝火燃料
    private void UpdateFeulValue() {
        if (currentFeulValue == 0) {
            return;
        }
        currentFeulValue = Mathf.Clamp(currentFeulValue - Time.deltaTime * campfireConfig.buringSpeed, 0, campfireConfig.maxFuelValue);
        SetLight(currentFeulValue);
    }

    // 预览模式额外设置
    public override void OnPreView() {
        base.OnPreView();
        isOnGround = false;
        // 关闭灯光和火焰效果
        SetLight(0);
    }

    // 设置火焰燃料
    private void SetLight(float fuelValue) {
        light.gameObject.SetActive(fuelValue != 0);
        fire.gameObject.SetActive(fuelValue != 0);
        if (fuelValue != 0) {
            // 计算当前燃料比例
            float fuelRatio = fuelValue / campfireConfig.maxFuelValue;
            // 设置灯光强度
            light.intensity = Mathf.Lerp(0, campfireConfig.maxLightIntensity, fuelRatio);
            // 设置灯光范围
            light.range = Mathf.Lerp(0, campfireConfig.maxLightRange, fuelRatio);
        }
    }
}
