using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using BestHTTP;
using Newtonsoft;
using System;
using UnityEngine.Networking;
//using BestHTTP.Cookies;
//using BestHTTP.SignalR.Transports;
public class RestAPIHandler : MonoBehaviour {
//	private HTTPRequest AuthRequest;
//	private Cookie Cookie;

//	public bool IsPreAuthRequired { get; private set; }
//
//	public event OnAuthenticationSuccededDelegate OnAuthenticationSucceded;
//	public event OnAuthenticationFailedDelegate OnAuthenticationFailed;
	// Use this for initialization

	public static RestAPIHandler Instance;
	[HideInInspector]
	public bool UploadedSuccessfully=false;

	void Start () {
		Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public IEnumerator PostRequest(string url, string json)
	{
		var uwr = new UnityWebRequest(url, "POST");
		byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
		uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
		uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		uwr.SetRequestHeader("Content-Type", "application/json");

		//Send the request then wait here until it returns
		Debug.Log("waiting");
		yield return uwr.SendWebRequest();
		UploadedSuccessfully = true;
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
		}
	}
//
}
