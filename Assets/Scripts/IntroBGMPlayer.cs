using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBGMPlayer : MonoBehaviour {
    [SerializeField] private AudioClip[] bgmList;
    [SerializeField] private AudioSource bgmPlayer;

    
    private void Init() {
        int index = Random.Range(0, this.bgmList.Length);

        this.bgmPlayer.clip = this.bgmList[index];
        this.bgmPlayer.Play();
    }
    
    private void Start() {
        Init();
    }
}
