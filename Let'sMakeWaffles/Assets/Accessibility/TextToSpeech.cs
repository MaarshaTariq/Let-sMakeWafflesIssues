using UnityEngine.UI;
//using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Newtonsoft;
using Newtonsoft.Json;

public class GetAudio
{
    public string mp3FileUrl { get; set; }
    public object timingFileUrl { get; set; }
    public List<int> timings { get; set; }
}

public class TextToSpeech : MonoBehaviour
{
    public int i = 1, cookieNum = 1;

    [System.Serializable]
    public class InfoTextSettings
    {
        [Tooltip("To check whether it is 1st correct option or second")] public int optionNumber;
        public int singularOffset;
        public int pluralOffset;
        public int singularOffset2;
        public int pluralOffset2;
        public int Part4TextIndex;
        public int objectCount;
        [Tooltip("Offsets to maintain flow of info text by removing empty part of sound from last")]
        public bool[] allowSounds = { true };
        public float[] offsets;

    }
    public InfoTextSettings infoTextSettings;
    [HideInInspector]
    public string tempStringHolder = "";

    bool wait = false;
    AudioFile audioFile;
    private bool n2y = false;
    AudioClip TempAudioClip;//By Humza
    public External ExternalRef;
    // private string key = "1246840dec93420a9e1f199bfdb3ae85"; //arslan
    // private string key = "d4856606d9de439aa37ce4d9e53ec0e1";//zaki
    public string key;//humza
    // private string key = "dd5d7684deda4da9b7d105dbfd636aad";
    //private string key = "";
    public AudioSource source;
    private AudioClip clip;
    //public InputField inputText;
    public bool LocalTTS;//false for main n2y build , true for local build 																	
    string text;
    [HideInInspector]
    //public Queue<AudioClip> sound = new Queue<AudioClip>();
    public static TextToSpeech ins;
    public bool justtext = false;
    public int TotalAudioToDownload;//total audio should be downloaded first before game is started
    // [HideInInspector]
    public int AudioDownloaded;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void OnEnable()
    {

        // playtext("Kindly navigate the game using buttons");
        if (GetComponent<AudioSource>() != null)
            source = GetComponent<AudioSource>();
        ins = this;

        audioFile = GetComponent<AudioFile>();
    }
    IEnumerator downloadAudio(AccessibiltyObject t)
    {
        if (LocalTTS)
        {
            Regex rg = new Regex("\\s+");
            string result = rg.Replace(text, "+");

            string url = "http://api.voicerss.org/?key=" + ExternalRef.KeyNew + "&hl=en-us&src=" + result + "&c=WAV";
            Debug.Log("Key Used : " + ExternalRef.KeyNew);
            WWW ww = new WWW(url);
            yield return ww;
            if (justtext)
            {
                playAudio(ww.GetAudioClip(false, false, AudioType.WAV));
            }
            else
            {
                if (t != null)
                    t.clip = ww.GetAudioClip(false, false, AudioType.WAV);
            }

        }
        else
        {
            Regex rg = new Regex("\\s+");
            string result = rg.Replace(text, "+");
            string url = ExternalRef.BaseUrl + "api/speechapi/GetDynamicSpeechData?text=" + result + "&speed=30&volume=90&speechLanguage=en";//n2y server
            WWW ww = new WWW(url);
            yield return ww;
            Debug.Log("Audio downloaded 2: " + ww.text);
            GetAudio Audio = JsonConvert.DeserializeObject<GetAudio>(ww.text);
            string NewUrl = Audio.mp3FileUrl;
            WWW wwNew = new WWW(NewUrl);
            yield return wwNew;
            Debug.Log("Done waiting now storing audio " + NewUrl + " " + wwNew.text + "  " + result);
            if (justtext)
                playAudio(wwNew.GetAudioClip(false, false, AudioType.MPEG));
            else
            {

                if (t != null)
                    t.clip = wwNew.GetAudioClip(false, false, AudioType.MPEG);
            }


            justtext = false;
        }

    }
    public void DownloadAllAudios(AccessibiltyObject t)
    {
        if (t.gameobject.GetComponent<TextToSpeak>())
        {
            text = t.gameobject.GetComponent<TextToSpeak>().textToSpeak;
        }
        else
        {
            text = t.gameobject.name;
        }
        StartCoroutine(downloadAudio(t));
    }
    public void playAudio(AudioClip clip)
    {
        Debug.Log("TextToSpeech->playAudio(" + (clip != null).ToString() + ")");

        if (clip != null)
        {
            // print("playing sound");

            source.clip = clip;
            source.Play();
        }

    }

