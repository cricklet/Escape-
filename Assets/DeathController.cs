using UnityEngine;
using System.Collections;

public class DeathController : MonoBehaviour {

	public GameObject waypointsParent;

	private Transform[] waypoints = null;
	private int currentWaypoint = 0;

	private enum State { FOLLOW_PATH, FOLLOW_PLAYER };
	private State state;

	public GameObject player;

	private Vector3 right = new Vector3(1,0,-1);
	
	public float walkSpeed = 3f;
	public float runSpeed = 5f;

	private SpriteAnimator spriteAnimator;

	private Light flashlight;
	private float flashlightAngle;

	// Use this for initialization
	void Start () {
		spriteAnimator = GetComponentInChildren<SpriteAnimator> ();
		flashlight = GetComponentInChildren<Light> ();
		state = State.FOLLOW_PATH;
	
		int i = 0;
		waypoints = new Transform[waypointsParent.transform.childCount];
		foreach (Transform waypoint in waypointsParent.transform) {
			waypoints[i] = waypoint;
			i ++;
		}
	}

	float WalkSpeed () {
		float s = Mathf.Sin ((2f * Time.time) * Mathf.PI);
		return walkSpeed * (1f + s * 0.2f);
	}

	void MoveTowards (Vector3 goal, bool run) {
		var dir = goal - transform.position;
		dir.y = 0;
		if (dir.magnitude == 0) {
			return;
		}
		dir.Normalize ();

		if (run) {
			dir *= runSpeed;
		} else {
			dir *= WalkSpeed ();
		}
		
		rigidbody.velocity = dir;
		
		float speed = rigidbody.velocity.magnitude;
		bool facingRight = Vector3.Dot (rigidbody.velocity, right) > 0;
		
		spriteAnimator.SetSpeed(speed, facingRight);
	}

	bool CanSeePlayer () {
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

	// Update is called once per frame
	void Update () {
		if (state == State.FOLLOW_PATH) {
			Vector3 goal = waypoints[currentWaypoint].position;
			MoveTowards(goal, false);

			float dist = (goal - transform.position).magnitude;
			if (dist < 1.2) {
				currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
			}

			if (CanSeePlayer()) {
				state = State.FOLLOW_PLAYER;
			}
		} else if (state == State.FOLLOW_PLAYER) {
			MoveTowards(player.transform.position, true);
		}
	}
}
