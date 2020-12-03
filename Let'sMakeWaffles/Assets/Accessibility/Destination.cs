using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Destination : MonoBehaviour, AcessibilityInterface
{
    public int emptySoundIndex;
    public string uniqueText = "DestinationOne";
    public int timeToWaitToPlayTextATStart = 3;
    public int soundIndex = 3;
    public string textToSayOnWait;
    public static Destination ins;
    public bool EnableAccessability = false;
    public bool removeDestination = false; //arslan - used in case if you want to remove destination object from list.
    public bool downloadSounds;
    [HideInInspector] public bool isSoundsDownloaded;
    public List<GameObject> list;
    public List<AccessibiltyObject> accesabilityObject;
    public GameObject greenbox;
    RectTransform greenTransform;
    int currentListIndex = 0;
    bool enableTouch = false;
    bool isButton = false;
    GameObject buttonObject;
    GameObject tempObject;
    public GameObject targetObject;

    [SerializeField] private float minimumSwipeDistanceY;
    [SerializeField] private float minimumSwipeDistanceX;

    private Touch t = default(Touch);
    private Vector3 startPosition = Vector3.zero;
    AccessibiltyObject tempAObj;


    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        accesabilityObject.Clear();
        currentListIndex = 0;
        isButton = false;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void OnEnable()
    {
        if (AccessibilityManager.instance == null)
            return;

        if (downloadSounds && !isSoundsDownloaded)
            AccessibilityManager.instance.populateAccessibiltyList(accesabilityObject, list, uniqueText);

        ins = this;
        currentListIndex = 0;
        greenTransform = greenbox.GetComponent<RectTransform>();
        GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
        // listTranform = list[currentListIndex];

        // StartCoroutine(CopyTransform(greenTransform, list[currentListIndex]));
    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void GreenBoxNaviagtionAndAudio(RectTransform green, GameObject gameObject)
    {
        if (gameObject.GetComponent<Button>()) // check if the object has button then in order to perform its event we will add a boolen
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
    public void getObject(GameObject obj)
    {
        //		Debug.Log("setting object");
        targetObject = obj;
    }
    public void ToogleGreenBoxState(bool state)
    {
        this.greenbox.SetActive(state);
    }
    public void changeState(bool state, bool isUrgent = false)
    {
        currentListIndex = 0;
        Debug.Log("changing state of  " + gameObject.name + "    " + state);

        if (!state)
            return;

        if (GameManager.Instance.GameNumber(51))
            AccessibilityManager.instance.populateAccessibiltyList(accesabilityObject, list, uniqueText);
        
        if (!GameManager.Instance.GameNumber(77))
         GreenBoxNaviagtionAndAudio(greenTransform, list[0]);
        Invoke("wait", timeToWaitToPlayTextATStart);
        greenbox.SetActive(false);
        EnableAccessability = false;
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
    AccessibiltyObject GetAccessibiltyObject(GameObject gameObject)
    {
        return accesabilityObject.Find(y => y.gameobject == gameObject);
    }
    public void moveForward()
    {
        if (EnableAccessability)
        {
            if (currentListIndex > 0)
            {
                currentListIndex--;
                playsound(list[currentListIndex]);
                GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
            }
            else
            {
                currentListIndex = list.Count - 1;
                playsound(list[currentListIndex]);
                GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
            }
        }

    }
     public int returnListLength()
    {
        return list.Count;
    }
    public void moveBackward()
    {
        if (EnableAccessability)
        {
            if (currentListIndex < list.Count - 1)
            {

                //StartCoroutine(CopyTransform(greenTransform, list[currentListIndex + 1]));
                //                Debug.Log("down " + currentListIndex);
                currentListIndex++;
                playsound(list[currentListIndex]);
                GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
            }
            else

            {
                currentListIndex = 0;
                playsound(list[currentListIndex]);
                GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);

            }
        }
    }
    public void select()
    {
        if (EnableAccessability)
        {
            Debug.Log("Destination->select()");
            if (AccessibilityManager.instance.DragnDrop)
            {
                // greenbox.transform.parent = targetObject.transform;
                if (removeDestination)
                {
                    if (list != null)
                    {
                        tempObject = list[currentListIndex];
                        tempAObj = GetAccessibiltyObject(tempObject);
                        list[currentListIndex].SetActive(false);
                        list.RemoveAt(currentListIndex);

                        AccessibilityManager.instance.AfterDestinationSelected(tempObject);
                    }

                }
                else
                {
                    if (!GameManager.Instance.GameNumber(51))
                        AccessibilityManager.instance.AfterDestinationSelected(list[currentListIndex]);
                    else if (GameManager.Instance.GameNumber(51))
                        ButtonSelection();

                }
            }
            else
            {
                print("simple");
                ButtonSelection();
            }
        }
    }

    public void ButtonSelection()
    {
        if (!isButton)
            return;

        Debug.Log("touch of " + gameObject.name);
        //isButton = false;
        buttonObject.GetComponent<Button>().onClick.Invoke();

        // For games 42 - 45
        if (GameManager.Instance.GameNumber(42, 43, 44, 45))
        {
            AccessibilityManager.instance.changeTarget();
            AccessibilityManager.instance.changeDestination();
        }

        if (buttonObject.GetComponent<TextToSpeak>())
        {
            if (buttonObject.GetComponent<TextToSpeak>().changeState)
            {
                AccessibilityManager.instance.SwitchToNextState(null);
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
        StartCoroutine(infoTextE());
    }

    IEnumerator infoTextE()
    {
        print("Destination->infoTextE()");
        FreezeControlsHandler.Instance.FreezeControlls();


        if (GameManager.Instance.GameNumber(51, 77))
        {
            CloseCaption.CCManager.instance.CreateCaption
            (
                AudioFile.instance.inGameString[soundIndex] + " " + AccessibilityManager.instance.STAO.text + ".",
                AccessibilityManager.instance.infolips[soundIndex].length + AccessibilityManager.instance.STAO.clip.length
            );
            
            TextToSpeech.ins.playAudio(AccessibilityManager.instance.infolips[soundIndex]);
            yield return new WaitForSeconds(AccessibilityManager.instance.infolips[soundIndex].length);

            TextToSpeech.ins.playAudio(AccessibilityManager.instance.STAO.clip);
            yield return new WaitForSeconds(AccessibilityManager.instance.STAO.clip.length);
        }
        else
        {
            AccessibilityManager.instance.PlayAudioAndShowCaption
            (
                AudioFile.instance.inGameString[soundIndex],
                AccessibilityManager.instance.infolips[soundIndex]
            );

            yield return new WaitForSeconds(AccessibilityManager.instance.infolips[soundIndex].length);
        }



        // if (GameManager.Instance.GameNumber(51,77))
        // {
        //     AccessibilityManager.instance.PlayAudioAndShowCaption
        //     (
        //         AccessibilityManager.instance.STAO.text,
        //         AccessibilityManager.instance.STAO.clip
        //     );

        //     yield return new WaitForSeconds(AccessibilityManager.instance.STAO.clip.length + 1);
        // }

        FreezeControlsHandler.Instance.UnFreezeControlls();
        AccessibilityManager.instance.PlayAudioAndShowCaption
        (
            accesabilityObject[currentListIndex].text,
            accesabilityObject[currentListIndex].clip
        );


    }
    public string getWaitText()
    {
        return textToSayOnWait;
    }
    public void revertOption()
    {
        list.Add(tempObject);
        if (!GameManager.Instance.GameNumber(77))
            tempObject.SetActive(true);
    }
    public void toggleNavigation(bool state)
    {
        EnableAccessability = state;
    }
    void wait()
    {
        // yield return new WaitForSeconds(timeToWaitToPlayTextATStart);

        Debug.Log("Destination->wait(" + gameObject.name + ")");

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
                        print("Game was paused");
                        if (GameManager.Instance.GameNumber(51, 77))
                        {
                            delayToEnableAccessibility = AccessibilityManager.instance.clips[soundIndex].length;
                            CloseCaption.CCManager.instance.CreateCaption
                            (
                                AudioFile.instance.inGameString[soundIndex] + " " + AccessibilityManager.instance.STAO.text + ".",
                                AccessibilityManager.instance.clips[soundIndex].length + AccessibilityManager.instance.STAO.clip.length
                            );
                            TextToSpeech.ins.playAudio(AccessibilityManager.instance.clips[soundIndex]);
                        }
                        else
                        {
                            delayToEnableAccessibility = AccessibilityManager.instance.clips[soundIndex].length;

                            AccessibilityManager.instance.PlayAudioAndShowCaption
                            (
                                AudioFile.instance.inGameString[soundIndex],
                                AccessibilityManager.instance.clips[soundIndex]
                            );
                        }
                    }
                    else
                    {
                        // TextToSpeech.ins.StopPlayRemainingInfoTextE();
                        FreezeControlsHandler.Instance.FreezeControlls();

                        if (GameManager.Instance.GameNumber(51, 77))
                        {
                            delayToEnableAccessibility = AccessibilityManager.instance.clips[soundIndex].length;
                            CloseCaption.CCManager.instance.CreateCaption
                            (
                                AudioFile.instance.inGameString[soundIndex] + " " + AccessibilityManager.instance.STAO.text + ".",
                                AccessibilityManager.instance.clips[soundIndex].length + AccessibilityManager.instance.STAO.clip.length
                            );
                            TextToSpeech.ins.playAudio(AccessibilityManager.instance.clips[soundIndex]);
                        }
                        else
                        {
                            delayToEnableAccessibility = AccessibilityManager.instance.clips[soundIndex].length;

                            AccessibilityManager.instance.PlayAudioAndShowCaption
                            (
                                AudioFile.instance.inGameString[soundIndex],
                                AccessibilityManager.instance.clips[soundIndex]
                            );
                        }
                    }
                }
            }

            if (GameManager.Instance.GameNumber(51, 77))
                Invoke("PlaySoundAfterTime", delayToEnableAccessibility);
            else
                Invoke("waitE", delayToEnableAccessibility);
        }
    }

    void waitE()
    {
        Debug.Log("@Destination->waitE(" + gameObject.name + ")");

        FreezeControlsHandler.Instance.UnFreezeControlls();
        EnableAccessability = true;
        // if (!GameManager.Instance.GameNumber(77))//faiq
        //     GreenBoxNaviagtionAndAudio(greenTransform, list[currentListIndex]);
        greenbox.SetActive(true);
        //arslan
        if (GameManager.Instance.GameNumber(51, 77))
            moveForward();
    }

    void PlaySoundAfterTime()
    {
        float delayToEnableAccessibility = 0;

        if (soundIndex != emptySoundIndex && AccessibilityManager.instance.LastCheck)
        {
            delayToEnableAccessibility = AccessibilityManager.instance.STAO.clip.length;
            TextToSpeech.ins.playAudio(AccessibilityManager.instance.STAO.clip);
        }

        Invoke("waitE", delayToEnableAccessibility);
    }
}