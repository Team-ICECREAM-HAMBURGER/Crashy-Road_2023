using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpinner : MonoBehaviour {
    [SerializeField] private float speed;

    private void Update() {
        transform.Rotate(0, 1 * speed * Time.deltaTime, 0);
    }
}
