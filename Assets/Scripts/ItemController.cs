using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour, IItem {
    enum Type {
        Repair,
        Mirror,
        SpeedUp
    }

    private Type itemType;
    

    private void OnEnable() {
        this.itemType = (Type)Random.Range(0, 3);     
    }

    public void Use(GameObject obj) {
        PlayerVehicleController pvc = obj.GetComponent<PlayerVehicleController>();

        switch (this.itemType) {
            case Type.Repair :
                int playerHp = pvc.hp;
                
                if (playerHp < 100) {
                    playerHp += 5;
                }

                break;
            case Type.Mirror :
                pvc.isMirror = true;
                break;
            case Type.SpeedUp :
                pvc.StartCoroutine("SpeedUp");
                break;
        }      

        GameManager.instance.ItemGet(this.itemType.ToString());
        GameManager.instance.ItemDeactive(gameObject);  
    }
}