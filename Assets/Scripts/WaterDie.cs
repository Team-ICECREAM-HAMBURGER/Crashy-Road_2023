// WaterDie.cs
/*
    플레이어 차량이나 경찰차, 오브젝트들이 물에 빠졌을 때 오브젝트 비활성화 처리를 위해 사용됩니다.
    플레이어 차량이 물에 빠질 경우 GameManager의 GameOver()를 호출합니다.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDie : MonoBehaviour {
    [SerializeField] private AudioSource waterDieAudioSource;


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject != this) {
            if (other.gameObject.CompareTag("Player")) {
                waterDieAudioSource.Play();
                GameManager.instance.GameOver();
            }
            else if (other.gameObject.CompareTag("Police")) {
                GameManager.instance.EnemyDeactive(other.gameObject);
                GameManager.instance.ScoreUp(10);
            }
            other.gameObject.SetActive(false);
        }
    }
}