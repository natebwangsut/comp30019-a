using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SunMoon : MonoBehaviour
{
    public float speed;
    
    // Use this for initialization
    void Start()
    {
        speed = (2*Mathf.PI) * 5;
    }

    // Update is called once per frame
    void Update()
    {
        // Assume x-axis is the of West-East
        transform.RotateAround(Vector3.zero, Vector3.right, speed * Time.deltaTime);
        transform.LookAt(Vector3.zero);
    }
}
