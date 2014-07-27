using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float walkSpeed = 3f;
	public float runSpeed = 7f;
	
	private Game game;
	
	private Rigidbody body;
	private Animator animator;
	
	private AudioSource runSound;
	private bool running = false;

	public bool IsRunning () {
		return running;
	}

	void Start () {
		game = GameObject.Find ("Game").GetComponent<Game>();
		body = GetComponent<Rigidbody> ();
		animator = GetComponentInChildren<Animator> ();
		runSound = GetComponent<AudioSource> ();
	}
	
	void FixedUpdate () {
		if (game.IsPaused()) {
			body.velocity = Vector3.zero;
			Shared.UpdateAnimator (animator, 0, Vector3.zero);
			return;
		}
		
		var dir = new Vector3(0,0,0);
		if (Input.GetKey ("w")) {dir += Shared.forward;}
		if (Input.GetKey ("s")) {dir -= Shared.forward;}
		if (Input.GetKey ("d")) {dir += Shared.right;}
		if (Input.GetKey ("a")) {dir -= Shared.right;}
		dir.Normalize ();

		if (Input.GetKey (KeyCode.LeftShift)) {
			if (!runSound.isPlaying) { runSound.Play(); running = true; }
			dir *= runSpeed;
		} else {
			if (runSound.isPlaying) { runSound.Stop(); running = false; }
			dir *= walkSpeed;
		}
		
		body.velocity = dir;
		Shared.UpdateAnimator (animator, dir.magnitude, dir);
	}
}
