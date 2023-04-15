// VehicleStatus.cs
/*
    플레이어 자동차의 사양을 ScriptableObject 형태로 저장 및 관리합니다.
    자동차의 가속력, 무개, 핸들링 등의 정보를 포함합니다.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vehicle Data", menuName = "ScriptableObjects/VehicleStatus", order = 1)]
public class VehicleStatus : ScriptableObject {
    public float maxSpeed;      // 최고 속도
    public float accelaration;  // 가속력
    public float turnSpeed;     // 회전 속도 (핸들링)
    public float gravity;       // 중력
    public float downforce;     // 다운 포스
}