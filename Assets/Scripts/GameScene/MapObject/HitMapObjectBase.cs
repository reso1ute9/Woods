using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public abstract class HitMapObjectBase : MapObjectBase
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip[] hurtAudioClips;
    [SerializeField] private float maxHp;
    [SerializeField] private int LootObjectConfigId = -1;   // 死亡时掉落物品id, -1默认为无效掉落
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
        if (LootObjectConfigId == -1) {
            return;
        }
        // TODO: 具体处理掉落物品逻辑
    }
}
