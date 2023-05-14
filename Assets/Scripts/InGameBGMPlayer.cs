using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBGMPlayer : MonoBehaviour {
    [SerializeField] private AudioClip[] bgmList;
    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioSource policeSiren;


    private void Init() {
        int index = Random.Range(0, this.bgmList.Length);

        this.bgmPlayer.clip = this.bgmList[index];
        this.bgmPlayer.Play();
        this.policeSiren.Play();
        
        GameManager.instance.gameOverHandler += BGMStop;
    }
    
    private void Start() {
        Init();
    }

    private void BGMStop() {
        this.policeSiren.Stop();
        this.bgmPlayer.Stop();
    }
}
