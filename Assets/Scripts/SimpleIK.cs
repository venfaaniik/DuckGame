using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleIK : MonoBehaviour
{
    [Header("Joints)")]
    public Transform joint0;
    public Transform joint1;
    public Transform hand;

    [Header("target")]
    public Transform target;

    private float length0;
    private float length1;

    void Start()
    {
        length0 = Vector2.Distance(joint0.position, joint1.position);
        length1 = Vector2.Distance(joint1.position, hand.position);
        Debug.Log(length0);
        Debug.Log(length1);
    }

    void Update()
    {
        float jointAngle0;
        float jointAngle1;

        //distance from joint0 to target
        float distance_to_target = Vector2.Distance(joint0.position, target.position);

        // Angle from joint0 and target
        Vector2 diff = target.position - joint0.position;
        float atan = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        //Debug.Log("target pos: " + target.position);
        //Debug.Log("joint0 pos: " + joint0.position);
        //Debug.Log("diff :" + diff);
        //Debug.Log("atan: " + atan);

        // Is the target reachable?
        // If not, we stretch as far as possible
        if (length0 + length1 < distance_to_target)
        {
            jointAngle0 = atan;
            jointAngle1 = 0f;
            Debug.Log("Distance to target is more than our length!");
        }
        else
        {
            //inner angle ALPHA
            float cosAngle0 = ((distance_to_target * distance_to_target) + (length0 * length0) - (length1 * length1)) / (2 * distance_to_target * length0);
            float angle0 = Mathf.Acos(cosAngle0) * Mathf.Rad2Deg;

            //inner angle BETA
            float cosAngle1 = ((length1 * length1) + (length0 * length0) - (distance_to_target * distance_to_target)) / (2 * length1 * length0);
            float angle1 = Mathf.Acos(cosAngle1) * Mathf.Rad2Deg;

            // So they work in Unity reference frame
            jointAngle0 = atan - angle0; //ANGLE A
            jointAngle1 = 180f - angle1; //ANGLE B
        }

        Vector3 euler0 = joint0.transform.localEulerAngles;
        euler0.z = jointAngle0;
        joint0.transform.localEulerAngles = euler0;

        Vector3 euler1 = joint1.transform.localEulerAngles;
        euler1.z = jointAngle1;
        joint1.transform.localEulerAngles = euler1;
    }
}