    public void StopAudio()
    {
        Debug.Log("TextToSpeech->StopAudio(" + source.isPlaying + ")");

        if (source.isPlaying)
        {
            Debug.Log("if (true)");
            source.Stop();
        }
    }

    UnityEngine.Coroutine PlayRemainingInfoTextC;
    bool PRITCIR = false; // Play Remaining Info Text Coroutine Is Running
    public void PlayRemainingInfoText(AccessibiltyObject selectedObject = null)
    {
        if (!PRITCIR)
            PlayRemainingInfoTextC = StartCoroutine(PlayRemainingInfoTextE(selectedObject));
        else
        {
            StopCoroutine(PlayRemainingInfoTextC);
            source.Stop();
            PRITCIR = false;
            PlayRemainingInfoTextC = StartCoroutine(PlayRemainingInfoTextE(selectedObject));
        }
    }

    IEnumerator PlayRemainingInfoTextE(AccessibiltyObject selectedObject)
    {
        PRITCIR = true;
        FreezeControlsHandler.Instance.FreezeControlls();

        // 1. Place
        if (infoTextSettings.allowSounds[0])
        {
            playAudio(audioFile.others[0]);
            yield return new WaitForSeconds(audioFile.others[0].length - infoTextSettings.offsets[0]);
        }

        // 2. Object(s) count
        if (infoTextSettings.allowSounds[1])
        {
            playAudio(audioFile.others[infoTextSettings.objectCount]);
            yield return new WaitForSeconds(audioFile.others[infoTextSettings.objectCount].length - infoTextSettings.offsets[1]);
        }

        // 3. Object(s) name
        if (infoTextSettings.allowSounds[2])
        {
            int objectNameIndex = 0;

            if (infoTextSettings.objectCount == 1)
            {
                if (infoTextSettings.optionNumber % 2 == 0)
                    objectNameIndex = AccessibilityManager.instance.LevelCounter + 1 + infoTextSettings.singularOffset;
                else
                    objectNameIndex = AccessibilityManager.instance.LevelCounter + 1 + infoTextSettings.singularOffset2;
            }
            else
            {
                if (infoTextSettings.optionNumber % 2 == 0)
                    objectNameIndex = AccessibilityManager.instance.LevelCounter + 1 + infoTextSettings.pluralOffset;
                else
                    objectNameIndex = AccessibilityManager.instance.LevelCounter + 1 + infoTextSettings.pluralOffset2;
            }

            playAudio(audioFile.others[objectNameIndex]);
            yield return new WaitForSeconds(audioFile.others[objectNameIndex].length - infoTextSettings.offsets[2]);
        }

        // 4. Holder object
        if (infoTextSettings.allowSounds[3])
        {
            playAudio(audioFile.others[infoTextSettings.Part4TextIndex]);
            yield return new WaitForSeconds(audioFile.others[infoTextSettings.Part4TextIndex].length - infoTextSettings.offsets[3]);
            //arslan
            if (GameManager.Instance.GameNumber(81))
            {
                GameManager.Instance.PlaySound(114);
                yield return new WaitForSeconds(audioFile.others[infoTextSettings.Part4TextIndex].length - infoTextSettings.offsets[3]);

            }
        }

        FreezeControlsHandler.Instance.UnFreezeControlls();
        //arslan
        if (GameManager.Instance.GameNumber(81))
        {
            FreezeControlsHandler.Instance.FreezeControlls();
            if (audioFile.others[infoTextSettings.Part4TextIndex].length == 0)
                yield return new WaitForSeconds(1f);


        }
        // 5. Selected object sound
        if (selectedObject != null)
        {
            yield return new WaitForSeconds(infoTextSettings.offsets[4]);
            playAudio(selectedObject.clip);
            CloseCaption.CCManager.instance.CreateCaption
            (
                selectedObject.text,
                selectedObject.clip.length
            );
            //arslan
            if (GameManager.Instance.GameNumber(81))
            {
                yield return new WaitForSeconds(selectedObject.clip.length);
            }
        }
        //arslan
        if (GameManager.Instance.GameNumber(81))
        {
            FreezeControlsHandler.Instance.UnFreezeControlls();

        }

        PRITCIR = false;
    }

