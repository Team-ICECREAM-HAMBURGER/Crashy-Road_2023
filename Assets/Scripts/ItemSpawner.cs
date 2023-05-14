using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemSpawner : ObjectPooling {
    public static ItemSpawner instance;
    
    [SerializeField] private float spawnTime;
    [SerializeField] private float maxDistance;

    private GameObject _player;
    

    private void Init() {
        if (instance == null) {
            instance = this;
        }
        
        this._player = GameObject.FindGameObjectWithTag("Player");
        base.Init();
        GameManager.instance.gameOverHandler += SpawnStop;
    }

    private void Start() {
        Init();
        StartCoroutine(nameof(ItemSpawn));
    }

    IEnumerator ItemSpawn() {
        while (true) {
            yield return new WaitForSeconds(this.spawnTime);

            Vector3 spawnPosition = GetRandomPointOnNavMesh(this._player.transform.position, this.maxDistance);
            spawnPosition += Vector3.up * 1.5f;

            if (!float.IsInfinity(spawnPosition.x) || !float.IsInfinity(spawnPosition.y) || !float.IsInfinity(spawnPosition.z)) {
                GameObject item = base.ActivatePoolItem();
                item.transform.position = spawnPosition;
            }
        }
    }
    
    public void ItemReSpawn(GameObject target) { 
        base.DeActivatePoolItem(target);
    }

    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance) {
        Vector3 randomPosition = Random.insideUnitSphere * distance + center;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPosition, out hit, distance, NavMesh.AllAreas);

        return hit.position;
    }

    private void SpawnStop() {
        StopCoroutine(nameof(ItemSpawn));
    }
}