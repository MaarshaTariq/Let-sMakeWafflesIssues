using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour {

	public GameObject infoBox;
	public GameObject closeBlock;
	public int infoSoundIndex;
	public Text txt;
	public string[] infoTexts;
	public int[] infoSounds;
	public static InfoManager instance;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	public void SetSoundIndex(int index)
	{
            
		infoSoundIndex = infoSounds [index];
		txt.text = infoTexts [index];
	}
	public void InfoHandler()
	{
		if (GameManager.Instance.CanClick ()) {
			StartCoroutine (OpenCloseInfo ());	
		}
	}
	IEnumerator OpenCloseInfo()
	{
		if (!infoBox.activeInHierarchy) {
			infoBox.SetActive (true);
			closeBlock.SetActive (true);
			SoundManager.instance.PlaySound (infoSoundIndex);
		} else {
			infoBox.GetComponent<Fade> ().Fadeout = true;
			yield return new WaitForSeconds (0.6f);
			infoBox.SetActive (false);
			closeBlock.SetActive (false);
			infoBox.GetComponent<Fade> ().Fadeout = false;
		}
	}
}