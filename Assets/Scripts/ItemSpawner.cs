using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObjectPooling))]
public class ItemSpawner : MonoBehaviour {
    [SerializeField] private float spawnTime;
    [SerializeField] private float maxDistance;

    private GameObject player;
    private ObjectPooling pooling;
    

    private void Init() {
        this.player = GameObject.FindGameObjectWithTag("Player");
        this.pooling = GetComponent<ObjectPooling>();

        GameManager.instance.gameOverHandler += SpawnStop;
    }

    private void Start() {
        Init();
        StartCoroutine("ItemSpawn");
    }

    IEnumerator ItemSpawn() {
        while (true) {
            yield return new WaitForSeconds(this.spawnTime);

            Vector3 spawnPosition = GetRandomPointOnNavMesh(this.player.transform.position, this.maxDistance);
            spawnPosition += Vector3.up * 1.5f;

            if (!float.IsInfinity(spawnPosition.x) || !float.IsInfinity(spawnPosition.y) || !float.IsInfinity(spawnPosition.z)) {
                GameObject item = this.pooling.ActivePoolItem();
                item.transform.position = spawnPosition;
            }
        }
    }

    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance) {
        Vector3 randomPosition = Random.insideUnitSphere * distance + center;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPosition, out hit, distance, NavMesh.AllAreas);

        return hit.position;
    }

    private void SpawnStop() {
        StopCoroutine("ItemSpawn");
    }
}