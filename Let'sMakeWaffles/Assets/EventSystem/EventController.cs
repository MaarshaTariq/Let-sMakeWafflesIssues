using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft;

public class EventController : MonoBehaviour
{
	#region  variables
	public string gameTitle;
	public string gameSubTitle;
	// public GameManager Manager;
	public bool[] totalNoOfLevels;
	public int totalPossibleCorrectGuesses = 0;
//	[HideInInspector] 
	public double levelCounter = 0;
	Report report;
	GameStudentModel Model;
	//public string gameTitle;
	int gameTimeCounter = 0;
	public static EventController instance;
	private string POSTAddUserURL = "";
	[HideInInspector]
	public int correctOptionSelectionCounter = 0, wrongOptionSelectionCounter = 0;
	public bool reportSent = false;
	#endregion

	#region  function
	void Start()
	{
		instance = this;
		report = new Report();
		//Invoke ("afterdelay", 3f);
		//totalPossibleCorrectGuesses = DragAndDrop.Instance.ObjectRef.Length;

		InitializeCommonEvents();
	}

	void InitializeCommonEvents()
	{
		setDateTime();
		StartCoroutine(Gametimer(true));

		SetGameTitle(gameTitle);
		SetGameSubTitle(gameSubTitle);

	}

	public void SetLevelCounter(int level = 0)
	{
		report.setPercentage(levelCounter);
	}
	
	public void SetGameType(string gameType = "None")
	{
		report.SetGameType(gameType);
	}

	public string GetGameType()
	{
		return report.GetGameType();
	}
	public void IncreaseCorrectAnswerBy(int increment)
	{
		Debug.Log("EventController->IncreaseCorrectAnswerBy(" + increment + ")");
		int CA = report.getCorrectAttemps();
		Debug.Log("CA: " + CA);
		CA += increment;
		report.setCorrectAttempts(CA);
	}

	public void IncreaseIncorrectAnswerBy(int increment)
	{
		Debug.Log("EventController->IncreaseIncorrectAnswerBy(" + increment + ")");
		int ICA = report.getIncorrectAttempts();
		Debug.Log("ICA: " + ICA);
		ICA += increment;
		report.setIncorrectAttempts(ICA);
	}
	public void IncreaseLevelCounterBy(int increment)
	{
		levelCounter += increment;
	}

	public void SetGameTitle(string title)
	{
		report.setGametitle(title);
	}

