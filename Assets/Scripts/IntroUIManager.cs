using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroUIManager : MonoBehaviour {
    [SerializeField] private Button singlePlayButton;
    [SerializeField] private Button multiPlayButton;
    [SerializeField] private Button quitButton;


    private void Init() {
        this.singlePlayButton.onClick.AddListener(SinglePlay);
        this.multiPlayButton.onClick.AddListener(MultiPlay);
        this.quitButton.onClick.AddListener(GameQuit);
    }

    private void Start() {
        Init();
    }
    
    private void SinglePlay() {
        SceneManager.LoadScene(1);
    }

    private void MultiPlay() {
        // TODO
    }

    private void GameQuit() {
        Application.Quit();
    }
}