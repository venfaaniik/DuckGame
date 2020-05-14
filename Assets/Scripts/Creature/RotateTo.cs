using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RotateTo : MonoBehaviour {
    // The target marker.
    public Transform target;

    // Angular speed in radians per sec.
    public float speed = 200.0f;

    void Update() {

        Vector3 vectorToTarget = target.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle - 45, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, speed);
    }
}
