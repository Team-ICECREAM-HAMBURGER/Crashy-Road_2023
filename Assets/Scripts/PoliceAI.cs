using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoliceAI : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private NavMeshPath navMeshPath;

    private float pathUpdateTime;

    private void Init() {
        this.navMeshPath = new NavMeshPath();
        this.pathUpdateTime = 0.0f;
    }

    private void Awake() {
        Init();
    }

    private void Update() {
        Pathing();
    }

    private void FixedUpdate() {
        Chasing();
    }

    private void Pathing() {
        this.pathUpdateTime += Time.deltaTime;

        if (this.pathUpdateTime > 0.1f) {
            this.pathUpdateTime = 0.0f;
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, this.navMeshPath);
        }
    }

    private void Chasing() {
        
    }
}