using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class CircleCast : MonoBehaviour
{
    public Transform LegHead;
    public Vector2 pivotPoint = Vector2.zero;
    public float range = 5.0f;
    public float angle = 45.0f;
    public float angle2 = 60.0f;
    public float accuracy = 10.0f;
    public float moveSpeed = 0.01f;

    public float tempAngle;

    private Vector2 startPoint = Vector2.zero;
    public LayerMask unwalkableMask;

    // Use this for initialization
    void Start()
    {
        tempAngle = angle;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("Raycaster");
    }

    IEnumerator Raycaster()
    {
        startPoint = (Vector2)transform.position + pivotPoint; // Update starting ray point.
        if (tempAngle > angle2)
        {
            changeAngleLower();
        }
        else if (tempAngle < angle)
        {
            changeAngleHigher();
        }

        yield return new WaitForSeconds(5f);
    }

    public Vector2 GetDirectionVector2D(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }

    void changeAngleLower()
    {
        while (!(tempAngle == angle2))
        {
            Vector2 direction = GetDirectionVector2D(tempAngle);

            RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, range, unwalkableMask); // Shot ray.

            // Draw ray. For Debug we have to multiply our direction vector. 
            // Even if there is said Debug.DrawRay(start, dir), not Debug.DrawRay(start, end). Keep that in mind.
            Debug.DrawRay(startPoint, direction * range, Color.red);
            tempAngle -= accuracy;

            if (hit.collider != null)
            {
                moveToPosition(hit.point, LegHead);
                Debug.Log(hit.point);
            }
        }
    }

    void changeAngleHigher()
    {
        while (!(tempAngle == angle))
        {
            Vector2 direction = GetDirectionVector2D(tempAngle);

            Physics2D.Raycast(startPoint, direction, range); // Shot ray.

            // Draw ray. For Debug we have to multiply our direction vector. 
            // Even if there is said Debug.DrawRay(start, dir), not Debug.DrawRay(start, end). Keep that in mind.
            Debug.DrawRay(startPoint, direction * range, Color.red);
            tempAngle += accuracy;
        }
    }

    public void moveToPosition(Vector3 worldPos, Transform transform)
    {
        transform.position = Vector3.Lerp(transform.position, worldPos, moveSpeed);
    }
}