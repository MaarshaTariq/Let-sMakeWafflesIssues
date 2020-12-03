using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class MenuManager : MonoBehaviour {
	
	[DllImport("__Internal")]
	private static extern void _OnGameStopped();
	[DllImport("__Internal")]
	private static extern void _ExitFullScreen();
	public GameObject mainScreen;
	public GameObject finalScreen;
	public GameObject pauseScreen;
	public GameObject playBtn;
	public float startDelay;
	public bool isMenu;
	public string SceneName;
	public static MenuManager instance;
	private bool accessibilty = false;
	public bool Accessibilty = false;
	public GameObject AccessibiltyObject;

	void OnEnable(){
		if (Accessibilty)
		AccessibiltyObject.SetActive(true);
	}
	// Use this for initialization
	void Start () {
		instance = this;
	}
	public void PauseGame()
	{
		pauseScreen.SetActive (true);
	}
	public void OpenPause()
	{
		if (GameManager.Instance.CanClick ()) {
			pauseScreen.SetActive (true);
			Time.timeScale = 0;
		}
	}
	public void StopGame(string state)
	{

		if (state == "stopFinal")
        {
			Debug.Log ("stopFinal");
			if (Screen.fullScreen)
            {
				_ExitFullScreen ();
				Screen.fullScreen = !Screen.fullScreen;
			}
			else
            {
				Debug.Log ("stoped");
				_OnGameStopped ();
			}
		}
        else
        {
			Debug.Log ("Pause Stop");
			
			Time.timeScale = 1f;
			
			Debug.Log ("Ending the game");
			if (Screen.fullScreen)
            {
				_ExitFullScreen ();
				Screen.fullScreen = !Screen.fullScreen;
			}
			else
            {
				Debug.Log ("else");
				_OnGameStopped ();
			}
		}
	}
	bool clickPlay=true;
	//Btn on start or pause
	public void PlayGame()  
	{
		if (isMenu)
        {
			Debug.Log ("isMenu" + isMenu);
			if(clickPlay)
				StartCoroutine (Play ());
			clickPlay = false;
		}
        else
        {
			Debug.Log ("pause play");
			Time.timeScale = 1f;
			pauseScreen.SetActive(false);
		}
	}
	IEnumerator Play()
	{
		SoundManager.instance.PlaySound (0);
		GameManager.Instance.ClickOff();
		StartCoroutine (GameManager.Instance.StartNewLevel(0,startDelay+1.2f));
		if(GameManager.Instance.Accessibilty)
			CloseCaption.CCManager.instance.CreateCaption (14,SoundManager.instance.sounds[0].length);
		yield return new WaitForSeconds (4f);
		mainScreen.GetComponent<Fade> ().Fadeout = true;
		isMenu = false;

	}
	public void EnableFinalScreen()
	{
		finalScreen.SetActive (true);
	}
	public void AgainGame()
	{
		//again game
		PlayerPrefs.SetInt("FullScreen", 0);
		Time.timeScale = 1f;
		StartCoroutine(GameManager.Instance.LoadScene());
	}
	public IEnumerator LoadScene()
	{
		yield return new WaitForSeconds(0.1f);
		if (Screen.fullScreen)
		{
			_ExitFullScreen();
			Screen.fullScreen = !Screen.fullScreen;
		}
		SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
	}
	public IEnumerator CallStop()
	{
		yield return new WaitUntil(() => RestAPIHandler.Instance.UploadedSuccessfully==true);
		_OnGameStopped();
	}

}
