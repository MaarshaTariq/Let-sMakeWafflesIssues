using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[System.Serializable]
public class AccessibiltyObject // 
{

    public GameObject gameobject;
    public AudioClip clip;
    public string text;
}
public class listhandler : MonoBehaviour, AcessibilityInterface
{
    public int emptySoundIndex;
    public string uniqueText = "ending";
    public string textToSayOnWait;
    public int soundIndex;
    public int timeToWaitToPlayTextATStart;
    public bool EnableAccessability = false;
    public bool downloadSounds;
    public GameObject[] extraResource;
    public List<GameObject> list;
    public List<AccessibiltyObject> accesabilityObject;
    public GameObject greenbox;
    RectTransform greenTransform;
    int currentListIndex = 0;
    bool enableTouch = false;
    bool isButton = false;
    GameObject buttonObject;
    private GameManager gameManager;

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        currentListIndex = 0;
        isButton = false;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void OnEnable()
    {
        currentListIndex = 0;

        Debug.Log("listhandler(" + gameObject.name + ")->OnEnable()");
        if (downloadSounds)
            StartCoroutine(Delay());

        greenTransform = greenbox.GetComponent<RectTransform>();
		//if(!AccessibilityManager.instance.autoExitGameplay)
		if(uniqueText.Equals("first") || uniqueText.Equals("pause") || uniqueText.Equals("ending"))
        	navigate();
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
        
        Debug.Log("listhandler(" + gameObject.name + ")->Delay()");
	
        if (GameManager.Instance.Accessibilty)
        {
			Debug.Log("Delay()-> GameManager.Instance.Accessibilty");
            AccessibilityManager.instance.populateAccessibiltyList(accesabilityObject, list, uniqueText);
        }
    }


    void GreenBoxNaviagtionAndAudio(RectTransform green, GameObject gameObject)
    {
        if (gameObject.GetComponent<Button>() != null) // check if the object has button then in order to perform its event we will add a boolen
        {
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
    public void changeState(bool state, bool isUrgent = false)
    {
        Debug.Log("listHandler(" + gameObject.name + ")->changeState(" + state + ", " + isUrgent + ")");

        // Debug.Log("changing state of  " + gameObject.name + "    " + state);
        EnableAccessability = false;
        greenbox.SetActive(false);

        if (state)
        {
            if (isUrgent)
                Invoke("wait", 0);
            else
            {
                Debug.Log("Invoking wait in " + timeToWaitToPlayTextATStart + " seconds");
                Invoke("wait", timeToWaitToPlayTextATStart);
            }
        }
    }
    public void revertOption()
    {

    }
    void playsound(GameObject gameobject)
    {
        AccessibiltyObject tempAccessibiltyObject = accesabilityObject.Find(y => y.gameobject == gameobject);
        if (tempAccessibiltyObject != null)// will call to play sound by find object in accessability 
        {
            Debug.Log("Playing Sound");

            AccessibilityManager.instance.PlayAudioAndShowCaption
            (
                tempAccessibiltyObject.text,
                tempAccessibiltyObject.clip
            );
        }
    }
    public void moveForward()
    {
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
            }
        }
    }
    public void moveBackward()
    {
        if (EnableAccessability)
        {
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
            }
        }
    }
    public int returnListLength()
    {
        return list.Count;
    }
    public void select()
    {
        if (EnableAccessability)
        {
            if (isButton)
            {
                Debug.Log("touch of " + gameObject.name);
                isButton = false;
                buttonObject.GetComponent<Button>().onClick.Invoke();
                if (buttonObject.GetComponent<AdditionalSoundHandler>() == null)
                    AccessibilityManager.instance.SwitchToNextState(null);
                else if (!buttonObject.GetComponent<AdditionalSoundHandler>().buttonLabel.Equals("Wrong"))
                    AccessibilityManager.instance.SwitchToNextState(null);
                else
                    print("Button is just for playing incorrect and info text sound");
            }
        }
    }
    public void navigate()
    {
        // Debug.Log("listHandler->navigate() called");

        GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
        playsound(list[currentListIndex]);
    }
    public void infoText()
    {
        Debug.Log("listHandler(" + gameObject.name + ")->infoText() called");

        if (soundIndex != emptySoundIndex)
        {
            TextToSpeech.ins.playAudio(AccessibilityManager.instance.infolips[soundIndex]);
            AccessibilityManager.instance.PlayAudioAndShowCaption
            (
                AudioFile.instance.inGameString[soundIndex],
                AccessibilityManager.instance.infolips[soundIndex]
            );
        }
    }

    public void unselect()
    {

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
        Debug.Log("listHandler(" + gameObject.name + ")->wait()");

        float delayToEnableAccessibility = 0;

        if (soundIndex != emptySoundIndex && AccessibilityManager.instance.LastCheck)
        {
            if (AccessibilityManager.instance.clips.Count > 0 && AccessibilityManager.instance.clips[soundIndex] != null)
            {
                if (AccessibilityManager.instance.gameWasPaused)
                {
                    AccessibilityManager.instance.gameWasPaused = false;
                    StopAllCoroutines();
                    FreezeControlsHandler.Instance.FreezeControlls();
                    delayToEnableAccessibility = AccessibilityManager.instance.clips[soundIndex].length;
                    AccessibilityManager.instance.PlayAudioAndShowCaption
                    (
                        AudioFile.instance.inGameString[soundIndex],
                        AccessibilityManager.instance.clips[soundIndex]
                    );
                }
                else
                {
                    // TextToSpeech.ins.StopPlayRemainingInfoTextE();
                    FreezeControlsHandler.Instance.FreezeControlls();
                    delayToEnableAccessibility = AccessibilityManager.instance.clips[soundIndex].length;
                    AccessibilityManager.instance.PlayAudioAndShowCaption
                    (
                        AudioFile.instance.inGameString[soundIndex],
                        AccessibilityManager.instance.clips[soundIndex]
                    );
                }
            }
        }

        Invoke("waitE", delayToEnableAccessibility);
    }

    void waitE()
    {
        if ((soundIndex != emptySoundIndex && !uniqueText.Contains(".")) || uniqueText.Equals("ending"))
            FreezeControlsHandler.Instance.UnFreezeControlls();

        EnableAccessability = true;
        greenbox.SetActive(true);

        if (!AccessibilityManager.instance.autoExitGameplay)
            GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
    }

    void PlaySoundAfterTime()
    {
        float delayToEnableAccessibility = AccessibilityManager.instance.clips[soundIndex].length;
        AccessibilityManager.instance.PlayAudioAndShowCaption
        (
            AudioFile.instance.inGameString[soundIndex],
            AccessibilityManager.instance.clips[soundIndex]
        );

        Invoke("waitE", delayToEnableAccessibility);
    }
}