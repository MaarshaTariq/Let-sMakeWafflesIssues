using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Fade : MonoBehaviour {
	[HideInInspector]
	public bool FadeIn;
	[HideInInspector]
	public bool Fadeout;
	public float FadeRate=0.85f;
	public bool FadeAtStart=true;
    public Camera cam;
	
    // Use this for initialization
	void OnEnable()
	{
		
		if (FadeAtStart) {
			FadeIn = true;
			Fadeout = false;
		}
	}
	void Update () {

		if (FadeIn) {
			this.gameObject.GetComponent<CanvasGroup> ().alpha += Time.deltaTime*FadeRate;
			
			if (this.gameObject.GetComponent<CanvasGroup> ().alpha == 1) {
				FadeIn = false;
				if(cam!= null)
				{
						cam.backgroundColor = Color.white;
				}
				
			}
		 }
		if (Fadeout) {
			this.gameObject.GetComponent<CanvasGroup> ().alpha -= Time.deltaTime*FadeRate;
			if(cam!= null)
				{
						cam.backgroundColor = Color.black;
				}
			
			
			if (this.gameObject.GetComponent<CanvasGroup> ().alpha == 0) {
				Fadeout = false;
				
				
			}
		}	
	}
}
