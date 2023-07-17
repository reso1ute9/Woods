using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class MenuSceneManager : LogicManagerBase<MenuSceneManager>
{

    public AudioClip BGAudio;
    public AudioClip BGAudio_fire;

    protected override void CancelEventListener() {}
    protected override void RegisterEventListener() {}

    private void Start() {
        UIManager.Instance.Show<UI_MenuScenceMainWindow>();
        // 框架本身不支持播放多个背景音乐
        InvokeRepeating(nameof(PlayFireAudio), 0.2f, BGAudio_fire.length);
        AudioManager.Instance.PlayBGAudio(BGAudio, true, 1.0f);

    }

    private void PlayFireAudio() {
        AudioManager.Instance.PlayOnShot(BGAudio_fire, Vector3.zero, 0.8f, false);
    }
}
