using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameOptionController : MonoBehaviour {
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Scrollbar soundBar;

    private int _oriScreenW;
    private int _oriScreenH;
    private static float _soundVal = 1;
    


    private void Init() {
        this._oriScreenW = Screen.currentResolution.width;
        this._oriScreenH = Screen.currentResolution.height;

        this.soundBar.value = _soundVal;
        SoundControl(_soundVal);
    }

    private void Start() {
        Init();
    }

    public void Resolution(int num) {
        switch (num) {
            case 1 :
                Screen.SetResolution(this._oriScreenW, this._oriScreenH, FullScreenMode.FullScreenWindow);
                break;
            case 2 :
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
            case 3 : 
                Screen.SetResolution(2560, 1080, FullScreenMode.FullScreenWindow);
                break;
        }
    }

    public void FPSOption(int num) {
        Application.targetFrameRate = num;
    }

    public void VsyncOption(int num) {
        QualitySettings.vSyncCount = num;
    }

    public void QualityOption(int num) {
        QualitySettings.SetQualityLevel(num);
    }

    public void Mute(bool isMute) {
        if (isMute) {
            SoundControl(0);
        }
        else {
            SoundControl(1);
        }
    }

    public void SoundControl(float value) {
        _soundVal = value;
        this.soundBar.value = value;
        this.mainMixer.SetFloat("Master", Mathf.Log10(value + 0.0001f)*20);
    }
}
