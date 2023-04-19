using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoliceAI : MonoBehaviour {
    public VehicleStatus vehicleStatus;
    
    [SerializeField] private Transform target;
    
    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider fl;
    [SerializeField] private WheelCollider fr;
    [SerializeField] private WheelCollider rl;
    [SerializeField] private WheelCollider rr;
    
    private NavMeshPath path;
    private Rigidbody rigidbody;
    private Vector3 direction;
    private float steer;
    private float sign;

    
    private void Init() {
        this.rigidbody = GetComponent<Rigidbody>();
        this.path = new NavMeshPath();
    }

    private void Awake() {
        Init();
    }

    private void FixedUpdate() {
        Pathing();
        Move();
        Rotate();

        // Debug
        for (int i = 0; i < path.corners.Length - 1; i++) {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }

    private void Move() {
        this.fl.motorTorque = vehicleStatus.maxSpeed;
        this.fr.motorTorque = vehicleStatus.maxSpeed;
        this.rr.motorTorque = vehicleStatus.maxSpeed;
        this.rl.motorTorque = vehicleStatus.maxSpeed;
    }

    private void Rotate() {
        if (this.path.corners.Length > 1) {
            this.direction = transform.position - this.path.corners[1];
        }
        else {
            this.direction = transform.position - this.target.position;
        }

        this.direction = gameObject.transform.InverseTransformDirection(this.direction);
        this.direction.Normalize();

        this.steer = -this.direction.x;

        this.fl.steerAngle = Mathf.Clamp(this.steer * 100, -45, 45);
        this.fr.steerAngle = Mathf.Clamp(this.steer * 100, -45, 45);
    }

    private void Pathing() {
        NavMesh.CalculatePath(transform.position, this.target.position, NavMesh.AllAreas, this.path);
    }

    // Debug
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + this.direction);
        
        if (path == null) {
            return;
        }

        for (int i = 0; i < path.corners.Length - 1; i++) {
            Gizmos.DrawWireSphere(path.corners[i], 2);
        }   
    }
}