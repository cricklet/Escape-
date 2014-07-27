using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {
	
	private Game game;
	private GameObject player;
	
	public float doorOpenSpeed;
	public float proximityDist;

	private float doorSize = 1f;

	void Start () {
		player = GameObject.Find ("Player");
		game = GameObject.Find ("Game").GetComponent<Game>();
	}

	void Update () {
		if (Vector3.Distance (transform.position, player.transform.position) < proximityDist) {
			doorSize -= doorOpenSpeed * Time.deltaTime;
		} else {
			doorSize += doorOpenSpeed * Time.deltaTime;
		}
		
		doorSize = Mathf.Clamp(doorSize, 0f, 1f);
		transform.localScale = new Vector3 (1.1f, doorSize * 1.1f, doorSize * 1.1f);
		
		if (doorSize == 0f) {
			game.Win();
		}
	}
}
