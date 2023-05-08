using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour {
    [SerializeField] private AudioClip[] bgmList;

    private AudioSource _bgmPlayer;
    private AudioSource _policeBGM;


    private void Start() {
        this._bgmPlayer = GetComponent<AudioSource>();

        int index = Random.Range(0, this.bgmList.Length);

        this._bgmPlayer.clip = this.bgmList[index];
        this._bgmPlayer.Play();
    }
}
