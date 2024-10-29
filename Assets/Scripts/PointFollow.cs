using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointFollow : MonoBehaviour
{
    public Transform follow;
    public float sensitivy;

    float radius;
    float mouseX;

    Vector3 offset;
    void Start()
    {
        offset = transform.position - follow.position;
        radius = offset.magnitude;
    }
    void Update()
    {
        transform.position = follow.position + offset;
        mouseX -= Input.GetAxis("Mouse X") * sensitivy * Time.deltaTime;
        offset = new Vector3(Mathf.Cos(mouseX) * radius, offset.y, Mathf.Sin(mouseX) * radius); 
    }
}
