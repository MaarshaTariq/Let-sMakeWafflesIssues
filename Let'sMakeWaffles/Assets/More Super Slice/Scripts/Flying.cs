using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : MonoBehaviour {

	public GameObject final;
    public float durationAfterNextPanel;
	// Use this for initialization
	void Start ()
    {
		Invoke ("DisableScreen", durationAfterNextPanel);
		SoundManager.instance.PlaySound (19);
	}

	void DisableScreen()
	{
		final.SetActive (true);
		this.gameObject.SetActive (false);
	}
}
