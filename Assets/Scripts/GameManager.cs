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

    public int score { get; private set; }
    private int highScore;

    public delegate void GameOverHandler();
    public event GameOverHandler gameOverHandler;


    private void Init() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(instance);

        this.score = 0;
        LoadHighScore();

        this.gameOverHandler += SaveHighScore;
    }

    private void OnEnable() {
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

    public void ScoreUp(int score) {
        this.score += score;
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