using System.Collections;
using JKFrame;
using UnityEngine;

// AI待机状态
public class AI_Idle : AIStateBase
{
    private Coroutine goPatorlCoroutin;
    
    public override void Enter() {
        // 播放待机动画
        AI.PlayAnimation("Idle");
        // 播放音效: 有一定概率(1/30)发出声音
        if (Random.Range(0, 30) == 0) {
            AI.PlayAudio("Idle", 0.5f);
        }
        // 休息一段时间后去巡逻
        goPatorlCoroutin = MonoManager.Instance.StartCoroutine(GoPatorlCoroutin());
    }

    IEnumerator GoPatorlCoroutin() {
        yield return CoroutineTool.WaitForSconds(Random.Range(0, 1));
        AI.ChangeState(AIState.Patrol);
    }

    public override void Exit() {
        if (goPatorlCoroutin != null) {
            MonoManager.Instance.StopCoroutine(GoPatorlCoroutin());
            goPatorlCoroutin = null;
        }
    }
}