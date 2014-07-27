using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	private static Music instance = null;
	public static Music Instance {
		get { return instance; }
	}

	public AudioSource lockMusic;
	public AudioSource defaultMusic;
	public float volumeRate;

	private float lockVolume;
	private int[] locks;
	
	void Awake () {
		if (instance != null) {
			Destroy (this);
		} else {
			instance = this;
			DontDestroyOnLoad(this);
		}
	}
	
	void Start () {
		locks = new int[10];
		for (int i = 0; i < locks.Length; i ++) {
			locks[i] = -1;
		}
		
		lockMusic.volume = 0;
		defaultMusic.volume = 1;
	}
	
	public void Lock(int id) {
		for (int i = 0; i < locks.Length; i ++) {
			if (locks[i] == -1) {
				locks[i] = id;
				return;
			}
		}
	}

	public void UnLock(int id) {
		for (int i = 0; i < locks.Length; i ++) {
			if (locks[i] == id) {
				locks[i] = -1;
			}
		}
	}

	public void UnLockAll () {
		for (int i = 0; i < locks.Length; i ++) {
			locks[i] = -1;
		}
	}

	void Update () {
		bool locked = false;
		for (int i = 0; i < locks.Length; i ++) {
			if (locks[i] != -1) {
				locked = true;
			}
		}

		float lockVolume = lockMusic.volume;
		if (locked) {
			lockVolume += volumeRate * Time.deltaTime;
		} else {
			lockVolume -= volumeRate * Time.deltaTime;
		}

		lockMusic.volume = Mathf.Clamp (lockVolume, 0, 1);
		defaultMusic.volume = 1 - lockMusic.volume;
	}
}
