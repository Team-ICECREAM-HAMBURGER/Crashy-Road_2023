using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GamaManager : MonoBehaviour {
    [System.Serializable]
    class SaveData {
        public string playerName;
        public int highScore;
    }

    public static GamaManager instance;

    private int score;
    private int highScore;
    private string playerName;

    public delegate void GameOverHandler();
    public event GameOverHandler gameOverHandler;


    private void Init() {
        if (instance == null) {
            instance = this;
        }
        
        LoadHighScore();
        this.gameOverHandler += SaveHighScore;
    }

    private void Awake() {
        Init();
    }

    private void Start() {
        StartCoroutine("TimeScoreCounter");
    }

    public void GameOver() {
        StopCoroutine("TimeScoreCounter");
        this.gameOverHandler();
    }

    private IEnumerator TimeScoreCounter() {
        while(true) {
            this.score += 1;
            yield return new WaitForSeconds(1f);
        }
    }

    private void SaveHighScore() {
        if (this.score > this.highScore) {
            this.highScore = this.score;
        }
        
        SaveData saveData = new SaveData();
        
        saveData.highScore = this.highScore;
        saveData.playerName = this.playerName;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    private void LoadHighScore() {
        string path = Application.persistentDataPath + "/savefile.json";
        
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
            this.highScore = saveData.highScore;
            this.playerName = saveData.playerName;
        }
    }
}