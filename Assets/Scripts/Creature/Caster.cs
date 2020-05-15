using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Caster : MonoBehaviour {
    public Transform targetter;
    public Transform root;
    public Vector2 pivotPoint = Vector2.zero;
    public int range = 5;
    public int angle = 0;
    public int angle2 = 30;
    public int accuracy = 10;
    public float moveSpeed = 0.01f;
    public int average = 3;
    public Color rayColour = Color.black;

    private Rigidbody2D rb;

    int tempAngle;
    Vector3 temp;

    private Vector2 startPoint = Vector2.zero;
    public LayerMask unwalkableMask;

    private List<Vector3> raycasts;

    // Use this for initialization
    void Start() {
        tempAngle = angle;
    }

    // Update is called once per frame
    void Update() {
        Raycaster();
    }

    void Raycaster() {
        raycasts = new List<Vector3>();      

        startPoint = (Vector2)transform.position + pivotPoint; // Update starting ray point.

        if (angle < angle2) {
            if (tempAngle >= angle2) {
                changeAngleLower();
            }
            else {
                changeAngleHigher();
            }
        }
        else {
            if (tempAngle >= angle) {
                changeAngleLower();
            }
            else {
                changeAngleHigher();
            }
        }

        if (raycasts.Count > 1) {
            //temp = getClosest();
            temp = getFurthest();
            if (temp != Vector3.zero) {
                moveToPosition(temp, targetter);
            }
        }
        //Debug.Log(raycasts.Count);
    }

    public Vector2 GetDirectionVector2D(float angle) {
        Vector2 v2angle = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        return v2angle;
    }

    //for (int i = 0; i< 10; i++)
    //{
    //    FVector LineEnd = UKismetMathLibrary::RotateAngleAxis(Direction, Angle, FVector::UpVector);
    //    LineEnd *= Range;
    //    DrawDebugLine(GetWorld(), GetActorLocation(), LineEnd, FColor::Red, false, 0.1f);
    //    Angle += 9.0f;
    //}

void changeAngleLower() {
        if (angle < angle2) {
            while (tempAngle >= angle) {
                Vector2 direction = GetDirectionVector2D(tempAngle);

                RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, range, unwalkableMask); // Shot ray.

                // Draw ray. For Debug we have to multiply our direction vector. 
                // Even if there is said Debug.DrawRay(start, dir), not Debug.DrawRay(start, end).
                Debug.DrawRay(startPoint, direction * range, rayColour);
                tempAngle -= accuracy;

                if (hit.collider != null) {
                    raycasts.Add(hit.point);
                }
            }
        }
        else {
            while (tempAngle >= angle2) {
                Vector2 direction = GetDirectionVector2D(tempAngle);
                RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, range, unwalkableMask); // Shot ray.

                // Draw ray. For Debug we have to multiply our direction vector. 
                // Even if there is said Debug.DrawRay(start, dir), not Debug.DrawRay(start, end).
                Debug.DrawRay(startPoint, direction * range, rayColour);
                tempAngle -= accuracy;

                if (hit.collider != null) {
                    raycasts.Add(hit.point);
                }
            }
        }
    }

    void changeAngleHigher() {
        if (angle < angle2) {
            while (tempAngle <= angle2) {
                Vector2 direction = GetDirectionVector2D(tempAngle);

                RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, range, unwalkableMask); // Shot ray.

                // Draw ray. For Debug we have to multiply our direction vector. 
                // Even if there is said Debug.DrawRay(start, dir), not Debug.DrawRay(start, end). Keep that in mind.
                Debug.DrawRay(startPoint, direction * range, rayColour);
                tempAngle += accuracy;

                if (hit.collider != null) {
                    raycasts.Add(hit.point);
                }
            }
        }
        else {
            while (tempAngle <= angle) {
                Vector2 direction = GetDirectionVector2D(tempAngle);
                
                RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, range, unwalkableMask); // Shot ray.

                // Draw ray. For Debug we have to multiply our direction vector. 
                // Even if there is said Debug.DrawRay(start, dir), not Debug.DrawRay(start, end). Keep that in mind.
                Debug.DrawRay(startPoint, direction * range, rayColour);
                tempAngle += accuracy;

                if (hit.collider != null) {
                    raycasts.Add(hit.point);
                }
            }
        }
    }

    public void moveToPosition(Vector3 worldPos, Transform transform) {
        transform.position = Vector2.Lerp(transform.position, (Vector2)worldPos, moveSpeed);
    }

    public Vector3 getClosest() {
        var shortestDistance = Mathf.Infinity;
        Vector3 dst = Vector3.zero;
        for (var i = 0; i < raycasts.Count - 1; i++) {
            for (var k = i + 1; k < raycasts.Count; k++) {
                var distance = Vector3.Distance(raycasts[i], root.transform.position);
                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    dst = raycasts[i];
                }
            }
        }
        //raycasts.Clear();
        return dst;
    }

    public Vector3 getFurthest() {
        var longestDst = Mathf.NegativeInfinity;
        Vector3 dst = Vector3.zero;
        for (var i = 0; i < raycasts.Count - 1; i++) {
            for (var k = i + 1; k < raycasts.Count; k++) {
                var distance = Vector3.Distance(raycasts[i], root.transform.position);
                if (distance > longestDst) {
                    longestDst = distance;
                    dst = raycasts[i];
                }
            }
        }
        return dst;
    }
}