using JKFrame;
using UnityEngine;

[Pool]
public class Hit2Effect : MonoBehaviour
{
    private void OnParticleSystemStopped() {
        this.JKGameObjectPushPool();
    }
}
