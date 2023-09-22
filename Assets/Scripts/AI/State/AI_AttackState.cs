using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

// AI攻击状态
public class AI_AttackState : AIStateBase
{
    public override void Enter() {
        // 随机播放一个攻击动画
        int index = Random.Range(1, 3);
        AI.PlayAnimation("Attack_" + index.ToString());
        AI.transform.LookAt(Player_Controller.Instance.playerTransform.position);
        // 播放音效
        AI.PlayAudio("Attack");

        AI.AddAnimationEvent("StartHit", StartHit);
        AI.AddAnimationEvent("StopHit", StopHit);
        AI.AddAnimationEvent("AttackOver", AttackOver);
        // 对于进入trigger的物体每一帧都执行一次检测和逻辑代码
        AI.Weapon.OnTriggerStay(CheckHitTriggerStay);
    }

    // 武器伤害检测
    private bool isAttacked = false;        // 是否已经攻击过了
    private void CheckHitTriggerStay(Collider other, object[] args) {
        // 避免一次攻击产生多次伤害
        if (isAttacked == true) {
            return;
        }
        if (other.gameObject.CompareTag("Player")) {
            isAttacked = true;
            AI.PlayAudio("Hit");
            Player_Controller.Instance.Hurt(AI.AttackValue);
        }
    }

    // 开启伤害
    private void StartHit() {
        AI.Weapon.gameObject.SetActive(true);
    }

    // 关闭伤害
    private void StopHit() {
        isAttacked = false;
        AI.Weapon.gameObject.SetActive(false);
    }

    // 攻击结束
    private void AttackOver() {
        AI.ChangeState(AIState.Pursue);
    }

    public override void Exit() {
        AI.RemoveAnimationEvent("StartHit", StartHit);
        AI.RemoveAnimationEvent("StopHit", StopHit);
        AI.RemoveAnimationEvent("AttackOver", AttackOver);
        AI.RemoveTriggerStay(CheckHitTriggerStay);
    }
}
