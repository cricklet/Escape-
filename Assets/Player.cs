using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float walkSpeed = 3f;
	public float runSpeed = 7f;
	
	private Rigidbody body;
	private Animator animator;

	void Start () {
		body = GetComponent<Rigidbody> ();
		animator = GetComponentInChildren<Animator> ();
	}

	void FixedUpdate () {
		var dir = new Vector3(0,0,0);
		if (Input.GetKey ("w")) {dir += Shared.forward;}
		if (Input.GetKey ("s")) {dir -= Shared.forward;}
		if (Input.GetKey ("d")) {dir += Shared.right;}
		if (Input.GetKey ("a")) {dir -= Shared.right;}
		dir.Normalize ();

		if (Input.GetKey (KeyCode.LeftShift)) {
			dir *= runSpeed;
		} else {
			dir *= walkSpeed;
		}
		
		body.velocity = dir;
		Shared.UpdateAnimator (animator, dir);
	}
}
