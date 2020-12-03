using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class DummyTTS : MonoBehaviour
{
    public bool justtext;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Upload1());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator TTSTest()
    {
        Regex rgx = new Regex("\\s+");
        // Replace the "spaces" with "% 20" for the link Can be interpreted
        string result = rgx.Replace("Hello", "%20");
        string url = "https://translate.google.com/translate_tts?tl=en&q=" + result;
		Debug.Log(url);
        WWW www = new WWW(url);
        yield return www;
        // audio.clip = www.GetAudioClip(false, false, AudioType.MPEG);
        // string url = "https://translate.google.com/translate_tts?ie=UTF-8&q=hi&tl=en&client=tgx";
        // WWW ww = new WWW(url);
        // yield return ww;
        if (justtext)
        {
            TextToSpeech.ins.playAudio(www.GetAudioClip(false, false, AudioType.MPEG));
        }
    }   

    IEnumerator Upload() {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add( new MultipartFormDataSection("field1=audioContent") );
        formData.Add( new MultipartFormDataSection("{\"audioConfig\": {\"audioEncoding\": \"MP3\",\"pitch\": 0,\"speakingRate\": 1},\"input\": {\"text\": \"This is a text\"},\"voice\": {\"languageCode\": \"en-US\",\"name\": \"en-US-Wavenet-D\"}}") );

        UnityWebRequest www = UnityWebRequest.Post("https://texttospeech.googleapis.com/v1/text:synthesize?fields=audioContent&key={AIzaSyDbmWAwzxnVhXKVaGg7Gkb1d-67VwsvEoA}", formData);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("Form upload complete!");
            
        }
    }
	
    IEnumerator Upload1()
    {
        WWWForm form = new WWWForm();
        form.AddField("audioContent", "{\"audioConfig\": {\"audioEncoding\": \"MP3\",\"pitch\": 0,\"speakingRate\": 1},\"input\": {\"text\": \"This is a text\"},\"voice\": {\"languageCode\": \"en-US\",\"name\": \"en-US-Wavenet-D\"}}");

        using (UnityWebRequest www = UnityWebRequest.Post("https://texttospeech.googleapis.com/v1/text:synthesize?key=AIzaSyDbmWAwzxnVhXKVaGg7Gkb1d-67VwsvEoA", form))
        {
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
    }
}
