using UnityEngine;
using System.Collections;

public class Shared {
	public static readonly Vector3 forward = new Vector3(1,0,1);
	public static readonly Vector3 right = new Vector3(1,0,-1);
	
	public static void UpdateAnimator (Animator animator, Vector3 dir) {
		animator.SetFloat ("Speed", dir.magnitude);
		
		if (dir.magnitude != 0) {
			if (Vector3.Dot (dir, right) > 0) {
				animator.transform.localScale = new Vector3 (1, 1, 1);
			} else {
				animator.transform.localScale = new Vector3 (-1, 1, 1);
			}
		}
	}

}
