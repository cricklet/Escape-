using UnityEngine;
using System.Collections;
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
			death = d;

			GameObject waypointsParent = death.waypointsParent;
			int i = 0;
			waypoints = new Transform[waypointsParent.transform.childCount];
			foreach (Transform waypoint in waypointsParent.transform) {
				waypoints[i] = waypoint;
				i ++;
			}
			waypointIndex = 0;
		}

		public State Update () {
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

		public FollowPlayerState (Death d) {
			death = d;
		}

		public State Update () {
			return this;
		}
	}
	
	public GameObject player;
	public GameObject waypointsParent;

	public float walkSpeed;
	public float runSpeed;
	public float proximityDistance;

	private bool computingPathLock;
	private Path path;
	private int pathIndex;
	
	private CharacterController controller;
	private Light flashlight;
	private Animator animator;

	private State currentState;

	void Start () {
		controller = GetComponent<CharacterController> ();
		flashlight = GetComponentInChildren<Light> ();
		animator = GetComponentInChildren<Animator> ();
		currentState = new FollowWaypointsState (this);

		Seeker seeker = GetComponent<Seeker>();
		seeker.StartPath (transform.position, player.transform.position, OnPathComplete);
	}

	void OnPathComplete (Path p) {
		computingPathLock = false;

		if (!p.error) {
			path = p;
			pathIndex = 0;
		} else {
			Debug.LogError("Couldn't find path: " + path);
		}
	}
	
	void UpdateFlashlight (Vector3 dir) {
		if (dir.magnitude > 0.1) {
			flashlight.transform.LookAt (transform.position + dir);
		}
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

		UpdateFlashlight (dir);
		Shared.UpdateAnimator (animator, dir);
	}

	float DistanceTo (Vector3 other) {
		Vector3 current = transform.position;
		other.y = current.y;
		return Vector3.Distance (current, other);
	}

	bool IsNear(Vector3 other) {
		return DistanceTo (other) < proximityDistance;
	}

	void FollowPath () {
		if (path == null) {	return; }
		if (pathIndex >= path.vectorPath.Count) { return; }
		
		Vector3 goal = path.vectorPath [pathIndex];
		MoveTowards (goal, false);

		if (IsNear (goal)) {
			pathIndex ++;
		}
	}

	bool SeesPlayer () {
		var dir = player.transform.position - transform.position;
		dir.Normalize();
		
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
		//FollowPath ();
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
