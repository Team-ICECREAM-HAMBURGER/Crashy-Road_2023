using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHovering : MonoBehaviour {
    [SerializeField] private float hoversingSpeed;

    
    private void Update() {
        if (transform.position.x < -110f) {
            return;
        } 
        
        transform.Translate(Vector3.left * this.hoversingSpeed * Time.deltaTime);
    }
}