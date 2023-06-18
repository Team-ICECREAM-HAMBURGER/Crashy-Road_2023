using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum ScreenSize {
    ScreenSize16_9 = 1,
    ScreenSize21_9 = 2
}

public enum Fps {
    FpsNative = 0,
    Fps60 = 1,
    Fps120 = 2
}

public enum GraphicQuality {
    GraphicQualityHigh = 0,
    GraphicQualityMedium = 1,
    GraphicQualityLow = 2
}

public enum Vsync {
    VsyncOn = 0,
    VsyncOff = 1
}

public enum Mute {
    MuteOn = 0,
    MuteOff = 1
}

public class GameOptionController : MonoBehaviour {
    class GameOptionData {
        public int screenSize = 0;
        public int fps = 0;
        public int vsync = 0;
        public int graphicQuality = 0;
        public int mute = 0;
        public float volume = 1f;
    }
    
    [Header("Screen Size")] 
    [SerializeField] private Toggle[] screenSizeToggles;

    [Header("Graphic Quality")] 
    [SerializeField] private Toggle[] graphicQualityToggles;

    [Header("FPS")]
    [SerializeField] private Toggle[] fpsToggles;

    [Header("Vsync")] 
    [SerializeField] private Toggle[] vsyncToggles;

    [Header("Sound Mute")] 
    [SerializeField] private Toggle[] muteToggles;
    
    [Header("Sound Scroll Bar")] 
    [SerializeField] private Scrollbar soundScrollbar;
    
    [Header("Components")]
    [SerializeField] private AudioMixer audioMixer;


    private int _oriScreenW;
    private int _oriScreenH;
    private float _soundVal;
    private GameOptionData _gameOptionData;
    
    
    private void Init() {
        this._gameOptionData = new GameOptionData();

        this._soundVal = 1f;
        this._oriScreenW = Screen.width;
        this._oriScreenH = Screen.height;

        LoadGameOptionData();
    }

    private void Start() {
        Init();
    }

    private void LoadGameOptionData() {
        string path = Application.persistentDataPath + "/GameOptionData.json";

        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            this._gameOptionData = JsonUtility.FromJson<GameOptionData>(json);
            
            this.screenSizeToggles[this._gameOptionData.screenSize].isOn = true;
            this.fpsToggles[this._gameOptionData.fps].isOn = true;
            this.vsyncToggles[this._gameOptionData.vsync].isOn = true;
            this.graphicQualityToggles[this._gameOptionData.graphicQuality].isOn = true;
            this.muteToggles[this._gameOptionData.mute].isOn = true;
            VolumeController(this._gameOptionData.volume);
        }
        else {
            SaveGameOptionData();
            LoadGameOptionData();
        }
    }

    private void SaveGameOptionData() {
        string json = JsonUtility.ToJson(this._gameOptionData);
        File.WriteAllText(Application.persistentDataPath + "/GameOptionData.json", json);
    }

    public void ResolutionChanger(int num) {
        switch (num) {
            case (int)ScreenSize.ScreenSize16_9 :
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
            case (int)ScreenSize.ScreenSize21_9 :
                Screen.SetResolution(2560, 1080, FullScreenMode.FullScreenWindow);
                break;
        }

        this._gameOptionData.screenSize = num;
        SaveGameOptionData();
    }

    public void FPSChanger(int num) {
        switch (num) {
            case (int)Fps.FpsNative :
                Application.targetFrameRate = -1;
                break;
            case (int)Fps.Fps60 :
                Application.targetFrameRate = 60;
                break;
            case (int)Fps.Fps120 :
                Application.targetFrameRate = 120;
                break;
        }
        
        this._gameOptionData.fps = num;
        SaveGameOptionData();
    }

    public void VsyncChanger(int num) {
        switch (num) {
            case (int)Vsync.VsyncOn :
                QualitySettings.vSyncCount = 1;
                break;
            case (int)Vsync.VsyncOff :
                QualitySettings.vSyncCount = 0;
                break;
        }

        this._gameOptionData.vsync = num;
        SaveGameOptionData();
    }

    public void GraphicQualityChanger(int num) {
        switch (num) {
            case (int)GraphicQuality.GraphicQualityHigh :
                QualitySettings.SetQualityLevel(2);
                break;
            case (int)GraphicQuality.GraphicQualityMedium :
                QualitySettings.SetQualityLevel(1);
                break;
            case (int)GraphicQuality.GraphicQualityLow :
                QualitySettings.SetQualityLevel(0);
                break;
        }

        this._gameOptionData.graphicQuality = num;
        SaveGameOptionData();
    }

    public void VolumeMuter(int num) {
        switch (num) {
            case (int)Mute.MuteOn :
                VolumeController(0);
                break;
            case (int)Mute.MuteOff :
                VolumeController(this._gameOptionData.volume);
                break;
        }

        this._gameOptionData.mute = num;
        SaveGameOptionData();
    }

    public void VolumeController(float value) {
        if (this._gameOptionData.mute == 0) {
            value = 0;
        }
        
        this._soundVal = value;
        this.soundScrollbar.value = value;
        this.audioMixer.SetFloat("Master", Mathf.Log10(value + 0.0001f) * 20);

        this._gameOptionData.volume = value;
        
        SaveGameOptionData();
    }
}