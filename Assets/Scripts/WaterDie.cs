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
                waterDieAudioSource.Play();
                other.GetComponent<EnemyVehicleController>().Explosion();
                EnemySpawner.instance.EnemyReSpawn(other.gameObject);
            }
            
            other.gameObject.SetActive(false);
        }
    }
}