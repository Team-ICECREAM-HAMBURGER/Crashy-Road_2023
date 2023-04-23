using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamaManager : MonoBehaviour {
    public static GamaManager instance;

    public delegate void GameOverHandler();
    public event GameOverHandler gameOverHandler;


    private void Init() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Awake() {
        Init();
    }

    public void GameOver() {
        Debug.Log("GameOver");
        this.gameOverHandler();
    }
}