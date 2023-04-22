// AIVehicleController.cs
/*
    플레이어를 추적하는 인공지능을 구현합니다.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIVehicleController : MonoBehaviour {    
    [SerializeField] private Transform target;  // 추적 대상
    [SerializeField] private float torquePower;
    [SerializeField] private LayerMask driveableLayer;
    
    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider fl;
    [SerializeField] private WheelCollider fr;
    [SerializeField] private WheelCollider rl;
    [SerializeField] private WheelCollider rr;
    
    [Header("Visuals")]
    [SerializeField] private Transform[] frontWheels;
    [SerializeField] private TrailRenderer[] skidMarkTrails;

    private NavMeshPath path;
    private Rigidbody rb;
    private Vector3 direction;
    private Vector3 carVelocity;
    private float distance;
    private float pathSearchCoolTime;
    private float _torquePower;

    
    private void Init() {
        this.target = GameObject.FindWithTag("Player").transform;
        this.rb = GetComponent<Rigidbody>();
        this.path = new NavMeshPath();
        this._torquePower = this.torquePower;
    }

    private void Awake() {
        Init();
    }

    private void FixedUpdate() {
        if (Grounded()) {
            Pathing();
            Move();
            Rotate();
            SkidMark();
        }

        // Debug
        for (int i = 0; i < path.corners.Length - 1; i++) {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }

    private bool Grounded() {
        if (Physics.Raycast(transform.position, -transform.up, 2f, this.driveableLayer)) {
            return true;
        }
        else {
            return false;
        }
    }

    private void Move() {
        if (this.distance < 7f) {
            this.torquePower = 0;
        }
        else {
            this.torquePower = this._torquePower;
        }

        this.fl.motorTorque = this.torquePower;
        this.fr.motorTorque = this.torquePower;
        this.rr.motorTorque = this.torquePower;
        this.rl.motorTorque = this.torquePower;
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

        this.fl.steerAngle = Mathf.Clamp(-this.direction.x * 100, -45, 45);
        this.fr.steerAngle = Mathf.Clamp(-this.direction.x * 100, -45, 45);
    }

    private void Pathing() {
        this.pathSearchCoolTime += Time.deltaTime;
        
        if (this.pathSearchCoolTime > 0.3f) {
            this.pathSearchCoolTime = 0;

            NavMesh.CalculatePath(transform.position, this.target.position, NavMesh.AllAreas, this.path);
            this.distance = Vector3.Distance(transform.position, this.target.position);
        }
    }

    private void SkidMark() {
        this.carVelocity = this.rb.transform.InverseTransformDirection(this.rb.velocity);

        if (Mathf.Abs(this.carVelocity.x) > 10) {
            foreach (TrailRenderer skid in this.skidMarkTrails) {
                skid.emitting = true;
            }
        }
        else {
            foreach (TrailRenderer skid in this.skidMarkTrails) {
                skid.emitting = false;
            }
        }
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
