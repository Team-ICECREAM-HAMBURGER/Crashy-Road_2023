using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour {
    [SerializeField] private AudioClip[] bgmList;

    private AudioSource bgmPlayer;
    private AudioSource policeBGM;


    private void Start() {
        this.bgmPlayer = GetComponent<AudioSource>();

        int index = Random.Range(0, this.bgmList.Length);

        this.bgmPlayer.clip = this.bgmList[index];
        this.bgmPlayer.Play();
    }

}
