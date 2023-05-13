using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Enemy Vehicle Data", menuName = "ScriptableObjects/EnemyVehicleStatus", order = 2)]
public class EnemyVehicleStatus : ScriptableObject {
    public float torquePower;   // 가속력
    public float turnSpeed;     // 회전 속도 (핸들링)
    public float gravity;       // 중력
    public float downforce;     // 다운 포스
    public int score;           // 점수
}