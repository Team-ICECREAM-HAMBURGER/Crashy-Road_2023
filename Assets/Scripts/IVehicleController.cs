using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVehicleController {
    void Init();
    void Movement();
    void Rotate();
    bool Crash();
    bool GroundCheck();
}