using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class Tree_Controller : MapObjectBase {
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip[] hurtAudioClips;
    [SerializeField] private float maxHp;
    private float hp;
    

    public void Start() {
        hp = maxHp;
    }

    public void Hurt(float damage) {
        UnityEngine.Debug.Log("树受伤了:" + damage);
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
        // 将当前树木对象放到对象池并掉落物品(TODO)
        RemoveOnMap();
        // TODO: 显示树木倒下动画
    }
}