    public void StopPlayRemainingInfoTextE()
    {
        if (PRITCIR && PlayRemainingInfoTextC != null)
        {
            StopCoroutine(PlayRemainingInfoTextC);
            PRITCIR = false;
        }
    }

    public void Game81Info()
    {
        StartCoroutine(Game81InfoE());
    }

    IEnumerator Game81InfoE()
    {
        CloseCaption.CCManager.instance.CreateCaption(AudioFile.instance.othersString[0] + " " + AudioFile.instance.othersString[1] + " " + AudioFile.instance.othersString[2], 5f);

        TextToSpeech.ins.playAudio(AudioFile.instance.others[0]);
        yield return new WaitForSeconds(AudioFile.instance.others[0].length);
        TextToSpeech.ins.playAudio(AudioFile.instance.others[1]);
        yield return new WaitForSeconds(AudioFile.instance.others[1].length);
        TextToSpeech.ins.playAudio(AudioFile.instance.others[2]);
        yield return new WaitForSeconds(AudioFile.instance.others[2].length);
        TextToSpeech.ins.playAudio(AudioFile.instance.others[3]);
        yield return new WaitForSeconds(AudioFile.instance.others[3].length);
        // Debug.LogError("DOne");
    }

    public float GetPlayRemainingInfoTextLength()
    {
        int objectNameIndex = 0;

        if (infoTextSettings.objectCount == 1)
            objectNameIndex = AccessibilityManager.instance.LevelCounter + 1 + infoTextSettings.singularOffset;
        else
            objectNameIndex = AccessibilityManager.instance.LevelCounter + 1 + infoTextSettings.pluralOffset;

        float[] part = { 0f, 0f, 0f, 0f };

        if (infoTextSettings.allowSounds[0])
            part[0] = audioFile.others[0].length;

        if (infoTextSettings.allowSounds[1])
            part[1] = audioFile.others[infoTextSettings.objectCount].length;

        if (infoTextSettings.allowSounds[2])
            part[2] = audioFile.others[objectNameIndex].length;

        if (infoTextSettings.allowSounds[3])
            part[3] = audioFile.others[infoTextSettings.Part4TextIndex].length;

        return part[0] + part[1] + part[2] + part[3];
    }

    public string GetPlayRemainingInfoText()
    {
        int objectNameIndex = 0;

        if (infoTextSettings.objectCount == 1)
            objectNameIndex = AccessibilityManager.instance.LevelCounter + 1 + infoTextSettings.singularOffset;
        else
            objectNameIndex = AccessibilityManager.instance.LevelCounter + 1 + infoTextSettings.pluralOffset;

        string[] part = { "", "", "", "" };

        if (infoTextSettings.allowSounds[0])
            part[0] = audioFile.othersString[0] + ' ';

        if (infoTextSettings.allowSounds[1])
            part[1] = audioFile.othersString[infoTextSettings.objectCount] + ' ';

        if (infoTextSettings.allowSounds[2])
            part[2] = audioFile.othersString[objectNameIndex] + ' ';

        if (infoTextSettings.allowSounds[3])
            part[3] = audioFile.othersString[infoTextSettings.Part4TextIndex];

        return part[0] + part[1] + part[2] + part[3] + ".";
    }

    public void playAudioThroughText(string t)
    {
        justtext = true;
        text = t;
        StartCoroutine(downloadAudio(null));
    }

