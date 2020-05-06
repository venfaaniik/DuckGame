using System.Collections.Generic;
using UnityEngine;

public struct IKjoint {
	public Vector3 euler;
	public float jointAngle;
	public Transform joint;

	//not needed really
	public IKjoint(Vector3 euler, float jointAngle, Transform joint) {
		this.euler = euler;
		this.jointAngle = jointAngle;
		this.joint = joint;
	}
}

public class IK : MonoBehaviour {
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
			IKjoint IK = new IKjoint(Vector3.zero, 0, joints[i]);
			jointsList.Add(IK);
		}
	}

	private void Start() {
		//init lenghts
		//this works for now
		for (int i = 0; i < joints.Length; i++) {
			float temp;
			if (i < (joints.Length - 1))
				temp = Vector2.Distance(joints[i].position, joints[i + 1].position);
			else
				temp = Vector2.Distance(joints[i].position, tip.position);

			totalLength += temp;
			boneLengths.Add(temp);
		}
	}

	// AND THIS IS WHERE THE SHIT BEGINS
	// fuk it, tää on iha ripulia
	private void Update() {

		//dst from start to end
		float dst_to_target = Vector2.Distance(jointsList[0].joint.position, target.position);
		//float joint_to_tip = Vector2.Distance(tip.position, target.position);


		//angle from j0 and target
		Vector2 diff = target.position - jointsList[0].joint.position;
		float atan = Mathf.Atan2(-diff.y, -diff.x) * Mathf.Rad2Deg;

		//is the target reachable?
		//if not, stretch as far as possible
		if (totalLength < dst_to_target) {

			//iha shaibaa
			var paska = jointsList[0];
			paska.jointAngle = atan;
			jointsList[0] = paska;

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

				//Debug.Log("joints pituus: " + joints.Length);
				//Debug.Log("bones pituus: " + boneLengths.Count); //te molemmat paskapäät ootte atm 4 miks itkette
				//se olin minä joka lopussa itki.

				//// 1() ((distance_to_target * distance_to_target) + (length0 * length0) - (length1 * length1)) / (2 * distance_to_target * length0);
				//// 2() ((length1 * length1) + (length0 * length0) - (distance_to_target * distance_to_target)) / (2 * length1 * length0);

				//if ((i == 0) && (i < boneLengths.Count - 1)) {
				//	cosAngle = ((dst_to_nxt(i) * dst_to_nxt(i)) + (boneLengths[i] * boneLengths[i]) - (boneLengths[i + 1] * boneLengths[i + 1])) / (2 * dst_to_nxt(i) * boneLengths[i]);
				//}

				if (i < boneLengths.Count - 1) {
					cosAngle = ((boneLengths[i + 1] * boneLengths[i + 1]) + (boneLengths[i] * boneLengths[i]) - (dst_to_nxt(i) * dst_to_nxt(i))) / (2 * boneLengths[i + 1] * boneLengths[i]);
				}

				else {
					//distance =>> väli J3 ja targetin välillä
					//dst_to_nxt => Vector2.Distance(joints[i].position, tip.position);
					//cosAngle = ((dst_to_nxt(i) * dst_to_nxt(i)) + (boneLengths[i] * boneLengths[i]) - (distance(i) * distance(i))) / (2 * dst_to_nxt(i) * boneLengths[i]);
					cosAngle = ((distance(i) * distance(i)) + (boneLengths[i] * boneLengths[i]) - (dst_to_nxt(i) * dst_to_nxt(i))) / (2 * dst_to_nxt(i) * boneLengths[i]);
				}

				angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
				if (float.IsNaN(angle)) {
					Debug.Log("angle is: " + angle);
				}

				//so they work in unity
				if (i == 0) {
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

			Vector3 euler = jointsList[i].joint.transform.localEulerAngles;
			euler.z = paska.jointAngle;
			joints[i].transform.localEulerAngles = euler;

			paska.euler = euler;
			jointsList[i] = paska;
		}
	}

	float dst_to_nxt(int i) {
		if (joints.Length > i + 2) {
			return Vector2.Distance(joints[i].position, joints[i + 2].position);
		}
		return Vector2.Distance(joints[i].position, tip.position);
	}

	//distance to target
	float distance(int i) {
		return Vector2.Distance(joints[i].position, target.position);
	}


	void backwards() {
		int last = joints.Length;
		joints[last - 1] = target;

		for (int i = 0; i < joints.Length; i++) {

		}
	}
}



/*

Y function chain:backward()
-- backward reaching; set end effector as target

self.joints[self.n] = self.target;
v for i = self.n - 1, 1, -1 do
local r = (self.joints[i+1] - self.joints[i]);
local 1 = self.lengths[i] / r.magnitude;

-- find new joint position
local pos = (1 - 1) * self.joints[i+1] + 1 * self.joints[i];
self.joints[i] = pos;
end;
end;

Y function chain: forward ()
-- forward reaching; set root at initial position

jself.joints[1] = self.origin.p;
local coneVec = (self.joints[2] - self.joints[1i]).unit;
“ for i= 1, self.n - 1 do
local r = (self.joints[i+1] - self.joints[i]);

local 1 = self.lengths[i] / r.magnitude;
-- setup matrix

local cf_.= CFrame.new(self.joints[i], self.joints[i] + coneVec);

-- find hew joint position
local pos = (1 - 1) * self.joints[i] + 1 * self.joints[i+1];

local t = self:constrain(pos - self.joints[i], coneVec, cf);

self.joints[i+1] = self.constrained and self.joints[i] + t or pos;

coneVec = self.joints[i+1] - self.joints[i];
end;
end;

 */
