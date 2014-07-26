using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private Vector3 forward = new Vector3(1,0,1);
	private Vector3 right = new Vector3(1,0,-1);
	
	public float walkSpeed = 3f;
	public float runSpeed = 7f;

	private SpriteAnimator spriteAnimator;

	// Use this for initialization
	void Start () {
		spriteAnimator = GetComponentInChildren<SpriteAnimator> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		var dir = new Vector3(0,0,0);
		if (Input.GetKey ("w")) {dir += forward;}
		if (Input.GetKey ("s")) {dir -= forward;}
		if (Input.GetKey ("d")) {dir += right;}
		if (Input.GetKey ("a")) {dir -= right;}
		dir.Normalize ();

		if (Input.GetKey (KeyCode.LeftShift)) {
			dir *= runSpeed;
		} else {
			dir *= walkSpeed;
		}

		rigidbody.velocity = dir;
	
		float speed = rigidbody.velocity.magnitude;
		bool facingRight = Vector3.Dot (rigidbody.velocity, right) > 0;
		
		spriteAnimator.SetSpeed(speed, facingRight);
	}
}
