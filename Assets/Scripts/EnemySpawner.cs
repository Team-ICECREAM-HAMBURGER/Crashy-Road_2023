using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPooling))]
public class EnemySpawner : MonoBehaviour {
    [SerializeField] private float spawnTime;
    [SerializeField] private List<Transform> spanwPoints;

    
    private int index;


    private void Start() {
        StartCoroutine("EnemySpawn");
    }

    IEnumerator EnemySpawn() {
        while (true) {  // TODO: if player is alive
            GameObject enemy = EnemyPooling.instance.ActivePoolItem();    // spawn

            this.index = Random.Range(0, this.spanwPoints.Count);

            enemy.transform.position = spanwPoints[this.index].position;

            yield return new WaitForSeconds(this.spawnTime);    // wait
        }
    }
}