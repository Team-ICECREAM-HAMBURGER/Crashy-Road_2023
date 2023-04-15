// VehicleController.cs
/* 
    플레이어 자동차의 조작을 제어합니다.
    앞/뒤/좌/우 이동 및 정지 등 자동차의 움직임과 관련된 기능들을 구현합니다.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour {
    public VehicleStatus vehicleStatus;
    
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
    [SerializeField] private LayerMask driveableLayer;

    private float carWheelRigidbodyRadius;
    private float sign;
    private float turnSpeedMultiplyer;
    private Vector3 carVelocity;
    private Vector3 rayOrigin;
    private Vector3 rayDirection;
    private RaycastHit hit;
    private float rayMaxDistance;



    private void Init() {
        this.carWheelRigidbodyRadius = this.carWheelRigidbody.GetComponent<SphereCollider>().radius;
    }

    private void Awake() {
        Init();
    }

    private void FixedUpdate() {
        if (GroundCheck()) {
            Move();
            Rotate();
        }
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
        if (VehicleInputManager.instance.verticalInput > 0.1f || this.carVelocity.z > 1) {  // 차가 앞으로 움직일 때, 좌/우
            this.carBodyRigidbody.AddTorque(Vector3.up * VehicleInputManager.instance.horizontalInput * this.sign * vehicleStatus.turnSpeed * this.turnSpeedMultiplyer * 100);
        }
        else if (VehicleInputManager.instance.verticalInput < -0.1f || this.carVelocity.z < -1) {   // 차가 뒤로 움직일 때, 우/좌
            this.carBodyRigidbody.AddTorque(Vector3.up * VehicleInputManager.instance.horizontalInput * this.sign * vehicleStatus.turnSpeed * this.turnSpeedMultiplyer * 100);
        }

        // 스키드 마크
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

        // 바퀴 회전
        foreach (Transform frontWheel in this.frontWheels) {
            frontWheel.localRotation = Quaternion.Slerp(frontWheel.localRotation, Quaternion.Euler(frontWheel.localRotation.eulerAngles.x, 30 * VehicleInputManager.instance.horizontalInput, frontWheel.localRotation.eulerAngles.z), 0.1f);
        }
    }

    private void Move() {
        if (Mathf.Abs(VehicleInputManager.instance.verticalInput) > 0.1f) {
            this.carWheelRigidbody.velocity = Vector3.Lerp(this.carWheelRigidbody.velocity, this.carBodyRigidbody.transform.forward * VehicleInputManager.instance.verticalInput * vehicleStatus.maxSpeed, vehicleStatus.accelaration / 10 * Time.deltaTime);
        }

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
}
