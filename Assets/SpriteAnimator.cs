using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour {
	
	private Animator animator;

	void Start () {
		animator = GetComponent<Animator> ();
	}

	public void SetSpeed(float speed, bool facingRight) {
		if (animator != null) {
			animator.SetFloat ("Speed", speed);
		}

		if (speed != 0) {
			if (facingRight) {
				renderer.transform.localScale = new Vector3 (1, 1, 1);
			} else {
				renderer.transform.localScale = new Vector3 (-1, 1, 1);
			}
		}
	}
}
