using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern void _OnGameStarted();
    [DllImport("__Internal")]
    private static extern void _OnGameStopped();
    [DllImport("__Internal")]
    private static extern void _ExitFullScreen();
    public int gameNumber;
	public int sceneNumber;

    [Header("Use these to keep track of GameNumber and Version")]
    public string _gameNumber;
    public float versionNumber;
    public bool unityLogger;

    public GameObject[] panelsList;
    Fade[] mode;
	public int noOfLevels;
	public GameObject progressBars;
    public GameObject Pause_Pnl;
    public string SceneName;
    public int levelCounter = 0;
    public static GameManager Instance;
    public GameObject AccessibiltyObject;
    public float gameSpeed = 1;
    public bool changeGameSpeed;
    public int pauseStopPressedCount;
	private bool accessibilty = false;
	public bool isExternalDone = false;
    public float levelDelay;
    public bool Accessibilty
	{
		set
		{
//			Debug.Log("Set Accessibilty: " + accessibilty + " -> " + value);
			accessibilty = value;
		}
		get
		{
//			Debug.Log("Get Accessibilty: " + accessibilty);
			return accessibilty;
		}
	}
	void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
		if(Accessibilty)
			AccessibiltyObject.SetActive(true);
		
        PlayerPrefs.SetInt("Click", 0);
        //Turning off Unity Debugger for removing logs.
        print("Game Number: " + _gameNumber);
        print("Version Number: " + versionNumber);
        Debug.unityLogger.logEnabled = unityLogger;
#if !UNITY_EDITOR
        _OnGameStarted();
#endif

    }
    void Start()
    {
        mode = new Fade[panelsList.Length]; // Initializing the mode with repect to the level of panel's length

        for (int i = 0; i < panelsList.Length; i++)
        {
            mode[i] = panelsList[i].GetComponent<Fade>();
        }
    }
    void Update()
    {
        if(changeGameSpeed)
        {
            changeGameSpeed = false;
            #if UNITY_EDITOR //for testing purposes in editor
            Time.timeScale = gameSpeed;
            #endif
        }
    }
	public void PlaySound(int ind)  //play sound function with both unity audio source and html audio play
	{
		SoundManager.instance.PlaySound(ind);
		Debug.Log("GameManager->PlaySound(" + ind + ")");

		if (!Accessibilty)
			return;

		if (TextToSpeech.ins && !GameNumber(92))
			TextToSpeech.ins.StopAudio();
		if (CloseCaption.CCManager.instance)
			CloseCaption.CCManager.instance.CreateCaptionsWithSubSentences(ind, SoundManager.instance.sounds[ind].length /*Audio.clip.length*/);
	}


	void SetProgress(int count)
	{
		//Debug.Log ("count " + count * 2);
		for (int i = 0; i < count * 2; i++) {
			progressBars.transform.GetChild (i).gameObject.GetComponent<CanvasGroup> ().alpha = 1;
		}
	}
	public void ClickOn(float delay=0)
	{
		StartCoroutine (ClickHandle(delay,1));
	}
	public void ClickOff(float delay=0)
	{
		StartCoroutine (ClickHandle(delay,0));
	}
	public bool CanClick()
	{
		bool check = false;
		if (PlayerPrefs.GetInt ("Click") == 0)
			check = false;
		else if (PlayerPrefs.GetInt ("Click") == 1)
			check = true;
		return check;
			
	}
	IEnumerator ClickHandle(float delay, int state)
	{
		yield return new WaitForSeconds (delay);
		PlayerPrefs.SetInt ("Click", state);
	}
    public void OnButtonClicked(string BtnName) 
    {
        if (BtnName == "FullScrren")
        {
            Debug.Log("Full Screen");
            if (Screen.fullScreen)
            {
                _ExitFullScreen();
                Screen.fullScreen = false;
                FullScreenBtn.Instance.IMG.sprite = FullScreenBtn.Instance.FullScreenIMG[0];
            }
            else
            {
                FullScreenBtn.Instance.IMG.sprite = FullScreenBtn.Instance.FullScreenIMG[1];
                Screen.fullScreen = true;
            }
        }
       
    }
	public IEnumerator StartNewLevel(int Ind, float sec)
	{
        Debug.Log("Index:"+Ind);
        if (sec != 0)
        {
		    levelDelay = sec;
        }
		yield return new WaitForSeconds (levelDelay);

		if (levelCounter == 16) {
			panelsList [noOfLevels-1].GetComponent<Fade> ().Fadeout = true;
			yield return new WaitForSeconds (1.2f);
			panelsList [noOfLevels-1].SetActive (false);
			MenuManager.instance.EnableFinalScreen();
			Debug.Log ("completed ");
		} else {
			SetProgress (Ind + 1);

			for (int i = 0; i < panelsList.Length; i++) {
				if (i == Ind) {
					panelsList [i].SetActive (true);
                    GamePlayController.gamePlayController.changeIngredients();
				} else {
                    GamePlayController.gamePlayController.optionsDisselection();
					panelsList [i].SetActive (false);
				}
			}
			levelCounter++;
			Debug.Log ("levelcounter ++" + levelCounter);
		}
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
    public bool GameNumber(params int[] gameNumbers)
    {
		for (int i = 0; i < gameNumbers.Length; i++)
        {
            if(gameNumbers[i] == gameNumber)
                return true;
        }
        return false;
    }

	public bool SceneNumber(params int[] SceneNumbers)
	{
		for (int i = 0; i < SceneNumbers.Length; i++)
		{
			if (SceneNumbers[i] == sceneNumber)
				return true;
		}

		return false;
	}
}