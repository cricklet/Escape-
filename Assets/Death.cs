using UnityEngine;
using System.Collections;
using System.Linq;
using Pathfinding;

public class Death : MonoBehaviour {

	interface State {
		State Update();
	}

	class FollowWaypointsState : State {
		private Death death;
		private Transform[] waypoints;
		private int waypointIndex;

		public FollowWaypointsState (Death d) {
			Debug.Log ("follow waypoints");
			death = d;

			GameObject waypointsParent = death.waypointsParent;

			int i = 0;
			waypoints = new Transform[waypointsParent.transform.childCount];
			foreach (Transform waypoint in waypointsParent.transform) {
				waypoints[i] = waypoint;
				i ++;
			}

			waypointIndex = ChooseWaypoint();
		}

		private int ChooseWaypoint() {
			float[] dots = new float[waypoints.Length];
			float[] dists = new float[waypoints.Length];

			for (int i = 0; i < waypoints.Length; i ++) {
				Transform waypoint = waypoints[i];
				Transform prevWaypoint = waypoints[(i + waypoints.Length - 1) % waypoints.Length];
				
				Vector3 waypointDir = (waypoint.position - death.transform.position).normalized;
				Vector3 pathDir = (waypoint.position - prevWaypoint.position).normalized;
				float dist = death.DistanceTo(waypoint.position);

				dots[i] = Vector3.Dot(waypointDir, pathDir);
				dists[i] = dist;
			}

			float[] scores = new float[waypoints.Length];
			float maxDist = dists.Max ();
			float minDot = dots.Max ();
			for (int i = 0; i < dists.Length; i ++) {
				scores[i] = (1 + maxDist - dists[i]) * (1 + dots[i] - minDot);
			}

			float maxScore = 0;
			int maxIndex = 0;
			for (int i = 0; i < scores.Length; i ++) {
				if (scores[i] > maxScore) {
					maxScore = scores[i];
					maxIndex = i;
				}
			}

			return maxIndex;
		}

		public State Update () {
			if (waypoints.Length == 0) {
				return this;
			}

			Vector3 goal = waypoints[waypointIndex].position;
			death.MoveTowards (goal, false);

			if (death.IsNear (goal)) {
				waypointIndex  = (waypointIndex + 1) % waypoints.Length;
			}

			if (death.SeesPlayer ()) {
				return new FollowPlayerState(death);
			}

			return this;
		}
	}

	class FollowPlayerState : State {
		private Death death;
		private Vector3 lastGoal;

		public FollowPlayerState (Death d) {
			Debug.Log ("follow player");
			death = d;
			death.flashlight.color = Color.red;
		}

		public State Update () {
			Vector3 goal = death.player.transform.position;
			death.MoveTowards (goal, true);
			
			if (!death.SeesPlayer ()) {
				return new FollowLostState(death, goal);
			}

			lastGoal = goal;
			return this;
		}
	}

	class FollowLostState : State {
		private Death death;
		private Vector3 goal;

		public FollowLostState (Death d, Vector3 g) {
			Debug.Log ("follow lost");
			death = d;
			goal = g;
		}
		
		public State Update () {
			death.MoveTowards (goal, true);
			
			if (death.SeesPlayer ()) {
				return new FollowPlayerState(death);
			}

			if (death.IsNear (goal)) {
				return new LookState (death);
			}

			return this;
		}
	}
	
	class LookState : State {
		private Death death;
		private float finish;
		
		public LookState (Death d) {
			Debug.Log ("look");
			death = d;
			finish = Time.time + 360f / death.angularSpeed;
		}
		
		public State Update () {
			death.MoveStop ();
			death.RotateFacing (death.angularSpeed);

			if (death.SeesPlayer ()) {
				return new FollowPlayerState(death);
			}

			if (Time.time > finish) {
				return new FollowWaypointsState(death);
			}

			return this;
		}
	}

	// the world
	public GameObject player;
	public GameObject waypointsParent;

	// public parameters
	public float walkSpeed;
	public float runSpeed;
	public float proximityDistance;
	public float fov;
	public float angularSpeed;

	// me
	private Vector3 facing;
	private CharacterController controller; 
	
	// rendering
	private Light flashlight;
	private Animator animator;

	// my state
	private State currentState;

	void Start () {
		controller = GetComponent<CharacterController> ();
		flashlight = GetComponentInChildren<Light> ();
		animator = GetComponentInChildren<Animator> ();
		currentState = new FollowWaypointsState (this);
	}

	void RotateFacing (float angularVelocity) {
		Quaternion rotation = Quaternion.AngleAxis(angularVelocity * Time.deltaTime, Vector3.up);
		SetFacing (rotation * facing);
	}

	void SetFacing (Vector3 dir) {
		if (dir.magnitude > 0.1) {
			facing = dir.normalized;
		}
	}
	
	void MoveStop () {
		controller.SimpleMove (Vector3.zero);
	}

	void MoveTowards (Vector3 goal, bool running) {
		Vector3 current = transform.position;
		goal.y = current.y;
		
		Vector3 dir = (goal - current).normalized;
		if (running) {
			dir *= runSpeed;
		} else {
			dir *= walkSpeed;
		}
		
		controller.SimpleMove (dir);

		SetFacing (dir);
	}

	float DistanceTo (Vector3 other) {
		Vector3 current = transform.position;
		other.y = current.y;
		return Vector3.Distance (current, other);
	}

	bool IsNear(Vector3 other) {
		return DistanceTo (other) < proximityDistance;
	}

	bool SeesPlayer () {
		var dir = player.transform.position - transform.position;
		dir.Normalize();

		if (Vector3.Angle(dir, facing) > fov) { return false; }
		
		Ray ray = new Ray (transform.position, dir);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 100f)) {
			GameObject obj = hit.collider.gameObject;
			if (obj == player) {
				return true;
			}
		}
		
		return false;
	}

	void Update () {
		currentState = currentState.Update ();
		
		Shared.UpdateAnimator (animator, controller.velocity.magnitude, facing);
		flashlight.transform.LookAt (transform.position + facing);
	}

	void OnControllerColliderHit (ControllerColliderHit hit) {
		Collider collider = hit.collider;
		Rigidbody body = collider.attachedRigidbody;
		if (body == null) { return; }

		Vector3 dir = (body.position - transform.position).normalized;
		dir.y = 0.5f;
		body.velocity = walkSpeed * dir * 1.3f;
	}
}
