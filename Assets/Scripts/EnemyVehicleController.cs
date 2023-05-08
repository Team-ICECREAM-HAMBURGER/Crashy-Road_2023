using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVehicleController : MonoBehaviour {    
    [SerializeField] private Transform target;  // 추적 대상
    [SerializeField] private float torquePower;
    [SerializeField] private LayerMask driveableLayer;

    [SerializeField] private float corneringSpeed;

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

    private NavMeshPath _path;
    private Rigidbody _rb;
    private Vector3 _direction;
    private Vector3 _carVelocity;
    private float _chaseDistance;
    private float _resetDistance;
    private float _pathSearchCoolTime;
    private bool _isCrashed;
    private bool _isCornering;
    public int Hp { get; set; }


    private void Init() {
        this.target = GameObject.FindWithTag("Player").transform;
        this._rb = GetComponent<Rigidbody>();
        this._path = new NavMeshPath();
        this.Hp = 1;
    }

    private void Start() {
        Init();
    }

    private void OnEnable() {
        StartCoroutine(nameof(Lighting));
    }

    private void OnDisable() {
        this._isCrashed = false;
        this.Hp = 1;
        this.explosionParticle.gameObject.SetActive(false);
        this.fireParticle.gameObject.SetActive(false);
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
        if (Grounded() && !this._isCrashed) {
            Pathing();
            Move();
            Rotate();
            SkidMark();

            StartCoroutine(nameof(Reset));
        }
        for (int i = 0; i < _path.corners.Length - 1; i++)
            Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);
    }

    private bool Grounded()
    {
        return Physics.Raycast(transform.position, -transform.up, 2f, this.driveableLayer);
    }

    private void Move() {
        if (!this._isCornering || !this._isCrashed) {
            this.fl.motorTorque = this.torquePower;
            this.fr.motorTorque = this.torquePower;
            this.rr.motorTorque = this.torquePower;
            this.rl.motorTorque = this.torquePower;
        }
        else if (this._isCrashed) {
            this.fl.brakeTorque = this.torquePower * 5;
            this.fr.brakeTorque = this.torquePower * 5;
            this.rr.brakeTorque = this.torquePower * 5;
            this.rl.brakeTorque = this.torquePower * 5;
        }
    }

    private void Rotate() {
        if (this._path.corners.Length > 1) {
            this._direction = transform.position - this._path.corners[1];
            if (this._rb.velocity.magnitude > this.corneringSpeed && Vector3.Distance(transform.position, this._path.corners[1]) < 30) {
                this._isCornering = true;
                this.fl.brakeTorque = this.torquePower * 5;
                this.fr.brakeTorque = this.torquePower * 5;
                this.rr.brakeTorque = this.torquePower * 5;
                this.rl.brakeTorque = this.torquePower * 5;
            }
            else {
                this._isCornering = false;
                this.fl.brakeTorque = 0;
                this.fr.brakeTorque = 0;
                this.rr.brakeTorque = 0;
                this.rl.brakeTorque = 0;
            }
        }
        else {
            this._direction = transform.position - this.target.position;
        }

        this._direction = gameObject.transform.InverseTransformDirection(this._direction);
        this._direction.Normalize();

        this.fl.steerAngle = Mathf.Clamp(-this._direction.x * 100, -45, 45);
        this.fr.steerAngle = Mathf.Clamp(-this._direction.x * 100, -45, 45);
    }

    private void Pathing() {
        this._pathSearchCoolTime += Time.deltaTime;
        
        if (this._pathSearchCoolTime > 0.3f) {
            this._pathSearchCoolTime = 0;

            NavMesh.CalculatePath(transform.position, this.target.position, NavMesh.AllAreas, this._path);
            this._chaseDistance = Vector3.Distance(transform.position, this.target.position);
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
    }

    private IEnumerator Crash() {
        if (this.Hp <= 0 && !this._isCrashed) {            
            this._isCrashed = true;
            this.explosionParticle.gameObject.SetActive(true);
            this.fireParticle.SetActive(true);

            this.explosionAudioSource.gameObject.SetActive(true);

            GameManager.instance.ScoreUp(10);

            yield return new WaitForSeconds(5);
            
            GameManager.instance.EnemyDeactive(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.CompareTag("Obstacle")) {
            this.Hp -= 2;
        }
        else if (other.transform.CompareTag("Police") || other.transform.CompareTag("Player")) {
            this.Hp -= 3;
        }

        StartCoroutine(nameof(Crash));
    }

    private void OnDrawGizmos()
    {
        // Draw spheres at every corner for debugging
        if (_path == null) return;
        for (int i = 0; i < _path.corners.Length - 1; i++)
            Gizmos.DrawWireSphere(_path.corners[i], 2);
    }
}
