using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowHandler : MonoBehaviour {
	[HideInInspector]
	public bool FadeIn;
	[HideInInspector]
	public bool Fadeout;
	CanvasGroup IMG_Glow;
	public float Rate;
	// Use this for initialization
	void Start () {
		FadeIn = true;
		IMG_Glow = this.GetComponent<CanvasGroup> ();
	}
	void Update () {

		if (FadeIn) {
			IMG_Glow.alpha += Rate * Time.deltaTime;
			if (IMG_Glow.alpha == 1) {
				FadeIn = false;
				Fadeout = true;
			}
		}
		if (Fadeout) {
			IMG_Glow.alpha -= Rate * Time.deltaTime;
			if (IMG_Glow.alpha == 0) {
				Fadeout = false;
				FadeIn = true;
			}
		}	

	}
}
