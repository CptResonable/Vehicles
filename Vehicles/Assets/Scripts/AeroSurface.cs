using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AeroSurface : MonoBehaviour {

    public AeroConfig config;

    private float _flapAngle;
    private float _flapChord;
    private float _angleOfAttack;
    
    private Vector3 _lastPosition = Vector3.zero;
    private Vector3 _worldVelocity = Vector3.zero;
    
    // "Constants"
    private float _correctedLiftSlope;
    
    private void Awake() {
        
        // Calculate flap chord from flap fraction and chord length.
        _flapChord = config.chord * config.flapFraction;
        
        // C. Effect of Aspect Ratio
        _correctedLiftSlope = config.liftSlope * config.aspectRatio / (config.aspectRatio + 2 * (config.aspectRatio + 4) / (config.aspectRatio + 2));
    }

    private void FixedUpdate() {
        _worldVelocity = (transform.position - _lastPosition) * Time.fixedTime;
        _lastPosition = transform.position;
    }

    public void CalculateForces(Vector3 worldAirVelocity, float airDensity, out Vector3 force, out Vector3 torque) {
        
        // x = z;
        // z = y;

        // B. Angle of Attack
        _angleOfAttack = Mathf.Atan(_worldVelocity.y / worldAirVelocity.z);
        // _angleOfAttack = Mathf.Atan2(airVelocity.y, -airVelocity.x);
        
        // D. Effect of Control Surface Deflection
        float theta_f = Mathf.Acos(2 * _flapChord / config.chord - 1);
        float flapEffectivenessFactor = 1 - (theta_f - Mathf.Sin(theta_f)) / Mathf.PI;
        float flapEffectivenessCorrection = Mathf.Lerp(0.8f, 0.4f, (Mathf.Abs(_flapAngle) * Mathf.Rad2Deg - 10) / 50); // η, taken from JumpTrajectorys code, he calls it "flapEffectivenessCorrection"
        float deltaLift = _correctedLiftSlope * flapEffectivenessFactor * flapEffectivenessCorrection * _flapAngle;

        // Calculate coefficients
        float liftCoefficient, dragCoefficient, torqueCoefficient;
        CalculateCoefficients(_angleOfAttack, _correctedLiftSlope, config.zeroLiftAoA, config.stallAngleHigh, config.stallAngleLow,
            out liftCoefficient, out dragCoefficient, out torqueCoefficient);
        
        // Calculate forces
        Vector3 localAirVelocity = transform.InverseTransformDirection(worldAirVelocity); // Convert velocity to local space
        localAirVelocity = new Vector3(0, localAirVelocity.y, localAirVelocity.z); // Discard sideways movement
        Vector3 dragDir = transform.InverseTransformDirection(localAirVelocity.normalized);
        Vector3 liftDir = Vector3.Cross(dragDir, transform.right); // Get lift direction by getting the vector perpendicular to drag direction and -transform.right

        float dynamicAirPressure = airDensity * localAirVelocity.sqrMagnitude / 2;

        config.wingArea = 4;
        
        Vector3 lift = liftCoefficient * dynamicAirPressure * config.wingArea * liftDir;
        Vector3 drag = dragCoefficient * dynamicAirPressure * config.wingArea * dragDir;
        force = lift + drag;
        
        torque = torqueCoefficient * dynamicAirPressure * config.wingArea * config.chord * transform.right;
        torque += Vector3.Cross(transform.localPosition, force); // Add torque resulting from forces action offset from aircraft COM

        // force = Vector3.one * 10;
        // torque = Vector3.one * 10;

        // Vector3 lift = liftCoefficient * (airDensity * Vector3.Scale(worldAirVelocity, worldAirVelocity) / 2) *
        //                config.wingArea;
    }
    
    // Accounting for aspect ratio effect on lift coefficient.
    void CalculateCoefficients(float angleOfAttack, float correctedLiftSlope,float zeroLiftAoA, float stallAngleHigh, float stallAngleLow, 
        out float liftCoefficient, out float dragCoefficient,  out float torqueCoefficient) {

        liftCoefficient = correctedLiftSlope * (_angleOfAttack - zeroLiftAoA);
        
        float inducedAoA = liftCoefficient / (Mathf.PI * config.aspectRatio);
        float effectiveAoA = _angleOfAttack - zeroLiftAoA - inducedAoA;
        float tangentialCoefficient = config.skinFriction * Mathf.Cos(effectiveAoA);
        float normalCoefficient = (liftCoefficient + tangentialCoefficient * Mathf.Sin(effectiveAoA)) /
                                  Mathf.Cos(effectiveAoA);
        
        dragCoefficient = normalCoefficient * Mathf.Sin(effectiveAoA) + tangentialCoefficient * Mathf.Cos(effectiveAoA);
        torqueCoefficient = -normalCoefficient * (0.25f - 0.175f * (1 - 2 * effectiveAoA / Mathf.PI)); // !!
    }
    
    // float CalculateLift(float liftCoef, float airDensity, float airflowSpeed) {
    //     float lift = liftCoef * ((airDensity * Mathf.Pow(airflowSpeed, 2) / 2) * config.wingArea);
    //     return lift;
    // }
    //
    // float CalculateDrag(float dragCoef, float airDensity, float airflowSpeed) {
    //     float drag = dragCoef * ((airDensity * Mathf.Pow(airflowSpeed, 2) / 2) * config.wingArea);
    //     return drag;
    // }
    //
    // float CalculateTorque(float torqueCoef, float airDensity, float airflowSpeed) {
    //     float drag = torqueCoef * ((airDensity * Mathf.Pow(airflowSpeed, 2) / 2) * config.wingLength);
    //     return drag;
    // }
}
