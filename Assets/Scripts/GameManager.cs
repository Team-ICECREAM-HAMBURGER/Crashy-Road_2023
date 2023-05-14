using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [System.Serializable]
    class SaveData {
        public int highScore;
    }

    public static GameManager instance;

    [SerializeField] private ObjectPooling itemPooling;
    [SerializeField] private GameUIManager gameUIManager;

    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public bool IsGameOver { get; set; }

    public delegate void GameOverHandler();
    public event GameOverHandler gameOverHandler;


    private void Init() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        
        instance = this;

        this.IsGameOver = false;
        this.Score = 0;
        LoadHighScore();
        Time.timeScale = 0;
        this.gameOverHandler += SaveHighScore;
    }

    private void Awake() {  // DO NOT CHANGE THIS METHOD
        Init();
    }

    private void Start() {
        StartCoroutine(nameof(TimeScoreCounter));
    }

    public void GameOver() {
        this.IsGameOver = true;
        this.gameOverHandler();
    }

    private IEnumerator TimeScoreCounter() {
        while(!this.IsGameOver) {
            yield return new WaitForSeconds(1f);
            this.Score += 1;
        }
    }
    
    public void ScoreUp(int score) {
        if (!this.IsGameOver) {
            this.Score += score;
        }
    }
    
    private void SaveHighScore() {
        if (this.Score > this.HighScore) {
            this.HighScore = this.Score;
        }
        
        SaveData saveData = new SaveData();
        saveData.highScore = this.HighScore;
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    private void LoadHighScore() {
        string path = Application.persistentDataPath + "/savefile.json";
        
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            this.HighScore = saveData.highScore;
        }
    }
}