using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : MonoBehaviour {
    [Header("In Game")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text itemGetText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private Button pauseResumeButton;
    [SerializeField] private Button pauseRestartButton;
    [SerializeField] private Button pauseMainMenuButton;

    [Header("GameOver Menu")]
    [SerializeField] private GameObject gameOverMenuCanvas;
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private Button gameOverMainMenuButton;
    [SerializeField] private TMP_Text highScoreText;

    [Header("Tutorial Menu")]
    [SerializeField] private GameObject tutorialMenuCanvas;
    [SerializeField] private Button nextButtonL;
    [SerializeField] private Button nextButtonR;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject[] rulePages;

    private int index;


    private void Init() {
        this.index = 0;

        this.pauseButton.onClick.AddListener(GamePause);

        this.pauseResumeButton.onClick.AddListener(GameResume);
        this.pauseRestartButton.onClick.AddListener(GameRestart);
        this.pauseMainMenuButton.onClick.AddListener(ReturnToMain);

        this.gameOverRestartButton.onClick.AddListener(GameRestart);
        this.gameOverMainMenuButton.onClick.AddListener(ReturnToMain);

        this.nextButtonL.onClick.AddListener(() => TutorialNextPage(1));
        this.nextButtonR.onClick.AddListener(() => TutorialNextPage(-1));
        this.closeButton.onClick.AddListener(() => TutorialPageClose());

        this.tutorialMenuCanvas.SetActive(true);
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
    }

    private void ReturnToMain() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    private void GameOver() {
        Time.timeScale = 0;
        this.gameOverMenuCanvas.SetActive(true);
        this.highScoreText.text = "Best Record" + "\n" + GameManager.instance.highScore.ToString();
    }

    public IEnumerator ItemGet(string name) {
        this.itemGetText.text = name + " Item Get!";
        this.itemGetText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        this.itemGetText.gameObject.SetActive(false);
    }

    private void TutorialNextPage(int way) {
        foreach (GameObject page in this.rulePages) {
            page.SetActive(false);
        }

        if (way == 1) {         // LEFT
            if (this.index <= 0) {
                this.index = this.rulePages.Length;
            }

            this.index -= 1;
        }
        else if (way == -1) {   // RIGHT
            if (this.index >= this.rulePages.Length-1) {
                this.index = -1;
            }

            this.index += 1;
        }

        this.rulePages[this.index].SetActive(true);
    }

    private void TutorialPageClose() {
        Time.timeScale = 1;
        this.tutorialMenuCanvas.SetActive(false);
    }
}