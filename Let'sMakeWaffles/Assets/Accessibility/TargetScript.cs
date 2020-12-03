using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetScript : MonoBehaviour, AcessibilityInterface
{
    #region Variables
	public bool game92;
    public int emptySoundIndex;
    public string uniqueText = "panelOne";
    public float timeToWaitToPlayTextATStart = 3;
    public int soundIndex = 2;
    public float infoOffset;
    public string textToSayOnWait;
    public bool removeTarget = false;
    public bool enablePause = true;
    public bool EnableAccessability = false;
    public bool downloadSounds = true;
    public bool UseRemainingSound;
    public List<GameObject> list;
    public List<AccessibiltyObject> accesabilityObject;
    public GameObject greenbox;
    RectTransform greenTransform;
    int currentListIndex = 0;
    bool enableTouch = false;
    public bool isButton = false;
    GameObject buttonObject;
    GameObject tempObject;
    AccessibiltyObject tempAObj;
    bool isInfoTextPlaying = false;

    #endregion
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        Debug.Log(gameObject.name + "->OnDisable()");
        //Debug.Log("TargetScript(" + uniqueText + ")->OnDisable()");

        if (GameManager.Instance.Accessibilty)
        {
            // print("Going to disable the target script for object " + gameObject.name);

            accesabilityObject.Clear();
            currentListIndex = 0;
            isButton = false;
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void OnEnable()
    {
        OnEnableFunctionality();
    }


    public void OnEnableFunctionality()
    {
		
        if (AccessibilityManager.instance == null)
            return;

        Debug.Log("TargetScript->OnEnable()");
        //arslan
        if (downloadSounds && !GameManager.Instance.GameNumber(81))
        {
            PopulateAccessibiltyList();
        }

        AccessibilityManager.instance.placee = uniqueText;
        AccessibilityManager.instance.enablepause = true;

        currentListIndex = 0;

        greenTransform = greenbox.GetComponent<RectTransform>();
        if (!GameManager.Instance.GameNumber(81))
            GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>

    public void PopulateAccessibiltyList()
    {
        Debug.Log("TargetScript->PopulateAccessibiltyList(" + GameManager.Instance.gameNumber + ")");
        if (GameManager.Instance.GameNumber(70))
        {
            accesabilityObject.Clear();
            // int LevelNumber = AccessibilityManager.instance.LevelCounter;
            AudioClip audioClip = null;
            string newText = "";

            for (int i = 0; i < list.Count; i++)
            {
				audioClip = SoundManager.instance.sounds[i];
                newText = CloseCaption.CCManager.instance.seqStrings[i];
                tempAObj = new AccessibiltyObject() { gameobject = list[i], clip = audioClip, text = newText };
                accesabilityObject.Add(tempAObj);
            }
        }
        else
        {
            if (GameManager.Instance.GameNumber(92))
                AudioFile.instance.PopulateInfoClips();

            AccessibilityManager.instance.populateAccessibiltyList(accesabilityObject, list, uniqueText);
        }


    }

    void GreenBoxNaviagtionAndAudio(RectTransform green, GameObject gameObject)
    {
        if (gameObject.GetComponent<Button>()) // check if the object has button then in order to perform its event we will add a boolen
        {
            // print("button found");
            isButton = true;
            buttonObject = gameObject;
        }
        else
        {
            isButton = false;
            buttonObject = null;
        }

        RectTransform list = gameObject.GetComponent<RectTransform>();
        green.SetParent(list.parent);
        green.anchorMin = list.anchorMin;
        green.anchorMax = list.anchorMax;
        green.anchoredPosition = list.anchoredPosition;
        // green.anchoredPosition = new Vector2(0,0);
        green.sizeDelta = list.sizeDelta;
        green.eulerAngles = list.eulerAngles;
        green.localScale = list.localScale;
        green.pivot = list.pivot;
        enableTouch = true;

    }

    void Update()
    {
        // Debug.Log(gameObject.name + ": " + accesabilityObject.Count);
    }

    public void revertOption()
    {
        if (GameManager.Instance.GameNumber(81))
        {
            FreezeControlsHandler.Instance.UnFreezeControlls();
            list[currentListIndex].SetActive(true);
        }
        else
        {
            list.Add(tempObject);
            tempObject.SetActive(true);
        }

        currentListIndex = list.Count - 1;
        GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
    }
    public void changeState(bool state, bool isUrgent = false)
    {
        Debug.Log("TargetScript(" + gameObject.name + ")->changeState(" + state + ", " + isUrgent + ")");
        AccessibilityManager.instance.placee = uniqueText;

        greenTransform = greenbox.GetComponent<RectTransform>();
        EnableAccessability = false;
        greenbox.SetActive(false);
		 currentListIndex = list.Count - 1;
       if (list.Count > 0 && state)
            GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);

        if (state)
        {
            
            if (isUrgent)
                Invoke("wait", 0);
            else
            {
                print("Invoking wait() in " + timeToWaitToPlayTextATStart + " seconds");
                Invoke("wait", timeToWaitToPlayTextATStart);

            }
        }
    }

    void playsound(GameObject gameObject)
    {
        tempAObj = GetAccessibiltyObject(gameObject);
        if (tempAObj != null)// will call to play sound by find object in accessability 
        {
            Debug.Log("Playing Sound");

            AccessibilityManager.instance.PlayAudioAndShowCaption
             (
                 tempAObj.text,
                 tempAObj.clip
             );
        }
    }

    public AccessibiltyObject GetAccessibiltyObject(GameObject gameObject)
    {
        return accesabilityObject.Find(y => y.gameobject == gameObject);
    }
    public void moveForward()
    {
        Debug.Log("TargetScript->moveForward(" + gameObject.name + ")");

        if (EnableAccessability)
        {
            if (currentListIndex > 0)
            {
                currentListIndex--;
                if (!list[currentListIndex].activeSelf)
                    moveForward();
                else
                    navigate();
            }
            else
            {
                currentListIndex = list.Count - 1;
                if (!list[currentListIndex].activeSelf)
                    moveForward();
                else
                    navigate();

                //arslan
                if (GameManager.Instance.GameNumber(69))
                {
                    if (list[currentListIndex].GetComponent<BoxCollider>().enabled == false)
                        moveForward();
                    else
                        navigate();
                }
            }
        }

    }
    public void moveBackward()
    {
        Debug.Log("TargetScript->moveBackward(" + gameObject.name + ")");

        if (!EnableAccessability)
            return;

        if (currentListIndex < list.Count - 1)
        {
            currentListIndex++;
            if (!list[currentListIndex].activeSelf)
                moveBackward();
            else
                navigate();

        }
        else
        {
            currentListIndex = 0;
            if (!list[currentListIndex].activeSelf)
                moveBackward();
            else
                navigate();
			
            if (GameManager.Instance.GameNumber(69))
            {
                if (list[currentListIndex].GetComponent<BoxCollider>().enabled == false)
                    moveBackward();
                else
                    navigate();
            }
        }
    }
    public void navigate(bool calledFromSelect = false)
    {
        Debug.Log("TargetScript->navigate(" + gameObject.name + ")");

        if (!calledFromSelect)
            playsound(list[currentListIndex]);

        GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
    }
    public void select()
    {
        print("TargetScript->select(" + gameObject.name + ")");

        if (!EnableAccessability)
            return;

        TextToSpeech.ins.StopAudio();

        if (AccessibilityManager.instance.DragnDrop)
        {
            if (removeTarget)
            {
                if (list != null && list[currentListIndex].activeSelf)
                {
                    //arslan
                    if (GameManager.Instance.GameNumber(81))
                    {
                        tempObject = list[currentListIndex];
                        list[currentListIndex].SetActive(false);
                    }
                    else
                    {
                        tempObject = list[currentListIndex];
                        tempAObj = GetAccessibiltyObject(tempObject);
                        //Debug.Log(tempClip.name);
                        list[currentListIndex].SetActive(false);
                        list.RemoveAt(currentListIndex);
                    }


                    if (AccessibilityManager.instance.isSingleDestination)// gets called when game only has 1 destination and we will not need destination.
                        AccessibilityManager.instance.AfterDestinationSelected(tempObject);
                    else
                        AccessibilityManager.instance.SwitchToNextState(tempObject, tempAObj);

                    if (list.Count > 0)
                    {
                        currentListIndex = 0;
                        navigate(true);
                    }
                }
                else
                    Debug.Log("list is empty" + list.Count);
            }
            else
            {
                print("simple");
                
                    if (!isButton)
                        return;

                    buttonObject.GetComponent<Button>().onClick.Invoke();
                
                isButton = false;
                navigate(true);

                tempObject = list[currentListIndex];
                tempAObj = GetAccessibiltyObject(tempObject);
                if (AccessibilityManager.instance.isSingleDestination)// gets called when game only has 1 destination and we will not need destination.
                    AccessibilityManager.instance.AfterDestinationSelected(tempObject);
                else
                    AccessibilityManager.instance.SwitchToNextState(tempObject, tempAObj);
            }
        }
        else
        {
            if (!isButton)
                return;

            Debug.Log("touch of " + gameObject.name);
            isButton = false;
            buttonObject.GetComponent<Button>().onClick.Invoke();
            navigate(true);
            if (buttonObject.GetComponent<TextToSpeak>())
            {
                if (buttonObject.GetComponent<TextToSpeak>().changeState)
                {
                    AccessibilityManager.instance.SwitchToNextState(null);
                }
            }
        }
    }
    public void unselect()
    {
        if (EnableAccessability)
        {
            AccessibilityManager.instance.revertBackToTarget();
        }

    }
    public void infoText()
    {
        Debug.Log("TargetScript->infoText(gameObject: " + gameObject.name + ", " + (EnableAccessability) + ")");

        if (EnableAccessability /*&& !isInfoTextPlaying*/)
        {
            if (UseRemainingSound)
                StartCoroutine(remainingInfoTextE());
            else
                StartCoroutine(infoTextE());
        }
    }

    IEnumerator remainingInfoTextE()
    {
        FreezeControlsHandler.Instance.FreezeControlls();
        TextToSpeech.ins.PlayRemainingInfoText(accesabilityObject[currentListIndex]);
        CloseCaption.CCManager.instance.CreateCaption
        (
            TextToSpeech.ins.GetPlayRemainingInfoText(),
            TextToSpeech.ins.GetPlayRemainingInfoTextLength()
        );
		yield return null;
    }

    IEnumerator infoTextE()
    {
        FreezeControlsHandler.Instance.FreezeControlls();
        isInfoTextPlaying = true;
		AccessibilityManager.instance.PlayAudioAndShowCaption
        (
            AudioFile.instance.inGameString[soundIndex],
            AccessibilityManager.instance.infolips[soundIndex],
            infoOffset
        );
        yield return new WaitForSeconds(AccessibilityManager.instance.infolips[soundIndex].length - infoOffset);

        isInfoTextPlaying = false;
        FreezeControlsHandler.Instance.UnFreezeControlls();

        tempAObj = GetAccessibiltyObject(list[currentListIndex]);
		Debug.Log (" tempAObj.text, " + tempAObj.text);
        AccessibilityManager.instance.PlayAudioAndShowCaption
        (
            tempAObj.text,
            tempAObj.clip
        );

    }
    public string getWaitText()
    {
        return textToSayOnWait;
    }
    public void toggleNavigation(bool state)
    {
        EnableAccessability = state;
    }
    public void ToogleGreenBoxState(bool state)
    {
        this.greenbox.SetActive(state);
    }
    void wait()
    {
        // yield return new WaitForSeconds(timeToWaitToPlayTextATStart);

        Debug.Log("TargetScript->wait(" + gameObject.name + ")");

        if (GameManager.Instance.GameNumber(66))
            PopulateAccessibiltyList();

        float delayToEnableAccessibility = 0;

        if (list.Count > 0)
        {
            if (soundIndex != emptySoundIndex && AccessibilityManager.instance.LastCheck)
            {
                if (AccessibilityManager.instance.clips.Count > 0 && AccessibilityManager.instance.clips[soundIndex] != null)
                {
                    if (AccessibilityManager.instance.gameWasPaused)
                    {
                        AccessibilityManager.instance.gameWasPaused = false;
                        StopAllCoroutines();
                        FreezeControlsHandler.Instance.FreezeControlls();
                        //TextToSpeech.ins.playAudio(GameManager.Instance.Sounds[0]);
                        delayToEnableAccessibility = PlayInfoAndReturnLength();
                    }
                    else
                    {
                        delayToEnableAccessibility = PlayInfoAndReturnLength();
                    }
                }
            }
			Invoke("waitE", delayToEnableAccessibility);
        }
    }
    void waitE()
    {
        Debug.Log("TargetScript->waitE(" + gameObject.name + ")");
        FreezeControlsHandler.Instance.UnFreezeControlls();
        greenbox.SetActive(true);
        GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
        EnableAccessability = true;
      	moveForward();

    }

    void PlaySoundAfterTime()
    {
        Debug.Log("TargetScript->PlaySoundAfterTime()");
        float delayToEnableAccessibility = 0;

        if (soundIndex != emptySoundIndex && AccessibilityManager.instance.LastCheck && AudioFile.instance.others[0] != null)
        {
            int tempOffset = 0;
            delayToEnableAccessibility = AudioFile.instance.others[0].length;

            AccessibilityManager.instance.PlayAudioAndShowCaption
            (
                AudioFile.instance.othersString[0],
                AudioFile.instance.others[0],
                tempOffset
            );
        }

        Invoke("waitE", delayToEnableAccessibility);
    }
    public int returnListLength()
    {
        return list.Count;
    }
    public float PlayInfoAndReturnLength()
    {
        float delayToEnableAccessibility = 0;

        if (UseRemainingSound)
        {
            delayToEnableAccessibility = TextToSpeech.ins.GetPlayRemainingInfoTextLength();
//			if (game92) {
//				if (currentListIndex != 0) {
//					currentListIndex = 0;
//					Debug.LogWarning ("currentListIndex "+currentListIndex);
//				}
//			}
			Debug.LogError ("currentListIndex" +  currentListIndex);
			if (currentListIndex >= accesabilityObject.Count) {
				currentListIndex = 0;
				Debug.LogError ("forcing currentListIndex" + currentListIndex);

			} else {
			}

			Debug.LogError ("before this.gameObject.activeInHierarchy ====== "+this.gameObject.activeInHierarchy);
			Debug.LogError ("count ====== "+accesabilityObject.Count);
			Debug.LogError ("this.gameObject.activeInHierarchy ====== "+this.gameObject.activeInHierarchy);
			if (this.gameObject.name == "L10 Hand") {
				this.gameObject.SetActive (true);
				Debug.LogError ("AAAAAAA");
			} else {

				Debug.LogError ("BBBBBBB");
			}
			Debug.LogError ("gameobject name      "+this.gameObject.name);
			Debug.LogError ("count ====== "+accesabilityObject.Count);
			Debug.LogError ("this.gameObject.activeInHierarchy ====== "+this.gameObject.activeInHierarchy);
			Debug.LogError ("accesabilityObject[currentListIndex]" + accesabilityObject[currentListIndex].text);

			if (TextToSpeech.ins != null) {
				Debug.Log (" TextToSpeech.ins not null");

			}
			else
				Debug.Log (" TextToSpeech.ins is null");
			if (this.gameObject.name == "L10 Hand") {
				Debug.LogError ("-----------------------");
				StartCoroutine (Game92Sound ());
			} else {
			
				Debug.LogError ("+++++++++++++++");
				TextToSpeech.ins.PlayRemainingInfoText (accesabilityObject [currentListIndex]);
			}
			string temSt = TextToSpeech.ins.GetPlayRemainingInfoText ();
			if (temSt == "You have spent $0. Use bills only.") {
				Debug.LogError ("okaa");
				StartCoroutine (DelayCloseCaption(temSt,delayToEnableAccessibility));
			} else {
				CloseCaption.CCManager.instance.CreateCaption (temSt, delayToEnableAccessibility);
			}
        }
      
        else
        {
            delayToEnableAccessibility = AccessibilityManager.instance.clips[soundIndex].length - infoOffset;

            AccessibilityManager.instance.PlayAudioAndShowCaption
            (
                AudioFile.instance.inGameString[soundIndex],
                AccessibilityManager.instance.clips[soundIndex],
                infoOffset
            );
        }

        return delayToEnableAccessibility;
    }
    ///<summary>
    /// this code is specifically for game 93 home isle bottom row - this code is here to play info audio of game.
    ///</summary>
	IEnumerator Game92Sound()
	{
		Debug.LogError ("Game92Sound");
		yield return new WaitForSeconds (2.5f);

		Debug.LogError ("after wait Game92Sound");
		TextToSpeech.ins.PlayRemainingInfoText(accesabilityObject[currentListIndex]);
	}
	IEnumerator DelayCloseCaption(string temSt, float delayToEnableAccessibilityy)
	{
		yield return new WaitForSeconds (2f);
		CloseCaption.CCManager.instance.CreateCaption (temSt, delayToEnableAccessibilityy);
	}
//   
}