using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "Player Vehicle Data", menuName = "ScriptableObjects/PlayerVehicleStatus", order = 1)]
public class PlayerVehicleStatus : ScriptableObject {
    public float maxSpeed;      // 최고 속도
    public float acceleration;  // 가속력
    public float turnSpeed;     // 회전 속도 (핸들링)
    public float gravity;       // 중력
    public float downforce;     // 다운 포스
}