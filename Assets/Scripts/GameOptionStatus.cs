using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Option Data", menuName = "ScriptableObjects/GameOptionStatus", order = 1)]
public class GameOptionStatus : ScriptableObject
{
    public int screenSize;
    public int fpsValue;
    public int vsyncValue;
    public int graphicQuality;
}
