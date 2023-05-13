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

    [SerializeField] private ObjectPooling enemyPooling;
    [SerializeField] private ObjectPooling itemPooling;
    [SerializeField] private GameUIManager gameUIManager;

    public int Score { get; private set; }
    public int HighScore { get; private set; }

    public delegate void GameOverHandler();
    public event GameOverHandler gameOverHandler;


    private void Init() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        
        instance = this;

        this.Score = 0;
        LoadHighScore();

        Time.timeScale = 0;

        this.gameOverHandler += SaveHighScore;
    }

    private void Awake() {
        Init();
    }

    private void Start() {
        StartCoroutine(nameof(TimeScoreCounter));
    }

    public void GameOver() {        
        StopCoroutine(nameof(TimeScoreCounter));
        this.gameOverHandler();
    }

    private IEnumerator TimeScoreCounter() {
        while(true) {
            yield return new WaitForSeconds(1f);
            this.Score += 1;
        }
    }

    public void EnemyDeactivate(GameObject obj) {
        this.enemyPooling.DeActivePoolItem(obj);
    }

    public void ItemDeactivate(GameObject obj) {
        this.itemPooling.DeActivePoolItem(obj);
    }

    public void ScoreUp(int score) {
        this.Score += score;
    }

    public void ItemGet(string name) {
        this.gameUIManager.StartCoroutine(nameof(GameUIManager.ItemGet), name);
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