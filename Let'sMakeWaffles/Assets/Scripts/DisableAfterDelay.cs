using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterDelay : MonoBehaviour {
	public GameObject[] Obj;
	public float Delay;
	public bool DelayBetween;
	// Use this for initialization
	void Start () {
		StartCoroutine (EnableFunction (Delay));
	}

	public IEnumerator EnableFunction(float sec)

	{
		if (DelayBetween) {
			for (int i = 0; i < Obj.Length; i++) {
				Obj [i].SetActive (false);
				yield return new WaitForSeconds (sec);
		}} else {
			yield return new WaitForSeconds (sec);
			for (int i = 0; i < Obj.Length; i++) {
				Obj [i].SetActive (false);
			}}}
}

