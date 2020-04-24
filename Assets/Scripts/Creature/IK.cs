using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public struct IKjoint
{
	public Vector3 euler;
	public float jointAngle, angle;
	public Transform joint;

	//not needed really
	public IKjoint(Vector3 euler, float jointAngle, float angle, Transform joint)
	{
		this.euler = euler;
		this.jointAngle = jointAngle;
		this.angle = angle;
		this.joint = joint;
	}
}

public class IK : MonoBehaviour
{
	[Header("Joints")]
	public Transform[] joints;

	[Header("Legs")]
	public Transform[] legs;
	public Transform tip;

	[Header("Target")]
	public Transform target;

	List<IKjoint> jointsList = new List<IKjoint>();
	List<float> boneLengths = new List<float>();
	float totalLength;

	private void Awake() {
		for (int i = 0; i < joints.Length; i++) {
			IKjoint IK = new IKjoint(Vector3.zero, 0, 0, joints[i]);
			jointsList.Add(IK);
		}
	}

	private void Start() {
		//init lenghts
		for (int i = 0; i < joints.Length - 1; i++) {
			boneLengths.Add(Vector2.Distance(joints[i].position, joints[i + 1].position));

			if (i == joints.Length - 2)
				boneLengths.Add(Vector2.Distance(joints[i + 1].position, tip.position));
			totalLength += boneLengths[i];
		}
	}

	// AND THIS IS WHERE THE SHIT BEGINS
	// fuk it, tää on iha ripulia
	private void Update() {

		//dst from start to end
		float dst_to_target = Vector2.Distance(jointsList[0].joint.position, target.position);

		//angle from j0 and target
		Vector2 diff = target.position - joints[0].position;
		float atan = Mathf.Atan2(-diff.y, -diff.x) * Mathf.Rad2Deg;

		//is the target reachable?
		//if not, stretch as far as possible
		if (totalLength < dst_to_target) {

			//iha shaibaa
			var paska = jointsList[0];
			paska.jointAngle = atan;
			jointsList[0] = paska;

			//miks vitus tää ei voi toimia
			//jointsList[0].jointAngle = atan;

			for (int i = 0; i < joints.Length; i++) {
				paska = jointsList[i];
				paska.jointAngle = 0f;
				jointsList[i] = paska;
			}
		}
		else {
			float cosAngle;

			//BE MY ANGLE PLEASE
			for (int i = 0; i < joints.Length; i++) {
				var paska = jointsList[i];

				if (i % 2 == 0) {
					cosAngle = ((dst_to_target * dst_to_target) + (boneLengths[i] * boneLengths[i]) - (boneLengths[i + 1] * boneLengths[i + 1])) / (2 * dst_to_target * boneLengths[i]);
					paska.angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
					//jointsList[i] = paska;
				}
				else {
					cosAngle = ((boneLengths[i + 1] * boneLengths[i + 1]) + (boneLengths[i] * boneLengths[i]) - (dst_to_target * dst_to_target)) / (2 * boneLengths[i + 1] * boneLengths[i]);
					paska.angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
					//jointsList[i] = paska;
				}

				//so they work in unity
				if (i % 2 == 0) {
					paska.jointAngle = 180f - paska.angle;
					jointsList[i] = paska;
				}
				else { 
					paska.jointAngle = atan - paska.angle;
					jointsList[i] = paska;
				}
			}

		}

		for (int i = 0; i < boneLengths.Count; i++) {
			var paska = jointsList[i];

			paska.euler = jointsList[i].joint.transform.localEulerAngles;
			float shit = paska.euler.z;
			shit = paska.jointAngle;
			jointsList[i].joint.transform.localEulerAngles = paska.euler;
			jointsList[i] = paska;

			Debug.Log("jointpaskaa euler = " + jointsList[i].euler);
		}
	}
}
