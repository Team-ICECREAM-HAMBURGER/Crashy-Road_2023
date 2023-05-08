using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraHovering : MonoBehaviour {
    [SerializeField] private float hoveringSpeed;

    
    private void Update() {
        if (transform.position.x < -110f) {
            return;
        } 
        
        transform.Translate(Vector3.left * this.hoveringSpeed * Time.deltaTime);
    }
}