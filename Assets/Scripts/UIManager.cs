using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour {
    [Header("In Game")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private TMP_Text scoreText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private Button pauseResumeButton;
    [SerializeField] private Button pauseRestartButton;
    [SerializeField] private Button pauseMainMenuButton;

    [Header("GameOver Menu")]
    [SerializeField] private GameObject gameOverMenuCanvas;
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private Button gameOverMainMenuButton;


    private void Init() {
        this.pauseButton.onClick.AddListener(GamePause);

        this.pauseResumeButton.onClick.AddListener(GameResume);
        this.pauseRestartButton.onClick.AddListener(GameRestart);
        this.pauseMainMenuButton.onClick.AddListener(ReturnToMain);

        this.gameOverRestartButton.onClick.AddListener(GameRestart);
        this.gameOverMainMenuButton.onClick.AddListener(ReturnToMain);

        this.pauseMenuCanvas.SetActive(false);
        this.gameOverMenuCanvas.SetActive(false);

        GameManager.instance.gameOverHandler += GameOver;
    }

    private void Start() {
        Init();
    }

    private void Update() {
        this.scoreText.text = GameManager.instance.score.ToString();
    }

    private void GamePause() {
        Time.timeScale = 0;
        this.pauseMenuCanvas.SetActive(true);
    }

    private void GameResume() {
        Time.timeScale = 1;
        this.pauseMenuCanvas.SetActive(false);
    }

    private void GameRestart() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        GameManager.instance.gameObject.SetActive(false);
        GameManager.instance.gameObject.SetActive(true);
    }

    private void ReturnToMain() {
        SceneManager.LoadScene(0);
    }

    private void GameOver() {
        Time.timeScale = 0;
        this.gameOverMenuCanvas.SetActive(true);

        GameManager.instance.gameObject.SetActive(false);
    }
}