    public void DowloadStartingSound(string[] text, List<AudioClip> clips)
    {
        clips.Clear();
        // clips = new List<AudioClip>();
        StartCoroutine(Download(text, clips));
    }

    public void DowloadRepeatingSound(string[] text)
    {
        StartCoroutine(DownloadR(text));
    }

    public IEnumerator DownloadInbetweenSounds(string text)
    {
        if (LocalTTS)
        {
            Regex rg = new Regex("\\s+");
            string result = rg.Replace(text, "+");
            string url = "http://api.voicerss.org/?key=" + ExternalRef.KeyNew + "&hl=en-us&src=" + result + "&c=WAV";
            Debug.Log("Key Used : " + ExternalRef.KeyNew);
            WWW ww = new WWW(url);
            yield return ww;
            playAudio(ww.GetAudioClip(false, false, AudioType.WAV));


        }
        else
        {
            Regex rg = new Regex("\\s+");
            string result = rg.Replace(text, "+");

            string url = ExternalRef.BaseUrl + "api/speechapi/GetDynamicSpeechData?text=" + result + "&speed=30&volume=90&speechLanguage=en";//n2y server
            WWW ww = new WWW(url);
            yield return ww;
            Debug.Log("Audio downloaded : " + ww.text);
            GetAudio Audio = JsonConvert.DeserializeObject<GetAudio>(ww.text);
            string NewUrl = Audio.mp3FileUrl;
            WWW wwNew = new WWW(NewUrl);
            yield return wwNew;
            Debug.Log("Done waiting now storing audio " + NewUrl + " " + wwNew.text + "  " + result);
            playAudio(wwNew.GetAudioClip(false, false, AudioType.MPEG));
        }
    }

