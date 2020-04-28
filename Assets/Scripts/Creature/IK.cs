using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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


	[Header("Target")]
	public Transform tip;
	public Transform target;

	List<IKjoint> jointsList = new List<IKjoint>();
	List<float> boneLengths = new List<float>();
	//List<float> dst_to_nxt = new List<float>();
	float totalLength;

	private void Awake() {
		//init list
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
		float tip_to_target = Vector2.Distance(tip.position, target.position);


		//angle from j0 and target
		Vector2 diff = target.position - joints[0].position;
		float atan = Mathf.Atan2(diff.x, -diff.y) * Mathf.Rad2Deg;

		//is the target reachable?
		//if not, stretch as far as possible
		if (totalLength < dst_to_target) {

			//iha shaibaa
			var paska = jointsList[0];
			paska.jointAngle = atan;
			jointsList[0] = paska;

			//miks vitus tää ei voi toimia
			//jointsList[0].jointAngle = atan;

			for (int i = 1; i < joints.Length; i++) {
				paska = jointsList[i];
				paska.jointAngle = 0f;
				jointsList[i] = paska;
			}
		}
		else {
			float cosAngle;
			float angle;

			//BE MY ANGLE PLEASE
			for (int i = 0; i < joints.Length; i++) {
				var paska = jointsList[i];

				Debug.Log("joints pituus: " + joints.Length);
				//Debug.Log("bones pituus: " + boneLengths.Count); //te molemmat paskapäät ootte atm 4 miks itkette
				//se olin minä joka lopussa itki.
				

				// A = (b² + c² - a²) / 2bc
				//bonelengths[i] = c
				//bonelengths[i+1] = a

				if ((i % 2 == 0) && (i < boneLengths.Count - 1)) {
					cosAngle = ((dst_to_nxt(i) * dst_to_nxt(i)) + (boneLengths[i] * boneLengths[i]) - (boneLengths[i + 1] * boneLengths[i + 1])) / (2 * dst_to_nxt(i) * boneLengths[i]);
				}
				// B = (a² + c² - b²) / 2ac
				else if (i < boneLengths.Count - 1) {
					cosAngle = ((boneLengths[i + 1] * boneLengths[i + 1]) + (boneLengths[i] * boneLengths[i]) - (dst_to_nxt(i) * dst_to_nxt(i))) / (2 * boneLengths[i + 1] * boneLengths[i]);
				}
				//kun ollaan raajan päässä
				else {
					cosAngle = ((dst_to_nxt(i) * dst_to_nxt(i)) + (boneLengths[i] * boneLengths[i]) - (dst_to_nxt(i) * dst_to_nxt(i))) / (2 * dst_to_nxt(i) * boneLengths[i]);
					//cosAngle = ((tip_to_target * tip_to_target) + (boneLengths[i] * boneLengths[i]) - (dst_to_target * dst_to_target)) / (2 * tip_to_target * boneLengths[i]);
					//cosAngle = ((dst_to_target * dst_to_target) + (boneLengths[i] * boneLengths[i]) - (boneLengths[i + 1] * boneLengths[i + 1])) / (2 * dst_to_target * boneLengths[i]);
				}

				angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
				if (float.IsNaN(angle)) {
					Debug.Log("angle is: " + angle);
				}

				//so they work in unity
				if (i % 2 == 0) {
					paska.jointAngle = atan - angle;
					jointsList[i] = paska;
				}
				else { 
					paska.jointAngle = 180f - angle;
					jointsList[i] = paska;
				}
			}
		}

		for (int i = 0; i < boneLengths.Count; i++) {
			var paska = jointsList[i];
			Vector3 euler = new Vector3();

			euler = jointsList[i].joint.transform.localEulerAngles;
			euler.z = paska.jointAngle;
			jointsList[i].joint.transform.localEulerAngles = euler;

			paska.euler = euler;
			jointsList[i] = paska;
		}
	}

	float dst_to_nxt(int i) {
		if (joints.Length > i + 2) {
			return Vector2.Distance(joints[i].position, joints[i + 2].position);
		}
		Debug.Log("joint tääl on: " + i);
		return Vector2.Distance(joints[i].position, tip.position);
	}
}
