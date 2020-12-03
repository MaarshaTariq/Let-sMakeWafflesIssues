using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class DelayToPlaySound : MonoBehaviour {
	
	public int AudioToPlay;
	SoundManager Sound;
	public float sec = 1f;

	// Use this for initialization

	void OnEnable()
	{
		Sound = GameObject.Find ("SoundManager").GetComponent<SoundManager> ();
		if(!GameManager.Instance.Accessibilty)
			StartCoroutine (wait ());
	}
	// Update is called once per frame
	void Update () {
		
	}
	IEnumerator wait()
	{
		yield return new WaitForSeconds (sec);
		SoundManager.instance.GetComponent<AudioSource>().clip =	SoundManager.instance.sounds [AudioToPlay];
		SoundManager.instance.GetComponent<AudioSource>().Play ();

		
	}
}
