using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpawner : ObjectPooling {
    public static EnemySpawner instance;
    
    [SerializeField] private float spawnTime;               // 적 생성 쿨 타임
    [SerializeField] private List<Transform> spawnPoints;   // 생성 위치

    private int _index; // spawnPoints list index


    private void Init() {
        if (instance == null) {
            instance = this;
        }
        
        base.Init();
        GameManager.instance.gameOverHandler += SpawnStop;
    }

    private void Start() {
        Init();
        StartCoroutine(nameof(EnemySpawn));
    }

    IEnumerator EnemySpawn() {
        while (true) {
            GameObject enemy = base.ActivatePoolItem();    // spawn

            this._index = Random.Range(0, this.spawnPoints.Count);

            enemy.transform.position = spawnPoints[this._index].position;
            enemy.transform.rotation = Quaternion.Euler(0, 0, 0);

            yield return new WaitForSeconds(this.spawnTime);    // wait
        }
    }

    public void EnemyReSpawn(GameObject target) {
        base.DeActivatePoolItem(target);
    }

    private void SpawnStop() {
        StopCoroutine(nameof(EnemySpawn));
    }
}