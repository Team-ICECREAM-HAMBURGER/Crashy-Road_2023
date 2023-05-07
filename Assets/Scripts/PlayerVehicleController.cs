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
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip driftClip;

    [Header("Items")]
    [SerializeField] private bool isShield;
    [SerializeField] private bool isSpeedUp;


    private bool isCrashed;
    private float turnSpeedMultiplyer;
    private float moveSpeedMultiplyer;
    private float rayMaxDistance;
    private float horizontalInput;
    private Vector3 carVelocity;
    private Vector3 rayOrigin;
    private Vector3 rayDirection;
    private RaycastHit hit;


    public void Init() {
        this.virtualCamera.m_Lens.FieldOfView = 90;
        this.turnSpeedMultiplyer = 1;
        this.moveSpeedMultiplyer = 1;
        this.rayMaxDistance = this.carWheelRigidbody.GetComponent<SphereCollider>().radius + 0.2f;
        this.rayDirection = -transform.up;
        this.isCrashed = false;
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
                                                       this.carBodyRigidbody.transform.forward * this.playerVehicleStatus.maxSpeed * this.moveSpeedMultiplyer, 
                                                       this.playerVehicleStatus.accelaration * Time.deltaTime);
        this.carWheelRigidbody.AddForce(-transform.up * this.playerVehicleStatus.downforce * this.carWheelRigidbody.mass);
    }

    public void Rotate() {
        this.horizontalInput = Input.GetAxis("Horizontal");
        

        this.carVelocity = this.carBodyRigidbody.transform.InverseTransformDirection(this.carBodyRigidbody.velocity);
        this.turnSpeedMultiplyer = this.turnCurve.Evaluate(Mathf.Abs(this.carVelocity.magnitude / 100)) * 100;

        if (Mathf.Abs(this.carVelocity.x) > 0) {    // 차가 좌/우로 회전하고 있을 때 -> 마찰 계수 조정
            this.frictionMaterial.dynamicFriction = this.frictionCurve.Evaluate(Mathf.Abs(this.carVelocity.x / 100));
        }

        if (this.carVelocity.z > 1) {  // 차가 전진하고 있을 때 -> 좌/우 회전
            this.carBodyRigidbody.AddTorque(Vector3.up * this.horizontalInput * this.playerVehicleStatus.turnSpeed * this.turnSpeedMultiplyer);
        }

        foreach (Transform frontWheel in this.frontWheels) {    // 앞바퀴 회전 표현
            frontWheel.localRotation = Quaternion.Slerp(frontWheel.localRotation, Quaternion.Euler(frontWheel.localRotation.eulerAngles.x, 30 * this.horizontalInput, frontWheel.localRotation.eulerAngles.z), 0.1f);
        }
        
        // 스키드 마크 효과
        if (Mathf.Abs(this.carVelocity.x) > 10) {
            foreach(TrailRenderer skid in this.skidMarkTrails) {
                skid.emitting = true;
                // TODO: 드리프트 사운드 클립 재생
            }
        }
        else {
            foreach(TrailRenderer skid in this.skidMarkTrails) {
                skid.emitting = false;
                // TODO: 드리프트 사운드 클립 정지
            }
        }
    }

    public bool Crash() {
        if (this.isCrashed) {
            StartCoroutine("Explosion");
            return true;
        }
        else {
            return false;
        }
    }

    public bool GroundCheck() {
        this.rayOrigin = this.carWheelRigidbody.transform.position;

        if (Physics.Raycast(this.rayOrigin, this.rayDirection, out this.hit, this.rayMaxDistance, this.driveableLayer)) {
            return true;
        }
        else {
            return false;
        }
    }

    private IEnumerator Explosion() {
        this.explosionParticle.Play();
        this.fireParticle.Play();

        this.virtualCamera.m_Lens.FieldOfView = 30;
        // TODO: 폭발 사운드 클립 재생
        // TODO: 드리프트 사운드 클립 정지

        yield return new WaitForSeconds(3f);

        this.explosionParticle.Stop();
        this.fireParticle.Stop();
        
        GameManager.instance.GameOver();
    }

    private void OnCollisionEnter(Collision other) {
        if (!this.isShield) {
            if (other.transform.CompareTag("Obstacle") || other.transform.CompareTag("Police")) {
                this.isCrashed = true;
            }
            else {
                this.isCrashed = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        IItem item = other.GetComponent<IItem>();

        if (item != null) {
            item.Use(gameObject);
        }
    }
}
