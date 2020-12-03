using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCaptionHandler : MonoBehaviour {

	public bool createCaptionOnEnable;
	public int soundIndex;

	public bool addCaption;
	void OnEnable()
	{
		if(createCaptionOnEnable)
			CloseCaption.CCManager.instance.CreateCaption(soundIndex, SoundManager.instance.sounds[soundIndex].length);

		if(addCaption) AddCaption();
	}
	
	void Update()
	{
		if(addCaption)
		{	
			addCaption = false;
			AddCaption();
		}
	}

	public void AddCaption()
	{
		CloseCaption.CCManager.instance.CreateCaption(soundIndex, SoundManager.instance.sounds[soundIndex].length);
	}
}
