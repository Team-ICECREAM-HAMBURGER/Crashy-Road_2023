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

    public int score { get; private set; }
    public int highScore { get; private set; }

    public delegate void GameOverHandler();
    public event GameOverHandler gameOverHandler;


    private void Init() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        
        instance = this;

        this.score = 0;
        LoadHighScore();

        Time.timeScale = 0;

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
            yield return new WaitForSeconds(1f);
            this.score += 1;
        }
    }

    public void EnemyDeactive(GameObject obj) {
        this.enemyPooling.DeActivePoolItem(obj);
    }

    public void ItemDeactive(GameObject obj) {
        this.itemPooling.DeActivePoolItem(obj);
    }

    public void ScoreUp(int score) {
        this.score += score;
    }

    public void ItemGet(string name) {
        this.gameUIManager.StartCoroutine("ItemGet", name);
    }

    private void SaveHighScore() {
        if (this.score > this.highScore) {
            this.highScore = this.score;
        }
        
        SaveData saveData = new SaveData();
        
        saveData.highScore = this.highScore;

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    private void LoadHighScore() {
        string path = Application.persistentDataPath + "/savefile.json";
        
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
            this.highScore = saveData.highScore;
        }
    }
}