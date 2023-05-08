using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ObjectPooling))]
public class EnemySpawner : MonoBehaviour {
    [SerializeField] private float spawnTime;
    [SerializeField] private List<Transform> spawnPoints;

    private int _index;
    private ObjectPooling _pooling;


    private void Init() {
        GameManager.instance.gameOverHandler += SpawnStop;
        this._pooling = GetComponent<ObjectPooling>();
    }

    private void Start() {
        Init();
        StartCoroutine(nameof(EnemySpawn));
    }

    IEnumerator EnemySpawn() {
        while (true) {  // TODO: if player is alive
            GameObject enemy = this._pooling.ActivePoolItem();    // spawn

            this._index = Random.Range(0, this.spawnPoints.Count);

            enemy.transform.position = spawnPoints[this._index].position;
            enemy.transform.rotation = Quaternion.Euler(0, 0, 0);

            yield return new WaitForSeconds(this.spawnTime);    // wait
        }
    }

    private void SpawnStop() {
        StopCoroutine(nameof(EnemySpawn));
    }
}