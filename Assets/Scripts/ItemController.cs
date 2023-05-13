using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour, IItem {
    enum Type {
        Shield,
        SpeedUp
    }

    private Type _itemType;
    

    private void OnEnable() {
        this._itemType = (Type)Random.Range(0, 2);     
    }

    public void Use(GameObject obj) {
        PlayerVehicleController pvc = obj.GetComponent<PlayerVehicleController>();

        switch (this._itemType) {
            case Type.Shield :
                pvc.StartCoroutine("Shield");
                break;
            case Type.SpeedUp :
                pvc.StartCoroutine("SpeedUp");
                break;
        }      

        GameManager.instance.ItemGet(this._itemType.ToString());
        GameManager.instance.ItemDeactivate(gameObject);  
    }
}