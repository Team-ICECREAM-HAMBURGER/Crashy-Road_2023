using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerVehicleController : MonoBehaviour, IVehicleController {
    [Header("Physics Settings")]
    [SerializeField] private Rigidbody carWheelRigidbody;
    [SerializeField] private Rigidbody carBodyRigidbody;
    [SerializeField] private PhysicMaterial frictionMaterial;

    [Header("Animation Curve Settings")]
    [SerializeField] private AnimationCurve frictionCurve;
    [SerializeField] private AnimationCurve turnCurve;

    [Header("Components")]
    [SerializeField] private LayerMask driveableLayer;
    [SerializeField] private PlayerVehicleStatus playerVehicleStatus;     // ScriptableObject
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private AudioSource playerAudioSource;

    [Header("Visuals")]
    [SerializeField] private Transform[] frontWheels;
    [SerializeField] private TrailRenderer[] skidMarkTrails;
    [SerializeField] private ParticleSystem fireParticle;
    [SerializeField] private ParticleSystem explosionParticle; 
    [SerializeField] private ParticleSystem itemSpeedUpParticle;
    [SerializeField] private ParticleSystem itemShieldParticle;
    [SerializeField] private AudioClip itemGetClip;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip driftClip;

    [Header("Status")]
    [SerializeField] private bool isShield;
    [SerializeField] private bool isSpeedUp; 
    [SerializeField] private bool isCrashed;

    private float _turnCurveValue;
    private float _turnSpeedMultiplier;
    private float _moveSpeedMultiplier;
    private float _rayMaxDistance;
    private float _horizontalInput;
    private Vector3 _carVelocity;
    private Vector3 _rayOrigin;
    private Vector3 _rayDirection;
    private RaycastHit _hit;

    public ParticleSystem ItemSpeedUpParticle {
        get { return this.itemSpeedUpParticle; }
    }
    public ParticleSystem ItemShieldParticle {
        get { return this.itemShieldParticle; }
    }
    
    public bool IsShield {
        get {
            return this.isShield;
        }
        set {
            this.isShield = value;
        }
    }
    public bool IsSpeedUp {
        get {
            return this.isSpeedUp;
        }
        set {
            this.isSpeedUp = value;
        }
    }
    public float MoveSpeedMultiplier {
        get {
            return this._moveSpeedMultiplier;
        }
        set {
            this._moveSpeedMultiplier = value;
        }
    }
    public float TurnSpeedMultiplier {
        get {
            return this._turnSpeedMultiplier;
        }
        set {
            this._turnSpeedMultiplier = value;
        }
    }
    

    public void Init() {
        this.virtualCamera.m_Lens.FieldOfView = 90f;
        this._turnSpeedMultiplier = 1f;
        this._moveSpeedMultiplier = 1f;
        this._rayMaxDistance = this.carWheelRigidbody.GetComponent<SphereCollider>().radius + 0.2f;
        this._rayDirection = -transform.up;
    }

    private void Start() {
        Init();
    }

    private void FixedUpdate() {
        if (GroundCheck() && !Crash()) {     // 차가 땅에 붙어 있을 때 && 사고가 나지 않았을 때 -> 자동차 조작 가능
            Movement();
            Rotate();
        }
    }

    public void Movement() {
        this.carWheelRigidbody.velocity = Vector3.Lerp(this.carWheelRigidbody.velocity, 
            this.carBodyRigidbody.transform.forward * (this.playerVehicleStatus.maxSpeed * this._moveSpeedMultiplier), 
            this.playerVehicleStatus.acceleration * Time.deltaTime);
        this.carWheelRigidbody.AddForce(-transform.up * (this.playerVehicleStatus.downforce * this.carWheelRigidbody.mass));
    }

    public void Rotate() {
        this._horizontalInput = Input.GetAxis("Horizontal");
        this._carVelocity = this.carBodyRigidbody.transform.InverseTransformDirection(this.carBodyRigidbody.velocity);
        this._turnCurveValue = this.turnCurve.Evaluate(Mathf.Abs(this._carVelocity.magnitude / 100)) * 100;

        if (Mathf.Abs(this._carVelocity.x) > 0) {    // 차가 좌/우로 회전하고 있을 때 -> 마찰 계수 조정
            this.frictionMaterial.dynamicFriction = this.frictionCurve.Evaluate(Mathf.Abs(this._carVelocity.x / 100));
        }

        if (this._carVelocity.z > 1) {  // 차가 전진하고 있을 때 -> 좌/우 회전
            this.carBodyRigidbody.AddTorque(Vector3.up * (this._horizontalInput * this.playerVehicleStatus.turnSpeed * this._turnCurveValue * this._turnSpeedMultiplier));
        }

        foreach (Transform frontWheel in this.frontWheels) {    // 앞바퀴 회전 표현
            frontWheel.localRotation = Quaternion.Slerp(frontWheel.localRotation, Quaternion.Euler(frontWheel.localRotation.eulerAngles.x, 30 * this._horizontalInput, frontWheel.localRotation.eulerAngles.z), 0.1f);
        }
        
        // 스키드 마크 효과
        if (Mathf.Abs(this._carVelocity.x) > 10) {
            foreach (TrailRenderer skid in this.skidMarkTrails) {
                skid.emitting = true;

                if (!this.playerAudioSource.isPlaying) {
                    this.playerAudioSource.PlayOneShot(this.driftClip);
                }
            }
        }
        else {
            foreach (TrailRenderer skid in this.skidMarkTrails) {
                skid.emitting = false;

                if (this.playerAudioSource.isPlaying) {
                    this.playerAudioSource.Stop();
                }
            }
        }
    }

    public bool Crash() {
        if (this.isCrashed) {
            this._moveSpeedMultiplier = 0;
            GameManager.instance.IsGameOver = true;
            return true;
        }
        else {
            return false;
        }
    }

    public bool GroundCheck() {
        this._rayOrigin = this.carWheelRigidbody.transform.position;

        if (Physics.Raycast(this._rayOrigin, this._rayDirection, out this._hit, this._rayMaxDistance, this.driveableLayer)) {
            return true;
        }
        else {
            foreach (TrailRenderer skid in this.skidMarkTrails) {
                skid.Clear();
            }
            return false;
        }
    }

    private IEnumerator Explosion() {
        this.explosionParticle.Play();
        this.fireParticle.Play();

        this.virtualCamera.m_Lens.FieldOfView = 30;
        
        this.playerAudioSource.Stop();
        this.playerAudioSource.PlayOneShot(this.explosionClip);
        
        yield return new WaitForSeconds(3f);

        this.explosionParticle.Stop();
        this.fireParticle.Stop();
        
        GameManager.instance.GameOver();
    }

    private void OnCollisionEnter(Collision other) {
        if (!this.isShield) {
            if (other.transform.CompareTag("Obstacle") || other.transform.CompareTag("Police")) {
                if (!this.isCrashed) { 
                    StartCoroutine(nameof(Explosion));
                }
                
                this.isCrashed = true;
            }
        }
        else if (this.isShield) {
            if (other.transform.CompareTag("Obstacle")) {
                if (!this.isCrashed) { 
                    StartCoroutine(nameof(Explosion));
                }
                
                this.isCrashed = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Item")) {
            ItemController item = other.GetComponent<ItemController>();
            item.Use(gameObject);
            
            this.playerAudioSource.PlayOneShot(this.itemGetClip);
        }
    }
}