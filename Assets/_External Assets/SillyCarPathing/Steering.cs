using UnityEngine;
using SillyCarPathing;

namespace SillyCarPathing
{
    public class Steering : MonoBehaviour
    {
        public WheelCollider FR;
        public WheelCollider FL;
        public WheelCollider RR;
        public WheelCollider RL;
        public Pathing pathing;
        public Vector3 Direction;
        public float Throttle;
        public Transform MainCarParent;
        public float WheelDrive;
        public Rigidbody Rigidbody;
        public bool Cornering;
        public float DotProd;
        public float CorneringSpeed;
        // Update is called once per frame
        void Update()
        {
            // Target acquisition and turning
            if (pathing.path.corners.Length > 1)
            {
                Direction = transform.position - pathing.path.corners[1];
            }
            else
            {
                Direction = transform.position - pathing.target.position;
            }
            Direction = MainCarParent.InverseTransformDirection(Direction);
            Direction.Normalize();
            float steer = -Direction.x;
            FR.steerAngle = Mathf.Clamp(steer * 100, -45, 45);
            FL.steerAngle = Mathf.Clamp(steer * 100, -45, 45);
            if (Physics.Raycast(transform.position, transform.forward, 5))
            {
                WheelDrive = 4000;
            }
            // Apply torque to each of the wheels, the car is a AWD setup
            FR.motorTorque = Throttle - WheelDrive;
            FL.motorTorque = Throttle - WheelDrive;
            RR.motorTorque = Throttle - WheelDrive;
            RL.motorTorque = Throttle - WheelDrive;
/*
            // Quick check for appropriate path length
            if (pathing.path.corners.Length > 1)
            {
                if (Vector3.Distance(transform.position, pathing.path.corners[1]) < 45)
                {
                    if (pathing.path.corners.Length > 2)
                    {
                        Vector3 initline;
                        Vector3 nextline;
                        initline = pathing.path.corners[1] - pathing.path.corners[0];
                        nextline = pathing.path.corners[2] - pathing.path.corners[1];
                        DotProd = 1 - Vector3.Dot(nextline.normalized, initline.normalized);
                    }
                    else
                    {
                        DotProd = 0;
                    }
                    WheelDrive = 1800 * DotProd;
                    Cornering = true;
                }
                else
                {
                    WheelDrive = 0;
                    Cornering = false;
                }
            }
            // Apply brakes if above a certain speed, set by cornering speed and the intensity of the turn determined by the dot product
            if (Rigidbody.velocity.magnitude > CorneringSpeed * (1 - DotProd) && Cornering)
            {
                RR.brakeTorque = 3000;
                RL.brakeTorque = 3000;
                FR.brakeTorque = 3000;
                FL.brakeTorque = 3000;
            }
            else
            {
                RR.brakeTorque = 0;
                RL.brakeTorque = 0;
                FR.brakeTorque = 0;
                FL.brakeTorque = 0;
            }
*/
        }
    
        void FixedUpdate()
        {
            // Apply downforce to make sure the car doesn't roll or introduce strange behavior in how it turns
            Rigidbody.AddForce(-transform.up * 8, ForceMode.Acceleration);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Direction);
        }
    }
}
