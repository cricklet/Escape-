using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	
	public string nextScene;
	
	private GameObject loseScreen;
	
	private bool dead = false;
	private float deadTime = 0;
	
	void Start () {
		if (Music.Instance) {
			Music.Instance.UnLockAll();
		}
		loseScreen = GameObject.Find("Lose Screen");
	}
	
	public bool IsPaused () {
		return dead;
	}
	
	public void Lose () {
		dead = true;
	}
	
	public void Win () {
		Application.LoadLevel (nextScene);
	}
	
	private Color UpdateAlpha (Color c, float alpha) {
		return new Color(c.r, c.g, c.b, alpha);
	}
	
	void Update () {
		if (dead) {
			GUITexture tex = loseScreen.GetComponent<GUITexture> ();
			GUIText text = loseScreen.GetComponentInChildren<GUIText> ();
			
			float alpha = Mathf.Clamp(deadTime * 1f, 0f, 1f);
			tex.color = UpdateAlpha(tex.color, alpha);
			text.color = UpdateAlpha(text.color, 1f);
			
			deadTime += Time.deltaTime;
			
			if (deadTime > 1.5f) {
				Application.LoadLevel (Application.loadedLevelName);
			}
		}
	}
	
	
	/*
	private Vector3 cameraPosition;
	public void Kill () {
		alive = false;
		cameraPosition = mainCamera.transform.position;
	}
	
		if (!alive) {
			Shared.UpdateAnimator (animator, 0, Vector3.zero);
			mainCamera.transform.position = cameraPosition + Random.insideUnitSphere * 0.1f;
			return;
		}
		
	*/
	
}
