using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Unity.VisualScripting;

public class Campfire_Conroller : BuildingBase
{
    [SerializeField] new Light light;
    [SerializeField] GameObject fire;
    [SerializeField] private AudioSource audioSource;
    private CampfireConfig campfireConfig;
    private CampfireData campfireData;
    private bool isOnGround;

    public override void Init(MapChunkController mapChunk, ulong mapObjectId, bool isFromBuild) {
        base.Init(mapChunk, mapObjectId, isFromBuild);
        campfireConfig = ConfigManager.Instance.GetConfig<CampfireConfig>(ConfigName.Campfire);
        // 获取存档数据
        if (isFromBuild == true) {
            campfireData = new CampfireData();
            campfireData.currentFeulValue = campfireConfig.defaultFuelValue;
            ArchiveManager.Instance.AddMapObjectTypeData(mapObjectId, campfireData);
        } else {
            campfireData = ArchiveManager.Instance.GetMapObjectTypeData(mapObjectId) as CampfireData;
        }
        SetLight(campfireData.currentFeulValue);
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
        if (campfireData.currentFeulValue == 0) {
            return;
        }
        campfireData.currentFeulValue = Mathf.Clamp(
            campfireData.currentFeulValue - Time.deltaTime * campfireConfig.buringSpeed * TimeManager.Instance.timeScale, 
            0, 
            campfireConfig.maxFuelValue
        );
        SetLight(campfireData.currentFeulValue);
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
        audioSource.gameObject.SetActive((fuelValue != 0));
        if (fuelValue != 0) {
            // 计算当前燃料比例
            float fuelRatio = fuelValue / campfireConfig.maxFuelValue;
            // 设置灯光强度
            light.intensity = Mathf.Lerp(0, campfireConfig.maxLightIntensity, fuelRatio);
            // 设置灯光范围
            light.range = Mathf.Lerp(0, campfireConfig.maxLightRange, fuelRatio);
            // 设置篝火音量
            audioSource.volume = fuelRatio;
        }
    }

    // 设置鼠标拖拽方法
    public override bool OnSlotEndDragSelect(int itemId) {
        // 木材/燃料作为燃料物品
        if (campfireConfig.TryGetFuelValueByItemId(itemId, out float fuelValue)) {
            campfireData.currentFeulValue = Mathf.Clamp(
                campfireData.currentFeulValue + fuelValue, 
                0, 
                campfireConfig.maxFuelValue
            );
            SetLight(campfireData.currentFeulValue);
            return true;
        }
        // 检查是否能制作食物
        if (campfireConfig.TryGetBakedItemByItemId(itemId, out int bakedItemId)) {
            // 检查当前篝火是否有燃料
            if (campfireData.currentFeulValue <= 0) {
                UIManager.Instance.AddTips("需要点燃篝火");
                return false;
            }
            // 生成制作后的食物
            InventoryManager.Instance.AddMainInventoryWindowItem(bakedItemId);
            return true;
        }
        return false;
    } 
}
