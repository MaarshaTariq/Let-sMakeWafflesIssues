using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastFade : MonoBehaviour {
	[HideInInspector]
	public bool FadeIn;
	[HideInInspector]
	public bool Fadeout;
	public float FadeRate=3f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame

	void Update () {

		if (FadeIn) {
			this.gameObject.GetComponent<CanvasGroup> ().alpha += Time.deltaTime*FadeRate;
			if (this.gameObject.GetComponent<CanvasGroup> ().alpha == 1) {
				FadeIn = false;
			}
		}
		if (Fadeout) {
			this.gameObject.GetComponent<CanvasGroup> ().alpha -= Time.deltaTime*FadeRate;
			if (this.gameObject.GetComponent<CanvasGroup> ().alpha == 0) {
				Fadeout = false;
			}
		}	
	}
}
