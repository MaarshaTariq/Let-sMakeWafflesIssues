using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Newtonsoft;
using Newtonsoft.Json;
using System;
public class External : MonoBehaviour
{
    #region Previous External Logic
    [HideInInspector]
    public GameObject assetDownloader;
    public static string urlFromServer;
    public bool game92Accessibility = false;
    // Use this for initialization
    public bool Preview = false;
	public static External Instance;
	// public string key;
	//public DemoDataEnter Ref;
	public GameManager Manager;

	string baseURL;
	[HideInInspector]
	public string BaseUrl
	{
		get { return baseURL; }
		set
		{
			print("Updating baseURL: " + value);
			baseURL = value;
		}
	}
	public TextToSpeech TTSRef;
	bool IsAccessibility;
	[HideInInspector]
	public string KeyNew;
	public GameStudentModel Model;
	public bool AccessibilityLocalTest;

	void Awake()
	{
		Instance = this;
	}
	public bool isAccessibilityLocalTest;
	// Use this for initialization
	void OnEnable()
	{
		//KeyNew = "0a6f6b69f5274fe0a2e23f22ab98b502";
		Instance = this;
		//GetGameAndStudentForScoring("{"+"\"GameId\":112"+","+"\"StudentId\":null"+","+"\"IsPreview\":true"+"}");
		#if UNITY_EDITOR //for testing purposes in editor
		Invoke("ManagerAccessibility", 0);

		#endif
	}

	void ManagerAccessibility()
	{
		Debug.Log("External->OnEnable()");
		if (AccessibilityLocalTest)
		{
			EnableAccessibilty("true");
			// SetKey(key);
		}
	}


    void Start()
    {
        //		  Debug.Log("in external");
        Manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // GetUrlFromServer("https://l3skills.n2y-dev.com/api/GameAssetApi/GetGameAssets?gameId=72");
    }
    public void DisableFullScreen()
    {
        PlayerPrefs.SetInt("DisableFullScreen", 1);
    }

    public void PlayUnityScene()
    {
        Manager.StartCoroutine(Manager.LoadScene());

    }
    public void GetUrlFromServer(string url)
    {
        Debug.Log("The given url is :  " + url);
        urlFromServer = url;
        //StartCoroutine(assetDownloader.GetComponent<SoundDownloader> ().GetJsonData());
    }
    #endregion

   
   
	public bool game92;
	//void OnLevelWasLoaded(int level)
	//{
	//	if (game92) {
	//		if (level >= 0)
	//		{
	//			Debug.Log("Playerrefs Accessibility: " + PlayerPrefs.GetString ("Accessibility", "false"));
	//			game92Accessibility = PlayerPrefs.GetString ("Accessibility", "false") == "true";
	//			Debug.Log("game 92 > 0");
	//			if (AccessibilityLocalTest)
	//				StartCoroutine(GetManagerAfter(1f));

	//			Debug.Log("Game92Accessibility (" + game92Accessibility.ToString() + ")");
	//		}
	//	}
	//	else if (level >= 1)
	//	{
	//		if (AccessibilityLocalTest)
	//			StartCoroutine(GetManagerAfter(1));

	//		Debug.Log("Game92Accessibility (" + game92Accessibility.ToString() + ")");
	//	}
	//}

	IEnumerator GetManagerAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		print("On Level was loaded worked");

//		if(game92)
//			Manager = FindObjectOfType<GamaManagerFinder>().gameManager;
//		else
			Manager = FindObjectOfType<GameManager>();

