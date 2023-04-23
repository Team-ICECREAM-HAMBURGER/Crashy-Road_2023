// EnemyPooling.cs
/*
    적(경찰차)들을 생성할 때 사용되는 Pooling 기법을 구현합니다.
    EnemySpawner의 필수 컴포넌트입니다.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPooling : MonoBehaviour {
    private class PoolingObject {
        public GameObject prefab;
        public bool isActive;
    }

    public static EnemyPooling instance;

    [SerializeField] private GameObject enemyPrefab;
    
    private List<PoolingObject> poolingObjects;
    private float maxCount;
    private float addCount;
    private float activeCount;


    private void Init() {
        if (instance == null) {
            instance = this;
        }

        this.poolingObjects = new List<PoolingObject>();

        this.maxCount = 0;
        this.addCount = 5;
        this.activeCount = 0;

        AddPoolItem();
    }

    private void Awake() {
        Init();
    }
    
    public void AddPoolItem() {
        this.maxCount += this.addCount;

        for (int i = 0; i < this.addCount; i++) {
            PoolingObject poolingObject = new PoolingObject();

            poolingObject.prefab = GameObject.Instantiate(this.enemyPrefab);
            poolingObject.prefab.SetActive(false);
        
            poolingObject.isActive = false;

            this.poolingObjects.Add(poolingObject);
        }
    }

    public GameObject ActivePoolItem() {
        if (this.poolingObjects == null) {
            return null;
        }

        if (this.maxCount == this.activeCount) {
            AddPoolItem();
        }

        foreach (PoolingObject obj in this.poolingObjects) {
            if (!obj.isActive) {
                obj.isActive = true;
                obj.prefab.SetActive(true);
                this.activeCount += 1;

                return obj.prefab;
            }
        }
        
        return null;
    }

    public void DeActivePoolItem(GameObject deActiveObject) {
        if (this.poolingObjects == null) {
            return; 
        }

        foreach (PoolingObject obj in this.poolingObjects) {
            if (obj.prefab == deActiveObject) {
                obj.prefab.SetActive(false);
                obj.isActive = false;
                this.activeCount -= 1;

                return;
            }
        }
    }
}