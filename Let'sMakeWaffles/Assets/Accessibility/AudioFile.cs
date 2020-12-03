using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFile : MonoBehaviour
{
    #region  variables
    public string[] inGameString;
    public string[] MainMenuString;
    // level text
    public string[] List1Strings;
    public string[] Target1Strings;
    public string[] Target2Strings;
    public string[] Target3Strings;
    public string[] Target4Strings;
    public string[] Target5Strings;
    public string[] Target6Strings;
    public string[] Target7Strings;
    public string[] Target8Strings;
    public string[] Target9Strings;
    public string[] Target10Strings;
    public string[] Target11Strings;
    public string[] Target12Strings;
    public string[] Target13Strings;
    public string[] Target14Strings;
    public string[] Target15Strings;


    public string[] Destination1Strings;
    public string[] Destination2Strings;
    public string[] Destination3Strings;
    public string[] Destination4Strings;
    public string[] Destination5Strings;
    public string[] Destination6Strings;
    public string[] Destination7Strings;
    public string[] Destination8Strings;
    public string[] Game92NonRepeatingStrings;
    
    public string[] endingString;
    public string[] pauseString;
    public string[] othersString;

//    [HideInInspector]
	public List<AudioClip> waitText, MainMenu, List1,Level1Target1,Level2Target1,Level3Target1,Level4Target1,Level5Target1,Level6Target1,Level7Target1,Level8Target1,Level9Target1,Level10Target1, FinalPanel, ending, others, pause,
 Level1Destination, Destination1, Destination2, Destination3, Destination4, Destination5, Destination6, Destination7
 , Destination8, infoclips, clips, Game92NonRepeating;

    [HideInInspector]
    public static AudioFile instance;
    #endregion
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
		print("Start->audiofile");
        instance = this;
        download();
    }
    public void download()
    {
        if (!GameManager.Instance.GameNumber(70, 81))
            TextToSpeech.ins.DowloadStartingSound(MainMenuString, MainMenu);

        if (!GameManager.Instance.GameNumber(92))
        {
            TextToSpeech.ins.DowloadStartingSound(inGameString, AccessibilityManager.instance.clips);
            TextToSpeech.ins.DowloadStartingSound(inGameString, AccessibilityManager.instance.infolips);
        }
        else
        {
            TextToSpeech.ins.DowloadStartingSound(inGameString, AccessibilityManager.instance.clips);
            TextToSpeech.ins.DowloadStartingSound(inGameString, AccessibilityManager.instance.infolips);
            TextToSpeech.ins.DowloadStartingSound(inGameString, clips);
            TextToSpeech.ins.DowloadStartingSound(inGameString, infoclips);
        }

        // all possibilities for target
        TextToSpeech.ins.DowloadStartingSound(List1Strings, List1);
       
            TextToSpeech.ins.DowloadStartingSound(Target1Strings, Level1Target1);
			TextToSpeech.ins.DowloadStartingSound(Target2Strings, Level2Target1);
			TextToSpeech.ins.DowloadStartingSound(Target3Strings, Level3Target1);
			TextToSpeech.ins.DowloadStartingSound(Target4Strings, Level4Target1);
			TextToSpeech.ins.DowloadStartingSound(Target5Strings, Level5Target1);
			TextToSpeech.ins.DowloadStartingSound(Target6Strings, Level6Target1);
			TextToSpeech.ins.DowloadStartingSound(Target7Strings, Level7Target1);
			TextToSpeech.ins.DowloadStartingSound(Target8Strings, Level8Target1);
			TextToSpeech.ins.DowloadStartingSound(Target9Strings, Level9Target1);
			TextToSpeech.ins.DowloadStartingSound(Target10Strings, Level10Target1);
  
//        TextToSpeech.ins.DowloadStartingSound(Target11Strings, Level4Target2);
//        TextToSpeech.ins.DowloadStartingSound(Target12Strings, Level4Target3);
//        TextToSpeech.ins.DowloadStartingSound(Target13Strings, Level5Target1);
//        TextToSpeech.ins.DowloadStartingSound(Target14Strings, Level5Target2);
//        TextToSpeech.ins.DowloadStartingSound(Target15Strings, Level5Target3);
//
//        // 4 multiple destinations
//        TextToSpeech.ins.DowloadStartingSound(Destination1Strings, Destination1);
//        TextToSpeech.ins.DowloadStartingSound(Destination2Strings, Destination2);
//        TextToSpeech.ins.DowloadStartingSound(Destination3Strings, Destination3);
//        TextToSpeech.ins.DowloadStartingSound(Destination4Strings, Destination4);
//        TextToSpeech.ins.DowloadStartingSound(Destination5Strings, Destination5);
//        TextToSpeech.ins.DowloadStartingSound(Destination6Strings, Destination6);
//        TextToSpeech.ins.DowloadStartingSound(Destination7Strings, Destination7);
//        TextToSpeech.ins.DowloadStartingSound(Destination8Strings, Destination8);
//
//
        TextToSpeech.ins.DowloadStartingSound(othersString, others);
        TextToSpeech.ins.DowloadStartingSound(endingString, ending);
        TextToSpeech.ins.DowloadStartingSound(pauseString, pause);
    }
    ///<summary>
    /// This function is used if there are any audio string instead of audio strings in audio file then we can download them 
    /// using this function.
    ///</summary>
    public void DownloadThirdPartySounds(string[] stringsToDownload, List<AudioClip> downloadedAudios)
    {
        TextToSpeech.ins.DowloadStartingSound(stringsToDownload, downloadedAudios);
    }

    public void PopulateInfoClips()
    {
		print ("AudioFIle->PopulateInfoClips(): " + AccessibilityManager.instance.clips.Count);

        if (AccessibilityManager.instance.clips.Count > 0)
            return;

        foreach (AudioClip audioClip in infoclips)
        {
            AccessibilityManager.instance.clips.Add(audioClip);
            AccessibilityManager.instance.infolips.Add(audioClip);
        }
    }
}
