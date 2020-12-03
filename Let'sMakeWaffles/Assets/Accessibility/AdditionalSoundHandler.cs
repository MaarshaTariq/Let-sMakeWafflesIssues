using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AdditionalSoundHandler : MonoBehaviour {

	public static AdditionalSoundHandler Instance;
	public string buttonLabel;
	AudioSource audioSource;
	public AudioClip soundToPlay;
	public string textToShow;
	bool isPlayingIncorrectAndInfoSound;
	bool isPlayingInfoSound;

	UnityEngine.Coroutine playIncorrectAndInfoSoundE;
	UnityEngine.Coroutine playInfoSoundE;

	// Use this for initialization
	void Start () {
		Instance = this;
		audioSource = GetComponent<AudioSource>();
		isPlayingIncorrectAndInfoSound = false;
		isPlayingInfoSound = false;	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlaySoundToPlay()
	{
		if(soundToPlay == null || !GameManager.Instance.Accessibilty)
			return;
			
		audioSource.clip = soundToPlay;
		audioSource.Play();
		CloseCaption.CCManager.instance.CreateCaption(textToShow, audioSource.clip.length);
	}

	public void PlayInfoSound(float delay = 0)
	{
		if(!GameManager.Instance.Accessibilty)
			return;
			
		Debug.Log("AdditionalSoundHandler->PlayInfoSound(" + !isPlayingInfoSound + ")");
		if(!isPlayingInfoSound)
			playInfoSoundE = StartCoroutine(PlayInfoSoundE(delay));
		else
		{
			StopCoroutine(playInfoSoundE);
			isPlayingInfoSound = false;
			playInfoSoundE = StartCoroutine(PlayInfoSoundE(delay));
		}
	}

	IEnumerator PlayInfoSoundE(float delay)
	{
		Debug.Log("AdditionalSoundHandler->PlayInfoSoundE(" + delay + ")");
		isPlayingInfoSound = true;
		FreezeControlsHandler.Instance.FreezeControlls();
		yield return new WaitForSeconds(delay);
		AccessibilityManager.instance.Info(true);
		float wait = TextToSpeech.ins.source.clip.length;
		yield return new WaitForSeconds(wait);
		
		isPlayingInfoSound = false;
	}

	public void PlayIncorrectAndInfoSound()
	{
		// Debug.Log("AdditionalSoundHandler->PlayIncorrectAndInfoSound() called");
		if(!isPlayingIncorrectAndInfoSound)
			playIncorrectAndInfoSoundE = StartCoroutine(PlayIncorrectAndInfoSoundE());
		else
		{
			StopCoroutine(playIncorrectAndInfoSoundE);
			isPlayingIncorrectAndInfoSound = false;
			playIncorrectAndInfoSoundE = StartCoroutine(PlayIncorrectAndInfoSoundE());
		}
	}

	public void PlayMatchThisSound()
	{
		audioSource.clip = SoundManager.instance.sounds[0];
		audioSource.Play();
	}

	IEnumerator PlayIncorrectAndInfoSoundE()
	{
		Debug.Log("AdditionalSoundHandler->PlayIncorrectAndInfoSoundE() called");
		isPlayingIncorrectAndInfoSound = true;

		FreezeControlsHandler.Instance.FreezeControlls();
		PlaySoundToPlay();
		yield return new WaitForSeconds(audioSource.clip.length - 1f);
		FreezeControlsHandler.Instance.UnFreezeControlls();
		PlayInfoSound(0);

		isPlayingIncorrectAndInfoSound = false;
	}
}
