using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour, IItem {
    enum Type {
        Shild,
        SpeedUp
    }

    private Type itemType;
    

    private void OnEnable() {
        this.itemType = (Type)Random.Range(0, 2);     
    }

    public void Use(GameObject obj) {
        PlayerVehicleController pvc = obj.GetComponent<PlayerVehicleController>();

        switch (this.itemType) {
            case Type.Shild :
                pvc.StartCoroutine("Shild");
                break;
            case Type.SpeedUp :
                pvc.StartCoroutine("SpeedUp");
                break;
        }      

        GameManager.instance.ItemGet(this.itemType.ToString());
        GameManager.instance.ItemDeactive(gameObject);  
    }
}