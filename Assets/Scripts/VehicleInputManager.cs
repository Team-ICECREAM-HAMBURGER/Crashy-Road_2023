// VehicleInputManager.cs
/*
    차량 조작에 필요한 Input 값을 인식해 전달합니다.
    싱글턴 방식을 사용하여 구현합니다.
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleInputManager : MonoBehaviour {
    public static VehicleInputManager instance;

    public float horizontalInput { get; private set; }
    public float verticalInput { get; private set; }

    
    private void Init() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Awake() {
        Init();
    }

    private void Update() {
        this.horizontalInput = Input.GetAxis("Horizontal");
        this.verticalInput = Input.GetAxis("Vertical");
    }
}
