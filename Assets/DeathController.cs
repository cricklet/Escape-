using UnityEngine;
using System.Collections;
using Pathfinding;

public class DeathController : MonoBehaviour {

	public GameObject waypointsParent;

	public GameObject player;

	public float walkSpeed = 3f;
	public float runSpeed = 5f;

	private Path currentPath;

	// Use this for initialization
	void Start () {
		//Get a reference to the Seeker component we added earlier
		Seeker seeker = GetComponent<Seeker>();
		
		//Start a new path to the targetPosition, return the result to the OnPathComplete function
		seeker.StartPath (transform.position, player.transform.position, OnPathComplete);
	}

	void OnPathComplete (Path path) {
		if (!path.error) {
			currentPath = path;
		}
		Debug.Log ("Yay, we got a path back. Did it have an error? " + path.error);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
