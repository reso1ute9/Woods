using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineTool
{
    public static IEnumerator WaitForSconds(float time) {
        float currentTime = 0;
        while (currentTime < time) {
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}
