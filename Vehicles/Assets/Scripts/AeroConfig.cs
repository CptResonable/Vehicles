using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AeroConfig {
    public float liftSlope = 6.28f;
    public float skinFriction = 0.02f;
    public float zeroLiftAoA = 0;
    public float stallAngleHigh = 15;
    public float stallAngleLow = -15;
    public float chord = 1;
    public float flapFraction = 0;
    public float span = 1;
    public bool autoAspectRatio = true;
    public float aspectRatio = 2;

    [HideInInspector] public float wingArea;
    //[HideInInspector] public float wingLength;
}