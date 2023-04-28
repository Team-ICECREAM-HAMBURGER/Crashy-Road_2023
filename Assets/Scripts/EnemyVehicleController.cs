// EnemyVehicleController.cs
/*
    플레이어를 추적하는 인공지능을 구현합니다.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVehicleController : MonoBehaviour {    
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
    [SerializeField] private GameObject fireParticle;
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private AudioSource explosionAudioSource;
    [SerializeField] private Light redLight;
    [SerializeField] private Light blueLight;

    private NavMeshPath path;
    private Rigidbody rb;
    private Vector3 direction;
    private Vector3 carVelocity;
    private float chaseDistance;
    private float resetDistance;
    private float pathSearchCoolTime;
    private float _torquePower;
    private bool isCrashed;
    public int hp { get; set; }


    private void Init() {
        this.target = GameObject.FindWithTag("Player").transform;
        this.rb = GetComponent<Rigidbody>();
        this.path = new NavMeshPath();
        this._torquePower = this.torquePower;
        this.hp = 10;
    }

    private void Start() {
        Init();
    }

    private void OnEnable() {
        StartCoroutine("Lighting");
    }

    private void OnDisable() {
        this.isCrashed = false;
        this.hp = 10;
        this.explosionParticle.gameObject.SetActive(false);
        this.explosionAudioSource.gameObject.SetActive(false);
    }

    private IEnumerator Lighting() {
        while (true) {
            this.redLight.enabled = true;
            this.blueLight.enabled = false;

            yield return new WaitForSeconds(0.3f);

            this.redLight.enabled = false;
            this.blueLight.enabled = true;

            yield return new WaitForSeconds(0.3f);
        }
    }

    private void FixedUpdate() {
        if (Grounded() && !this.isCrashed) {
            Pathing();
            Move();
            Rotate();
            SkidMark();
            Fire();

            StartCoroutine("Reset");
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
        if (this.chaseDistance < 7f) {
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
            this.chaseDistance = Vector3.Distance(transform.position, this.target.position);
        }
    }

    private void Fire() {
        if (this.hp <= 5) {
            this.fireParticle.SetActive(true);
        }
        else {
            this.fireParticle.SetActive(false);
        }
    }

    private IEnumerator Reset() {
        Vector3 positionA = transform.position;
            
        yield return new WaitForSeconds(3);
            
        Vector3 positionB = transform.position;

        if (Vector3.Distance(positionA, positionB) < 0.5f) {
            if (Vector3.Distance(transform.position, this.target.transform.position) > 15) {
                GameManager.instance.EnemyDeactive(gameObject);
            }
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

    private IEnumerator Crash() {
        if (this.hp <= 0 && !this.isCrashed) {

            Debug.Log("Crashed");
            
            this.isCrashed = true;
            this.explosionParticle.gameObject.SetActive(true);
            this.explosionAudioSource.gameObject.SetActive(true);

            GameManager.instance.ScoreUp(10);

            yield return new WaitForSeconds(5);
            
            GameManager.instance.EnemyDeactive(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.CompareTag("Obstacle")) {
            if (Mathf.Abs(this.carVelocity.z) > 20) {
                this.hp -= 2;
            }
        }
        else if (other.transform.CompareTag("Police") || other.transform.CompareTag("Player")) {
            if (Mathf.Abs(this.carVelocity.z) > 20) {
                this.hp -= 3;
            }
        }

        StartCoroutine("Crash");
    }
}
