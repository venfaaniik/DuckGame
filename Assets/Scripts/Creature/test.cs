using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private void Start() {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = transform.TransformDirection(Vector3.right);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 50); // Shot ray.

        // Draw ray. For Debug we have to multiply our direction vector. 
        // Even if there is said Debug.DrawRay(start, dir), not Debug.DrawRay(start, end).
        Debug.DrawRay(transform.position, direction * 50, Color.blue);
    }
}
