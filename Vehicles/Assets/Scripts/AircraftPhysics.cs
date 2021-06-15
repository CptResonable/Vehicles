using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftPhysics : MonoBehaviour {
    [SerializeField] private AeroSurface[] surfaces;
    [SerializeField] private Vector3 testTorque;
    
    private Rigidbody _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {

        Vector3 totalForce = Vector3.zero, totalTorque = Vector3.zero;
        for (int i = 0; i < surfaces.Length; i++) {
            Vector3 force, torque;
            surfaces[i].CalculateForces(_rb.velocity, 1.225f, out force, out torque);
            totalForce += force;
            totalTorque += torque;
        }
        
        //_rb.AddForce(totalForce);
        _rb.AddTorque(totalTorque);
        //_rb.AddTorque(testTorque);
        
        // //Debug.Log(_rb.velocity.magnitude * 3.6f);
        // Debug.Log("Force: " + totalForce);
        // Debug.Log("Torque: " + totalTorque);
    }
}
