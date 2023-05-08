using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour {
    private class PoolingObject {
        public GameObject prefab;
        public bool isActive;
    }

    [SerializeField] private GameObject objectPrefab;
    
    private List<PoolingObject> _poolingObjects;
    private float _maxCount;
    private float _addCount;
    private float _activeCount;
    

    private void Init() {
        this._poolingObjects = new List<PoolingObject>();

        this._maxCount = 0;
        this._addCount = 5;
        this._activeCount = 0;

        AddPoolItem();
    }

    private void Start() {
        Init();
    }
    
    public void AddPoolItem() {
        this._maxCount += this._addCount;

        for (int i = 0; i < this._addCount; i++) {
            PoolingObject poolingObject = new PoolingObject();

            poolingObject.prefab = GameObject.Instantiate(this.objectPrefab);
            poolingObject.prefab.SetActive(false);
        
            poolingObject.isActive = false;

            this._poolingObjects.Add(poolingObject);
        }
    }

    public GameObject ActivePoolItem() {
        if (this._poolingObjects == null) {
            return null;
        }

        if (this._maxCount == this._activeCount) {
            AddPoolItem();
        }

        foreach (PoolingObject obj in this._poolingObjects) {
            if (!obj.isActive) {
                obj.isActive = true;
                obj.prefab.SetActive(true);
                this._activeCount += 1;

                return obj.prefab;
            }
        }
        
        return null;
    }

    public void DeActivePoolItem(GameObject deActiveObject) {
        if (this._poolingObjects == null) {
            return; 
        }

        foreach (PoolingObject obj in this._poolingObjects) {
            if (obj.prefab == deActiveObject) {
                obj.prefab.SetActive(false);
                obj.isActive = false;
                this._activeCount -= 1;

                return;
            }
        }
    }
}