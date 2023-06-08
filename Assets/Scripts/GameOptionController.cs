using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum ScreenSize {
    ScreenSizeNative = 0,
    ScreenSize16_9 = 1,
    ScreenSize21_9 = 2
}

public class GameOptionController : MonoBehaviour {
    [Header("Graphic Screen Size")] 
    [SerializeField] private Toggle[] screenSizeToggles;

    [Header("Graphic Quality")] 
    [SerializeField] private Toggle qualityHigh;
    [SerializeField] private Toggle qualityMedium;
    [SerializeField] private Toggle qualityLow;

    [Header("FPS")] 
    [SerializeField] private Toggle fpsNative;
    [SerializeField] private Toggle fps60;
    [SerializeField] private Toggle fps120;

    [Header("Vsync")] 
    [SerializeField] private Toggle vsyncOn;
    [SerializeField] private Toggle vsyncOff;
    
    [Header("Sound Scroll Bar")] 
    [SerializeField] private Scrollbar soundScrollbar;
    
    [Header("Components")]
    [SerializeField] private AudioMixer mainMixer;
    
    private int _oriScreenW;
    private int _oriScreenH;
    private float _soundVal = 1;
    
    
    private void Init() {
        this.screenSizeToggles[this.gameOptionStatus.screenSize].isOn = true;
    }

    private void Start() {
        Init();
    }

    public void ResolutionChanger(int num) {   // TODO: 토글 클릭 시, isOn 항목 저장 후 복원
        switch (num) {
            case (int)ScreenSize.ScreenSizeNative :
                Screen.SetResolution(this._oriScreenW, this._oriScreenH, FullScreenMode.FullScreenWindow);
                break;
            case (int)ScreenSize.ScreenSize16_9 :
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
            case (int)ScreenSize.ScreenSize21_9 :
                Screen.SetResolution(2560, 1080, FullScreenMode.FullScreenWindow);
                break;
        }
    }

    public void FPSOption(int num) {    // TODO: 토글 클릭 시, isOn 항목 저장 후 복원
        Application.targetFrameRate = num;
    }

    public void VsyncOption(int num) {  // TODO: 토글 클릭 시, isOn 항목 저장 후 복원
        QualitySettings.vSyncCount = num;
    }

    public void QualityOption(int num) {    // TODO: 토글 클릭 시, isOn 항목 저장 후 복원
        QualitySettings.SetQualityLevel(num);
    }

    public void Mute(bool isMute) { // TODO: 토글 클릭 시, isOn 항목 저장 후 복원
        if (isMute) {
            SoundControl(0);
        }
        else {
            SoundControl(1);
        }
    }

    public void SoundControl(float value) { // TODO: 음량 게이지 저장 후 복원
        this._soundVal = value;
        this.soundScrollbar.value = value;
        this.mainMixer.SetFloat("Master", Mathf.Log10(value + 0.0001f)*20);
    }
}