	public void SetGameSubTitle(string subtitle)
	{
		report.setSubtitle(subtitle);
	}
	void setDateTime()
	{
		report.setDatetime(System.DateTime.UtcNow);
	}
	public void setSkillTag(string tag)
	{
		report.setAssociatedskilltag(tag);
	}
	public IEnumerator Gametimer(bool isRunning)
	{
		while (isRunning)
		{
			yield return new WaitForSeconds(1f);
			gameTimeCounter++;
			report.setPlaytime(gameTimeCounter); // in case if user quits in game and we also wanna show the played time
		}
		report.setPlaytime(gameTimeCounter);
	}
	public void StopTime()
	{
		StartCoroutine(Gametimer(false));
	}
    public void CountScreenInteraction()
    {
		if (GameManager.Instance.GameNumber(85))
        {
			if (Input.GetMouseButtonUp(0) && !GameManager.Instance.Accessibilty)
            {
                report.setResponsiveness(report.getResponsiveness() + 1);
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0) && !GameManager.Instance.Accessibilty)
            {
                report.setResponsiveness(report.getResponsiveness() + 1);
            }
        }
	}
	public void DecreaseCountForPlayPause()
	{
		if (!External.Instance.Preview) {

			report.setResponsiveness (report.getResponsiveness () - 1);
			Debug.Log ("--");
		}
	}
	public void CountScreenInteractionWithoutCheck()
	{
		if(!External.Instance.Preview)
			report.setResponsiveness(report.getResponsiveness() + 1);
	}

	public void SetResponsiveness(int increaseBy = 1)
	{
		if(!External.Instance.Preview)
			report.setResponsiveness(report.getResponsiveness() + increaseBy);
	}
	public void GameMode(string mode)
	{
		report.setSelectedmode(mode);
	}
	public void gameSubtitle(string sub)
	{
		report.setSubtitle(sub);
	}
	void istutorial(bool status)
	{
		report.isHastutorial(status);
	}
	public void AddORUpdateLetter(string key)
	{
		Report.Keys tempKey = new Report.Keys(); // creates a temporary key to hols the obj
		tempKey.key = key;// assign the input key to object

		if (report.getSelectedkeys().Exists(kt => kt.key == tempKey.key)) // checking that the provided key is present in dicsionary or not. if it is then it will go inside. 
		{
			report.getSelectedkeys().Find(x => x.key.Contains(key)).value += 1;// will find the object who has its "key" attribute equal to key that is provided and incrementing the value by 1.
		}

		else
		{
			tempKey.value = 1;
			report.getSelectedkeys().Add(tempKey);

		}

	}
	public void PrintReport()
	{
		printReport();
	}

	public void SetGamePercentage(double levelCounter)
	{
		//levelCounter++;

		report.setPercentage(levelCounter);
	}

	public void currentGamePercentage()
	{
		Debug.Log ("currentGamePercentage");
		//levelCounter++;
		if(reportSent)
            return;

		reportSent = true;

		report.setPercentage(levelCounter);

		PrintReport();
	}
	public void GussedAnswer()
	{
		report.setCorrectAttempts(correctOptionSelectionCounter);
		report.setIncorrectAttempts(wrongOptionSelectionCounter);
	}
	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		CountScreenInteraction();

		GussedAnswer();
	}

	#region comments

	#endregion
	public void sendReport()
	{
		//  StartCoroutine(Upload());
	}


	IEnumerator Upload()
	{
		// SaveItemInfo(); //this function is used to store the data in json file
		string json = JsonUtility.ToJson(report);
		UnityWebRequest www = UnityWebRequest.Post(POSTAddUserURL, json);
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(www.error);
		}
		else
		{
			Debug.Log("Form upload complete!");
		}
	}


	public void printReport()
	{
		Debug.Log ("Uploading Report");

		report.reportsSentCount++;

		if(report.reportsSentCount == 1)
			report.setFirstPostDateTime(System.DateTime.UtcNow);
		
		PlayerGameScore playerGameScore = new PlayerGameScore ();
		//		Model = new GameStudentModel (); 
		playerGameScore.Complete =(int)report.getPercentage();
		playerGameScore.CreatedBy = (External.Instance.Model.GetGameId()).ToString();
		playerGameScore.CreatedDate = report.GetFirstPostDateTime ();
		playerGameScore.Duration = (int)report.getPlaytime ();
		playerGameScore.GameId = External.Instance.Model.GetGameId();
		playerGameScore.IncorrectAttempts = report.getIncorrectAttempts();
		playerGameScore.InstanceId = 0;
		playerGameScore.IsInAccessibilityMode = GameManager.Instance.Accessibilty;
		playerGameScore.ModifiedBy = (External.Instance.Model.GetGameId()).ToString();
		playerGameScore.ModifiedDate = System.DateTime.UtcNow;
		playerGameScore.PlayType = GetGameType();
		playerGameScore.Responsiveness = report.getResponsiveness();
		playerGameScore.StudentId = External.Instance.Model.GetStudentId ();
		playerGameScore.Timestamp = report.getDatetime();

		// if (playerGameScore.StudentId == 0) {
		// 	Debug.Log ("Student Id received was 0 so setting up temporary ID : 6000409");
		// 	playerGameScore.StudentId = 6000409;
		// }

		print("percentage completed in game:" + playerGameScore.Complete);
		print("Game Created By :" + playerGameScore.CreatedBy);
		print("Game Created date :" + playerGameScore.CreatedDate);
		print("Total time played is: " + playerGameScore.Duration);
		print("Game Id is : " + playerGameScore.GameId);
		print("Total Incorrect Attempts : " + playerGameScore.IncorrectAttempts);
		print("Instance Id Of the game : " + playerGameScore.InstanceId);
		print("InAccessibilityMode Of the game : " + playerGameScore.IsInAccessibilityMode);
		print("Game Modified By : " + playerGameScore.ModifiedBy);
		print("Game Modified Date : " + playerGameScore.ModifiedDate);
		print("Game play type : " + playerGameScore.PlayType);
		print("Total number of interaction in game (responsiveness):" + playerGameScore.Responsiveness);
		print("Student Id in Game :" + playerGameScore.StudentId);
		print("Date and time  is: " + playerGameScore.Timestamp);
		//        print("Subtitle is: " + report.getSubtitle());
		//        print("has tutorial: " + report.getHastutorial());
		//        print("Corrected guess are: " + report.getCorrectAttemps());
		
		string JsonString = JsonConvert.SerializeObject (playerGameScore);
		Debug.Log (JsonString);
		string Url=External.Instance.BaseUrl + "api/PlayerScoreApi/SavePlayerScore";
		Debug.Log ("URl used for posting : "+Url);
		RestAPIHandler.Instance.StartCoroutine(RestAPIHandler.Instance.PostRequest(Url,JsonString));

	}


	#endregion
}
[Serializable]
public class PlayerGameScore
{
	public int? Complete { get; set; }
	public string CreatedBy { get; set; }
	public DateTime CreatedDate { get; set; }
	public int Duration { get; set; }
	public int GameId { get; set; }
	public int? IncorrectAttempts { get; set; }
	public int InstanceId { get; set; }
	public  bool IsInAccessibilityMode { get; set; }
	public string ModifiedBy { get; set; }
	public DateTime ModifiedDate { get; set; }
	public string PlayType { get; set; }
	public int? Responsiveness { get; set; }
	public int StudentId { get; set; }
	public DateTime Timestamp { get; set; }
}
