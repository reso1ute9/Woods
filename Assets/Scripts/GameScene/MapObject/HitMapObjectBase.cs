using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public abstract class HitMapObjectBase : MapObjectBase
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip[] hurtAudioClips;
    [SerializeField] private float maxHp;
    [SerializeField] private int lootObjectConfigId = -1;   // 死亡时掉落物品id, -1默认为无效掉落
    private float hp;

    public override void Init(MapChunkController mapChunk, ulong mapObjectId) {
        base.Init(mapChunk, mapObjectId);
        hp = maxHp;
    }

    public void Hurt(float damage) {
        UnityEngine.Debug.Log("当前攻击物体受伤了:" + damage);
        hp -= damage;
        if (hp <= 0) {
            Dead();
        } else {
            animator.SetTrigger("Hurt");
            AudioManager.Instance.PlayOnShot(
                hurtAudioClips[Random.Range(0, hurtAudioClips.Length)],
                transform.position
            );
        }
    }

    private void Dead() {
        // 将当前可以被攻击对象放到对象池并掉落物品(TODO)
        RemoveOnMap();
        // TODO: 显示树木倒下动画
        // 掉落物品
        if (lootObjectConfigId == -1) {
            return;
        }
        LootConfig lootConfig = ConfigManager.Instance.GetConfig<LootConfig>(ConfigName.Loot, lootObjectConfigId);
        if (lootConfig == null) {
            return;
        }
        // 根据概率决定是否实例化
        for (int i = 0; i < lootConfig.Configs.Count; i++) {
            int randValue = Random.Range(1, 101);
            if (randValue <= lootConfig.Configs[i].Probability) {
                // 生成掉落物品
                // 1. 掉落物品在父物体的上方一些
                float randomX = 1.0f * Random.Range(-10, 10) / 20;
                float randomZ = 1.0f * Random.Range(-10, 10) / 20;
                Vector3 pos = transform.position + new Vector3(randomX, 1, randomZ);
                MapManager.Instance.GenerateMapObject(mapChunk, lootConfig.Configs[i].LootObjectConfigId, pos);
            }
        }
    }
}