		EnableAccessibilty("true");
		// SetKey(key);
	}
    public void GetBaseUrl(string uRL)
    {
        //api/GameAssetApi/GetGameAssets?gameId=PlayerScoreApi
        //	Debug.Log ("The Base Url from server is : " + BaseURL+"n2y.Web.Api.L3Skills/ApiControllers/L3Skills/Player/PlayerScoreApiController/SavePlayerScore?gameId=107");
        Debug.Log("The Base Url from server is : " + uRL);
        BaseUrl = uRL;
    }


	public void GetGameAndStudentForScoring(string Json)
	{
		Model = new GameStudentModel();
		//Debug.Log ("Json Received");
		Debug.Log("The Json from server is : " + Json);

		// if (Json.Contains("\"IsAccessibility\""))
		// {
		// }
		// else
		// {
		//     string output = Json.Replace("}", "");
		//     Json = output + "," + "\"IsAccessibility\":true" + "}";
		//     Debug.Log("Json changed: " + Json);

		// }

		if (Json.Contains("\"StudentId\":null"))
		{
			string output = Json.Replace("\"StudentId\":null", "\"StudentId\":0");
			Debug.Log("OutPut changed because student ID is Null : " + output);
			Model = JsonConvert.DeserializeObject<GameStudentModel>(output);
		}
		else
		{
			Model = JsonConvert.DeserializeObject<GameStudentModel>(Json);
		}

		Preview = Model.GetPreviewBool();

		Debug.Log("Game ID : " + Model.GetGameId());
		Debug.Log("Student ID : " + Model.GetStudentId());
		Debug.Log("IsPreview : " + Model.GetPreviewBool());

		// if (Model.GetAccessibiltyBool() == true)
		// {
		//     StartCoroutine(WaitToPerformFunctionality(true));

		// }
		// else
		// {
		//     StartCoroutine(WaitToPerformFunctionality(false));
		// }
	}

//      public void GetAccessibilty(string state)
//      { 
//          StartCoroutine(WaitToPerformFunctionality(state == "true"));
//      }

	IEnumerator WaitToPerformFunctionality(bool ISAccessibilty)
	{
//		Debug.Log("External->WaitToPerformFunctionality(ISAccessibilty: " + ISAccessibilty + ")");
		yield return new WaitForSeconds(0.15f);
//		Debug.Log ("Manager.AccessibiltyA " + Manager.Accessibilty + " -> " + ISAccessibilty);
		Manager.Accessibilty = ISAccessibilty;
//		Debug.Log ("Manager.AccessibiltyB " + Manager.Accessibilty + " -> " + ISAccessibilty);
//		Debug.Log ("Manager.AccessibiltyObject: " + Manager.AccessibiltyObject);

		Manager.AccessibiltyObject.SetActive(ISAccessibilty);
//		Debug.Log ("Manager.AccessibiltyObject: " + Manager.AccessibiltyObject);

		Manager.isExternalDone = true;
	}

	bool isAccesibilityAssigned;
	public void EnableAccessibilty(string newValue)
	{
		if (Manager.AccessibiltyObject == null)
		{
			Debug.Log ("Manager is not found so we are going to find it now");

//			if(game92)
//				Manager = FindObjectOfType<GamaManagerFinder>().gameManager;
//			else
				Manager = FindObjectOfType<GameManager>();

			if (Manager.AccessibiltyObject == null)
			{

				Debug.LogError("Accessibility is not implemented in this game. Please contact your provider.");
				return;
			}

		}


		PlayerPrefs.SetString ("Accessibility", newValue);
//		Debug.Log("Playerrefs Accessibility: " + PlayerPrefs.GetString ("Accessibility", "false") + " -> ");

		Manager.gameObject.SetActive(true);
		game92Accessibility = Manager.GameNumber(92) && newValue == "true";

//		Debug.Log("EnableAccessibilty (" + newValue + ")");
		StartCoroutine(WaitToPerformFunctionality(newValue == "true"));
	}

	public void SetKey(string NewKey)//for local functionality
	{
		KeyNew = NewKey;
		Debug.Log("External->WaitToPerformFunctionality(NewKey: " + NewKey + ")");

		Manager.Accessibilty = IsAccessibility;
		Manager.AccessibiltyObject.SetActive(IsAccessibility);

		Debug.Log("New Key Added : " + NewKey);

	}
}

[Serializable]
public class GameStudentModel
{
    public int GameId;
    public int StudentId;
    public bool IsPreview;

    public int GetGameId()
    {
        return this.GameId;
    }

    public int GetStudentId()
    {
        return this.StudentId;
    }

    public bool GetPreviewBool()
    {
        return this.IsPreview;
    }

    public void SetPreviewBool(bool isPre)
    {
        this.IsPreview = isPre;
    }

    public void SetGameId(int ID)
    {
        this.GameId = ID;
    }

    public void SetStudentId(int StID)
    {
        this.StudentId = StID;
    }

}

