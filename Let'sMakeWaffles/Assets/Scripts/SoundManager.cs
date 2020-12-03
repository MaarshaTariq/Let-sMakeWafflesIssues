using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	public AudioClip[] sounds;
	public AudioSource audio;
	// Use this for initialization
	void Start () {
		instance = this;
		audio = this.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void PlaySound(int index)
	{
        audio.clip = sounds[index];
            audio.Play();
    }

    //Trying to wait before next sound plays
    //public IEnumerator _playSound(int index)
    //{
    //    Debug.Log("Playing:"+index);
    //    audio.clip = sounds[index];
    //    audio.Play();
    //    Debug.Log("Waiting for:"+audio.clip.name);
    //    Debug.Log("Waiting for:"+audio.clip.length);
    //    yield return new WaitForSeconds(audio.clip.length);
    //}


    public IEnumerator PopSounds(int count)
	{	//audio.clip = sounds [10];
		for (int i = 0; i < count; i++) {
			audio.PlayOneShot (sounds [10],1f);
			yield return new WaitForSeconds (Random.Range(0.03f,0.06f));
		}
	}

}
