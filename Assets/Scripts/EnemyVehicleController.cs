using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVehicleController : MonoBehaviour, IVehicleController {    
    [Header("Components")]
    [SerializeField] private LayerMask driveableLayer;
    [SerializeField] private AudioSource enemyAudioSource;
    [SerializeField] private EnemyVehicleStatus enemyVehicleStatus;
    
    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider fl;
    [SerializeField] private WheelCollider fr;
    [SerializeField] private WheelCollider rl;
    [SerializeField] private WheelCollider rr;
    
    [Header("Visuals")]
    [SerializeField] private TrailRenderer[] skidMarkTrails;
    [SerializeField] private ParticleSystem fireParticle;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private Light redLight;
    [SerializeField] private Light blueLight;
    
    [Header("Status")]
    [SerializeField] private bool isCrashed;
    [SerializeField] private bool isCornering;
    
    private Transform _target;  // 추적 대상
    private NavMeshPath _path;
    private Rigidbody _rb;
    private Vector3 _direction;
    private Vector3 _carVelocity;
    private float _pathSearchCoolTime;
    

    public void Init() {
        this._target = GameObject.FindWithTag("Player").transform;
        this._rb = gameObject.GetComponent<Rigidbody>();
        this._path = new NavMeshPath();
    }

    private void Start() {
        Init();
    }

    private void OnEnable() {   // Spawn
        StartCoroutine(nameof(Lighting));
        this.isCrashed = false;
        this.isCornering = false;
    }

    private void OnDisable() {  // Die
        this.isCrashed = false;
        this.isCornering = false;
    }

    private void FixedUpdate() {
        if (GroundCheck() && !Crash()) {
            PathFinding();
            Movement();
            Rotate();
        }
    }

    private void PathFinding() {
        this._pathSearchCoolTime += Time.deltaTime;

        if (this._pathSearchCoolTime > 0.3f) {
            this._pathSearchCoolTime = 0f;
            NavMesh.CalculatePath(transform.position, this._target.position, NavMesh.AllAreas, this._path);
        }
    }

    private IEnumerator Lighting() {
        while (!this.isCrashed) {
            this.redLight.enabled = true;
            this.blueLight.enabled = false;

            yield return new WaitForSeconds(0.3f);

            this.redLight.enabled = false;
            this.blueLight.enabled = true;

            yield return new WaitForSeconds(0.3f);
        }
    }
    
    public void Explosion() {
        GameManager.instance.ScoreUp(this.enemyVehicleStatus.score);    // 점수 카운트
        
        this.explosionParticle.Play();
        this.fireParticle.Play();
        
        this.enemyAudioSource.Stop();
        this.enemyAudioSource.PlayOneShot(this.explosionClip);
    }
    
    public void Movement() {
        if (!this.isCornering && !this.isCrashed) {   // 코너링 도중이 아니며, 사고가 나지 않았을 경우 -> 기속
            this.fl.motorTorque = this.enemyVehicleStatus.torquePower; 
            this.fr.motorTorque = this.enemyVehicleStatus.torquePower;
            this.rr.motorTorque = this.enemyVehicleStatus.torquePower;
            this.rl.motorTorque = this.enemyVehicleStatus.torquePower;
        }
    }

    public void Rotate() {
        if (this._path.corners.Length > 1) {    // 코너가 2개 이상 존재할 경우,
            this._direction = transform.position - this._path.corners[1];

            if (this._rb.velocity.magnitude > this.enemyVehicleStatus.turnSpeed &&
                Vector3.Distance(transform.position, this._path.corners[1]) < 30) {
                this.isCornering = true;
                this.fl.brakeTorque = this.enemyVehicleStatus.torquePower * 5;
                this.fr.brakeTorque = this.enemyVehicleStatus.torquePower * 5;
                this.rl.brakeTorque = this.enemyVehicleStatus.torquePower * 5;
                this.rr.brakeTorque = this.enemyVehicleStatus.torquePower * 5;
            }
            else {
                this.isCornering = false;
                this.fl.brakeTorque = 0;
                this.fr.brakeTorque = 0;
                this.rl.brakeTorque = 0;
                this.rr.brakeTorque = 0;
            }
        }
        else {
            this._direction = transform.position - this._target.position;
        }

        this._direction = transform.InverseTransformDirection(this._direction);
        this._direction.Normalize();

        // 스키드 마크 효과
        this._carVelocity = this._rb.transform.InverseTransformDirection(this._rb.velocity);
        
        if (Mathf.Abs(this._carVelocity.x) > 10) {
            foreach (TrailRenderer skid in this.skidMarkTrails) {
                skid.emitting = true;
            }
        }
        else {
            foreach (TrailRenderer skid in this.skidMarkTrails) {
                skid.emitting = false;
            }
        }
        
        // 바퀴 회전
        this.fl.steerAngle = Mathf.Clamp(-this._direction.x * 100, -45, 45);
        this.fr.steerAngle = Mathf.Clamp(-this._direction.x * 100, -45, 45);
    }

    public bool Crash() {
        if (this.isCrashed) {
            this.fl.brakeTorque = this.enemyVehicleStatus.torquePower;
            this.fr.brakeTorque = this.enemyVehicleStatus.torquePower;
            this.rl.brakeTorque = this.enemyVehicleStatus.torquePower;
            this.rr.brakeTorque = this.enemyVehicleStatus.torquePower;

            foreach (TrailRenderer skid in this.skidMarkTrails) {
                skid.Clear();
            }
            
            return true;
        }
        else {
            return false;
        }
    }

    public bool GroundCheck() {
        return Physics.Raycast(transform.position, -transform.up, 2f, this.driveableLayer);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.CompareTag("Obstacle") || other.transform.CompareTag("Player") || other.transform.CompareTag("Police")) {
            if (!this.isCrashed) {
                Explosion();
            }
            
            this.isCrashed = true;
        }
    }
}
