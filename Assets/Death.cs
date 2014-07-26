using UnityEngine;
using System.Collections;
using Pathfinding;

public class Death : MonoBehaviour {

	public GameObject waypointsParent;
	public GameObject player;

	public float walkSpeed;
	public float runSpeed;
	public float proximityDistance;

	private Path path;
	private int pathIndex;
	
	private Rigidbody body;
	private Animator animator;

	void Start () {
		body = GetComponent<Rigidbody> ();
		animator = GetComponentInChildren<Animator> ();

		Seeker seeker = GetComponent<Seeker>();
		seeker.StartPath (transform.position, player.transform.position, OnPathComplete);
	}

	void OnPathComplete (Path p) {
		if (!p.error) {
			path = p;
			pathIndex = 0;
		} else {
			Debug.LogError("Couldn't find path: " + path);
		}
	}

	Vector3 UpdateDir () {
		if (path == null) {	return Vector3.zero; }
		if (pathIndex >= path.vectorPath.Count) { return Vector3.zero; }
		
		Vector3 current = transform.position;
		Vector3 goal = path.vectorPath [pathIndex];
		goal.y = current.y;
		
		Vector3 dir = (goal - current).normalized;
		dir *= walkSpeed;

		if (Vector3.Distance (current, goal) < proximityDistance) {
			pathIndex ++;
		}

		return dir;
	}

	// Update is called once per frame
	void Update () {
		Vector3 dir = UpdateDir ();
		body.velocity = dir;
		
		Shared.UpdateAnimator (animator, dir);
	}
}