    // This array contains words which are not getting converted into audio in thier normal forms
    // e.g audio of banana is only generating when written as bANANA
	//
	// ok
	//'';
    public Dictionary<string, string> n2ySpecialWords = new Dictionary<string, string>()
    {
		{"Let’s sell bags of popcorn! Here, we put popcorn packs in boxes. Be sure to count by 5s. Let’s go!","Let’s sell bags of popcorn! Here, we put popcorn packs in boxes. Be sure to count by fives. Let’s go!"},
        
		{"Counting by 5s, drag","Counting by fives, drag"},// Game 52
		{"Counting by 2s, drag","Counting by twoes, drag"},
		{"blanket", "bLANKET"},
		{"turkey & cheese sandwich $4.50","turkey and cheese sandwich $4.50"},
        {"basketball", "bASKETBALL"},
        // Game 52
        {"20", "TWENTY"},
        {"30", "THIRTY"},
        {"40", "FORTY"},
        {"50", "FIFTY"},
        {"60", "SIXTY"},
        {"70", "SEVENTY"},
        {"80", "EIGHTY"},
        {"90", "NINETY"},
        // Game 66
        {"Errands: One-Up Strategy!", "Errands one up strategy"},
        {"$0.00", "zero dollar"},
        {"$1.00", "one dollar"},
        {"$2.00", "TWO dollars"},
        {"$3.00", "THREE dollars"},
        {"$4.00", "FOUR dollars"},
        {"$5.00", "FIVE dollars"},
        {"$6.00", "SIX dollars"},
        {"$7.00", "SEVEN dollars"},
        {"$8.00", "EIGHT dollars"},
        {"$9.00", "NINE dollars"},
        {"$10.00", "TEN dollars"},
        {"$11.00", "11 dollars"},

        //arslan
        // {"You did it!","You did it"},
        // {"$2.00 and one up. He needs $3.00 to buy the dog bowl.","Two dollars and one up. He needs 3 dollars to buy the dog bowl."},
        // {"$4.00 and one up. He needs $5.00 to buy the dog collar.","Four dollars and one up. He needs 5 dollars to buy the flea collar."},
        // {"$3.00 and one up. He needs $4.00 to buy the dog toy.","Three dollars and one up. He needs 4 dollars to buy the dog toy."},
        // {"$5.00 and one up. He needs $6.00 to buy the dog shampoo.","Five dollars and one up. He needs 6 dollars to buy the dog shampoo."},
        // {"$1.00 and one up. He needs $2.00 to buy the pickles.","One dollar and one up. He needs 2 dollars to buy the pickles."},
        // {"$2.00 and one up. He needs $3.00 dollars to buy the bread.","Two dollars and one up. He needs 3 dollars to buy the bread."},
        // {"$3.00 and one up. He needs $4.00 to buy the apples.","Three dollars and one up. He needs 4 dollars to buy the apples."},
        // {"$11.00 and one up. He needs $12.00 to buy all of the items.","Eleven dollars and one up. He needs 12 dollars to buy all of the items."},

        // {"Dog bowl costs $2.19.","Dog bowl costs 2.19 dollars. " },
        // {"Dog collar costs $4.45." ,"Dog collar costs 4.45 dollars."},
        // {"Dog toy costs $3.59.","Dog toy costs 3.59 dollars."},
        // {"Dog shampoo costs $5.88.","Dog shampoo costs 5.88 dollars."},
        // {"Pickles cost $1.29.","Pickles costs 1.29 dollars."},
        // {"Bread costs $2.29.","Bread costs 2.29 dollars.  "},
        // {"Bag of apples costs $3.29.","Bag of apples costs 3.29 dollars."},
        // {"Root beer, frozen pizza and music gift card costs $11.28.","Root beer, frozen pizza and music gift card costs 11.28 dollars."},
      
      //game 69
      {"Oh great! I love popcorn. I’ll take five bags of popcorn.","Oh great. I love popcorn. I’ll take five bags of popcorn."},
      {"Looks delicious. I’ll take fifteen bags of popcorn.","Looks delicious. I’ll take 15 bags of popcorn."},
      {"Great after-school snack. May I have twenty-five bags, please?","Great after school snack. May I have 25 bags please"},
      {"What a good deal. Could I have thirty-five bags, please?","What a good deal. Could I have 35 bags please"},
      {"I have to take snacks to our club meeting. May I have forty bags, please?","I have to take snacks to our club meeting. May I have 40 bags please"},
      {"I’ll buy some for my family picnic this weekend. Let’s see...I need 45 bags, please.","I’ll buy some for my family picnic this weekend. Let’s see. I need 45 bags please"},
      
        // Game 77
        {"$3333", "11 dollars"},
        {"Pick, Pack, Ship!", "Pick, pack, ship"},
        {"hat", "hAT"},
        //Game 92
        {"Snacks cost $1.00-$2.00.", "Snacks cost 1$ to 2$."},
        {"Sandwiches cost $4.50-$6.50.", "Sandwiches cost 4.5$ to 6.5$."},
        {"5-dollar bill", "5 dollar bill"},
        {"lemon-lime", "lemon lime"},
        //Game 93
    //      {"/", "forward slash"},
          {".", "fullStop"},
          {"Ship It!","Ship iT"},
          {"That was awesome!","That was awesome"},
          {"[","Opening Square Bracket"},
          {"]","Closing Square Bracket"},
          {"'","Single Quote"},
          {",","Comma"},
          {"Where's the phone?","Where's the phone"}
    };

