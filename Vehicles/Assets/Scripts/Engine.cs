using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {
    [SerializeField] private Transform prop;
    [SerializeField] private float force;
    private Rigidbody rb;
    
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (Input.GetKey(KeyCode.W)) {
            rb.AddForceAtPosition(Time.fixedTime * force * transform.forward, prop.position);
        }
    }
}
