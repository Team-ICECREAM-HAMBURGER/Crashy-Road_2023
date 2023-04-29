// PlayerVehicleController.cs
/* 
    플레이어 자동차의 조작을 제어합니다.
    앞/뒤/좌/우 이동 및 정지 등 자동차의 움직임과 관련된 기능들을 구현합니다.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerVehicleController : MonoBehaviour {
    public VehicleStatus vehicleStatus;
    
    public int hp { get; set; }
    public bool isShild { get; set; }

    [SerializeField] private LayerMask driveableLayer;
    
    [Header("Physics Settings")]
    [SerializeField] private Rigidbody carWheelRigidbody;
    [SerializeField] private Rigidbody carBodyRigidbody;
    [SerializeField] private PhysicMaterial frictionMaterial;

    [Header("Animation Curve Settings")]
    [SerializeField] private AnimationCurve frictionCurve;
    [SerializeField] private AnimationCurve turnCurve;

    [Header("Visuals")]
    [SerializeField] private Transform[] frontWheels;
    [SerializeField] private TrailRenderer[] skidMarkTrails;
    [SerializeField] private GameObject fireParticle;
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private AudioSource explosionAudioSource;
    [SerializeField] private AudioSource skidAudioSource;    

    private float sign;
    private float turnSpeedMultiplyer;
    private float moveSpeedMultiplyer = 1;
    private float rayMaxDistance;

    private Vector3 carVelocity;
    private Vector3 rayOrigin;
    private Vector3 rayDirection;
    private RaycastHit hit;


    private void Start() {
        this.hp = 1;
        this.virtualCamera.m_Lens.FieldOfView = 90;
    }

    private void FixedUpdate() {
        if (GroundCheck() && !Crash()) {
            Move();
            Rotate();
        }
    }

    public IEnumerator Explosion() {
        this.explosionParticle.SetActive(true);
        this.fireParticle.SetActive(true);

        this.virtualCamera.m_Lens.FieldOfView = 30;
        this.explosionAudioSource.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        this.skidAudioSource.gameObject.SetActive(false);

        GameManager.instance.GameOver();
    }

    private void Rotate() {
        this.carVelocity = this.carBodyRigidbody.transform.InverseTransformDirection(this.carBodyRigidbody.velocity);
        this.sign = Mathf.Sign(this.carVelocity.z);
        this.turnSpeedMultiplyer = this.turnCurve.Evaluate(this.carVelocity.magnitude / 100);

        // 마찰 계수 조정
        if (Mathf.Abs(this.carVelocity.x) > 0) {
            this.frictionMaterial.dynamicFriction = this.frictionCurve.Evaluate(Mathf.Abs(this.carVelocity.x / 100));
        }

        // 차량 회전
        if (PlayerInputManager.instance.verticalInput > 0.1f || this.carVelocity.z > 1) {  // 차가 앞으로 움직일 때, 좌/우
            this.carBodyRigidbody.AddTorque(Vector3.up * PlayerInputManager.instance.horizontalInput * this.sign * vehicleStatus.turnSpeed * this.turnSpeedMultiplyer * 100);
        }
        else if (PlayerInputManager.instance.verticalInput < -0.1f || this.carVelocity.z < -1) {   // 차가 뒤로 움직일 때, 우/좌
            this.carBodyRigidbody.AddTorque(Vector3.up * PlayerInputManager.instance.horizontalInput * this.sign * vehicleStatus.turnSpeed * this.turnSpeedMultiplyer * 100);
        }

        // 스키드 마크
        if (Mathf.Abs(this.carVelocity.x) > 10) {
            foreach (TrailRenderer skid in this.skidMarkTrails) {
                skid.emitting = true;
                this.skidAudioSource.gameObject.SetActive(true);
            }
        }
        else {
            foreach (TrailRenderer skid in this.skidMarkTrails) {
                skid.emitting = false;
                this.skidAudioSource.gameObject.SetActive(false);
            }
        }

        // 바퀴 회전
        foreach (Transform frontWheel in this.frontWheels) {
            frontWheel.localRotation = Quaternion.Slerp(frontWheel.localRotation, Quaternion.Euler(frontWheel.localRotation.eulerAngles.x, 30 * PlayerInputManager.instance.horizontalInput, frontWheel.localRotation.eulerAngles.z), 0.1f);
        }
    }

    private void Move() {
        this.carWheelRigidbody.velocity = Vector3.Lerp(this.carWheelRigidbody.velocity, this.carBodyRigidbody.transform.forward * 1 * vehicleStatus.maxSpeed * this.moveSpeedMultiplyer, vehicleStatus.accelaration / 10 * Time.deltaTime);

        // Downforce
        this.carWheelRigidbody.AddForce(-transform.up * vehicleStatus.downforce * this.carWheelRigidbody.mass);
    }

    private bool GroundCheck() {
        this.rayOrigin = this.carWheelRigidbody.transform.position;
        this.rayMaxDistance = this.carWheelRigidbody.GetComponent<SphereCollider>().radius + 0.2f;
        this.rayDirection = -transform.up;

        if (Physics.Raycast(this.rayOrigin, this.rayDirection, out this.hit, this.rayMaxDistance, this.driveableLayer)) {
            return true;
        }
        else {
            return false;
        }
    }

    private bool Crash() {
        if (this.hp <= 0) {
            StartCoroutine("Explosion");
            return true;
        }
        else {
            return false;
        }
    }

    private IEnumerator SpeedUp() {
        this.moveSpeedMultiplyer = 1.2f;

        yield return new WaitForSeconds(5f);

        this.moveSpeedMultiplyer = 1;
    }

    private IEnumerator Shild() {
        this.isShild = true;

        yield return new WaitForSeconds(3f);

        this.isShild = false;
    }

    private void OnCollisionEnter(Collision other) {
        if (!this.isShild) {
            if (other.transform.CompareTag("Obstacle")) {
                this.hp -= 2;
            }
            else if (other.transform.CompareTag("Police")) {
                this.hp -= 3;
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