    public Dictionary<string, AudioClip> downloadedWords = new Dictionary<string, AudioClip>();
    IEnumerator Download(string[] text, List<AudioClip> clips)
    {
        if (LocalTTS)
        {
            if (googleTTSSettings.useGoogleTTS)
                GoogleTextToSpeech(text, clips);
            else
            {
                for (int i = 0; i < text.Length; i++)
                {
                    Regex rg = new Regex("\\s+");
                    string result = rg.Replace(text[i], "+");
                    string url = "http://api.voicerss.org/?key=" + ExternalRef.KeyNew + "&hl=en-us&src=" + result + "&c=WAV";
                    Debug.Log("Key Used : " + ExternalRef.KeyNew);
                    WWW ww = new WWW(url);
                    yield return ww;
                    clips.Add(ww.GetAudioClip(false, false, AudioType.WAV));
                    AudioDownloaded++;
                    Debug.Log(AudioDownloaded);
                }
            }
        }
        // else if (GameManager.Instance.GameNumber(92))
        // {
        //     print("Game 92 Test in TTS");
        //     print("************************************* [START] " + text.Length + " *************************************");
        //     for (int i = 0; i < text.Length; i++)
        //     {
        //         Debug.Log("Request for [" + i + "] \"" + text[i] + "\" is sent");

        //         Regex rg = new Regex("\\s+");
        //         string val;
        //         string result = n2ySpecialWords.TryGetValue(text[i], out val) ? rg.Replace(val, "+") : rg.Replace(text[i].ToUpper(), "+");

        //         string url = ExternalRef.BaseUrl + "api/speechapi/GetDynamicSpeechData?text=" + result + "&speed=30&volume=90&speechLanguage=en";//n2y server

        //         if (!downloadedWords.ContainsKey(text[i]))
        //         {
        //             downloadedWords.Add(text[i], null);

        //             WWW ww = new WWW(url);
        //             yield return ww;
        //             Debug.Log("Audio downloaded: " + ww.text);
        //             GetAudio Audio = JsonConvert.DeserializeObject<GetAudio>(ww.text);
        //             string NewUrl = Audio.mp3FileUrl;
        //             WWW wwNew = new WWW(NewUrl);
        //             yield return wwNew;
        //             Debug.Log("Done waiting now storing audio [" + i + "]:" + NewUrl + " " + wwNew.text);
        //             downloadedWords[text[i]] = wwNew.GetAudioClip(false, false, AudioType.MPEG);
        //         }
        //         else Debug.Log("\"" + text[i] + "\" already exists [" + i + "]");
        //     }

        //     print("************************************* [Middle] " + text.Length + " *************************************");
        //     for (int i = 0; i < text.Length; i++)
        //     {
        //         clips.Add(downloadedWords[text[i]]);
        //         AudioDownloaded++;
        //         print("AudioDownloaded: " + AudioDownloaded);
        //     }
        //     print("************************************* [END] " + text.Length + " *************************************");
        // }
        else
        {
            for (int i = 0; i < text.Length; i++)
            {
                Debug.Log("Request for \"" + text[i] + "\" is sent");
                Regex rg = new Regex("\\s+");
                string val;
                string temp = "";
                bool d = false;
                string result = null;
                if (text[i].Contains("Ship It!"))
                {
                    // temp = text[i];
                    // Debug.Log("## " + text[i]);
                    // text[i] = "ship it " + tempStringHolder;
                    // d = true;
                    // Debug.Log("##2 " + text[i]);
                    result = n2ySpecialWords.TryGetValue(text[i], out val) ? rg.Replace(val, "+") : rg.Replace(text[i], "+");
                }
                else
                    result = n2ySpecialWords.TryGetValue(text[i], out val) ? rg.Replace(val, "+") : rg.Replace(text[i].ToUpper(), "+");
                // if (d)
                // {
                //     text[i] = temp;
                //     Debug.Log("##3 " + text[i]);
                //     d = false;
                // }

                print("TextToSpeech->ExternalRef.BaseUrl: " + ExternalRef.BaseUrl);


                string url = ExternalRef.BaseUrl + "api/speechapi/GetDynamicSpeechData?text=" + result + "&speed=30&volume=90&speechLanguage=en";//n2y server
                print(url);

                WWW ww = new WWW(url);
                yield return ww;
                Debug.Log("#Audio downloaded : " + ww.text);
                GetAudio Audio = JsonConvert.DeserializeObject<GetAudio>(ww.text);
                string NewUrl = Audio.mp3FileUrl;
                WWW wwNew = new WWW(NewUrl);
                yield return wwNew;
                Debug.Log("Done waiting now storing audio " + NewUrl + " " + wwNew.text);
                clips.Add(wwNew.GetAudioClip(false, false, AudioType.MPEG));
                AudioDownloaded++;
            }
        }
    }

    IEnumerator DownloadR(string[] text)
    {
        print("Game 92 Test in TTS");
        for (int i = 0; i < text.Length; i++)
        {
            Debug.Log("Request for [" + i + "] \"" + text[i] + "\" is sent");

            Regex rg = new Regex("\\s+");
            string val;
            string result = n2ySpecialWords.TryGetValue(text[i], out val) ? rg.Replace(val, "+") : rg.Replace(text[i].ToUpper(), "+");

            string url = ExternalRef.BaseUrl + "api/speechapi/GetDynamicSpeechData?text=" + result + "&speed=30&volume=90&speechLanguage=en";//n2y server

            WWW ww = new WWW(url);
            yield return ww;
            Debug.Log("Audio downloaded: " + ww.text);
            GetAudio Audio = JsonConvert.DeserializeObject<GetAudio>(ww.text);
            string NewUrl = Audio.mp3FileUrl;
            WWW wwNew = new WWW(NewUrl);
            yield return wwNew;
            Debug.Log("Done waiting now storing audio [" + i + "]:" + NewUrl + " " + wwNew.text + " : " + text[i]);
            downloadedWords.Add(text[i], wwNew.GetAudioClip(false, false, AudioType.MPEG));
        }

        print("Dictionary Data: ");
        for (int i = 0; i < downloadedWords.Count; i++)
        {
            if (downloadedWords.ContainsKey(text[i]) && downloadedWords[text[i]] != null)
                print("Dictionary Data [" + text[i] + "]: " + downloadedWords[text[i]]);
            else
                print("Dictionary Data [" + text[i] + "]: null");
        }

        AssignClips(AudioFile.instance.Target1Strings, AudioFile.instance.Level1Target1);
        AssignClips(AudioFile.instance.Target2Strings, AudioFile.instance.Level2Target1);
        AssignClips(AudioFile.instance.Target3Strings, AudioFile.instance.Level3Target1);
        AssignClips(AudioFile.instance.Target4Strings, AudioFile.instance.Level4Target1);
        AssignClips(AudioFile.instance.Target5Strings, AudioFile.instance.Level5Target1);
        AssignClips(AudioFile.instance.Target6Strings, AudioFile.instance.Level6Target1);
        AssignClips(AudioFile.instance.Target7Strings, AudioFile.instance.Level7Target1);
        AssignClips(AudioFile.instance.Target8Strings, AudioFile.instance.Level8Target1);
        AssignClips(AudioFile.instance.Target9Strings, AudioFile.instance.Level9Target1);
        AssignClips(AudioFile.instance.Target10Strings, AudioFile.instance.Level10Target1);
    }

    public void AssignClips(string[] text, List<AudioClip> clips)
    {
        for (int i = 0; i < text.Length; i++)
        {
            clips.Add(downloadedWords[text[i]]);
            AudioDownloaded++;
            print("AudioDownloaded: " + AudioDownloaded);
        }
    }

    #region Google Text to speech

    [Serializable]
    public class GoogleTTSSettings
    {
        [Serializable]
        public class DowloadedDataDetails
        {
            public int downloadedAudios;
            public int alreadyDownloadedAudios;
        }
        public DowloadedDataDetails dowloadedDataDetails;

        public bool useGoogleTTS = true;
        public bool isDownloaded;
        public string url = "https://texttospeech.googleapis.com/v1/text:synthesize?fields=audioContent&key=";
        public string path = "Assets/Resources/Sounds/";
        public string APIKey = "AIzaSyDbmWAwzxnVhXKVaGg7Gkb1d-67VwsvEoA";
    }

    public GoogleTTSSettings googleTTSSettings;
    string url;
    string bodyData;

    public void GoogleTextToSpeech(string[] text, List<AudioClip> clips)
    {
        // url = "https://texttospeech.googleapis.com/v1/text:synthesize?fields=audioContent&key=" + APIKey;
        url = googleTTSSettings.url + googleTTSSettings.APIKey;
        string val;
        for (int i = 0; i < text.Length; i++)
        {
            if (!googleTTSSettings.isDownloaded)
            {
                if (ReturnClipofName(n2ySpecialWords.TryGetValue(text[i], out val) ? val : text[i]) == null)
                {
                    bodyData = "{\"audioConfig\": {\"pitch\": 0,\"audioEncoding\": \"MP3\",\"speakingRate\": 1},\"input\": {\"ssml\": \"<speak>" + text[i] + "</speak>\"},\"voice\": {\"languageCode\": \"en-US\",\"name\": \"en-US-Wavenet-E\", \"ssmlGender\":\"FEMALE\"}}";
                    StartCoroutine(PostRequest(url, bodyData, n2ySpecialWords.TryGetValue(text[i], out val) ? val : text[i]));
                }
                else
                {
                    Debug.Log("\"" + text[i] + "\" is already available.");
                    googleTTSSettings.dowloadedDataDetails.alreadyDownloadedAudios++;
                }
            }
            else
                clips.Add(ReturnClipofName(n2ySpecialWords.TryGetValue(text[i], out val) ? val : text[i]));
        }
    }

    IEnumerator PostRequest(string url, string json, string audioName)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Request for \"" + audioName + "\" is sent.");
        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            // Debug.Log("Received: " + uwr.downloadHandler.text);

            string base64Text = uwr.downloadHandler.text;
            MakeAudioClip(base64Text.Substring(21, base64Text.Length - 25), audioName);
            Debug.Log("Request for \"" + audioName + "\" is done.");
            googleTTSSettings.dowloadedDataDetails.downloadedAudios++;
        }
    }

    void MakeAudioClip(string base64Text, string audioName)
    {
        MakeDirectoryIfNotExists();
        byte[] bytes = System.Convert.FromBase64String(base64Text);
        File.WriteAllBytes(googleTTSSettings.path + audioName + ".wav", bytes);
    }

    AudioClip ReturnClipofName(string audioName)
    {
        //print(audioName);
        MakeDirectoryIfNotExists();
        AudioDownloaded++;
//        print("AudioDownloaded: " + AudioDownloaded);
        return Resources.Load<AudioClip>("Sounds/" + audioName);
    }

    void MakeDirectoryIfNotExists()
    {
        if (!Directory.Exists("Assets/Resources/Sounds"))
            Directory.CreateDirectory("Assets/Resources/Sounds");
    }

    #endregion

    public void playLongAudio(int i, int cookieNum)
    {
        if (GameManager.Instance.Accessibilty)
        {
            if (wait == false)
                StartCoroutine(playIT(i, cookieNum));
        }
    }
    public void playLongAudio()
    {
        if (GameManager.Instance.Accessibilty)
        {
            if (wait == false)
                StartCoroutine(playIT(i, cookieNum));
        }

    }

    public void PlayOtherSounds(int index)
    {
        if (-1 < index && index < audioFile.othersString.Length)
            playAudio(audioFile.others[index]);
        else
            Debug.Log("TextToSpeech->PlayOtherSounds() recieving an invalid index");
    }

    IEnumerator playIT(int i, int cookieNum)
    {
        yield return new WaitForSeconds(0.1f);
        // float gap = 0.75f;
        // wait = true;
        // AccessibilityManager.instance.ToggleNaviagtion(false); //arslan
        // yield return new WaitForSeconds(01);
        // //kindly select
        // source.clip = AudioFile.instance.others[0];
        // source.Play();
        // yield return new WaitForSeconds(source.clip.length-gap);
        // //num of cookie

        // source.clip = AudioFile.instance.numbers[i];
        // source.Play();
        // yield return new WaitForSeconds(source.clip.length-gap);
        // //cookie nam
        // if (i == 0)
        // {
        //     source.clip = AudioFile.instance.cookie[cookieNum];
        //     source.Play();
        //     yield return new WaitForSeconds(source.clip.length-gap);
        // }
        // else
        // {
        //     source.clip = AudioFile.instance.cookies[cookieNum];
        //     source.Play();
        //     yield return new WaitForSeconds(source.clip.length-gap);
        // }
        // AccessibilityManager.instance.ToggleNaviagtion(true); //arslan
        // wait = false;
    }
}
