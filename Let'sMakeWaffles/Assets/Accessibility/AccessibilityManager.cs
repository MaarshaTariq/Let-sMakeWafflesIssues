using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]

public class AccessibilityManager : MonoBehaviour
{
    #region Variables

    #region private
    bool checkActivity = false;

    public int targetCounter = 0, destinationCounter = 0;

    private bool flag = true;
    public Direction mydirection, tempDirection; // contain current active state of game

    [HideInInspector] public GameObject selectedTargetObject; // holds info of the obj which was selected when state machine goes from target to destination state ..in order to give the info to game.
    [HideInInspector] public AccessibiltyObject STAO; //Selected Target Accessibility Object
    #endregion
    #region public
    [HideInInspector] public bool backOfficeInitDone = false;
    public int gameNumber;
    public int LevelCounter = 0;
    public int completedLevels = 0;
    public bool[] totalLevels;
    public bool gameWasPaused;
    [HideInInspector] public bool LastCheck;
    public float switchWait = 10f;
    public bool isSpaceEnabled;
    public bool enablepause = true;
    public bool isSingleDestination = false;
    public bool DragnDrop = false;
    public bool ShowPausePanel = true;
    public bool autoExitGameplay;

    public GameObject GreenBox;
    public GameObject block;
    public GameObject LoadingScreen;
    public GameObject pausePanel;
    public GameObject TextToSpeechObject;

    [HideInInspector]
    public float timeOut = 30.0f, timeOutTimer = 0.0f; // countr to check for screen time out func
    public delegate void MultiDelegate(GameObject obj, GameObject des);
    public MultiDelegate DestinationSelected; // use to tell notify everyone on time of destination selection

    public List<AudioClip> infolips = new List<AudioClip>();

    public List<AudioClip> clips = new List<AudioClip>();
    public delegate void levelComplete(int nextlevel);
    public levelComplete levelCompleted; // use to tell notify everyone on time of level completion
    public static AccessibilityManager instance;
    public GameObject[] listHandleList; // handle list of all state machine in game
    public GameObject[] levelsList; // handle list of all destination which will change over time in game
    public GameObject[] targetList; // handle list of all target which will change over time in game
    public GameObject[] destiantionList; // handle list of all destination which will change over time in game
    public enum Direction { MainMenu, GamePlay, Target, Destination, Ending, pause }; // states which will be enable through out the game
    public Dictionary<Direction, AcessibilityInterface> root = new Dictionary<Direction, AcessibilityInterface>(); // this dicionary is reponsible for storing all state and able to iterate it through game

    #endregion

    #endregion

    #region  functions

    //populating the root
    void Start()
    {
        if (LoadingScreen)
            LoadingScreen.SetActive(true);

        LastCheck = true;
        //CanCheckInfo = true;
        ShowPausePanel = true;
        //ShowPausePanel = true;

        // Blocking all inputs from mouse
        if (block != null)
            block.SetActive(true);

        if (GreenBox != null)
            GreenBox.SetActive(true);

		if (TextToSpeechObject != null)
			TextToSpeechObject.SetActive (true);
		else {
			if (GameManager.Instance.GameNumber (92)) {
				
				TextToSpeechObject = FindObjectOfType<External> ().TTSRef.gameObject;
				TextToSpeechObject.SetActive (true);
			}
			else
				TextToSpeechObject = FindObjectOfType<TextToSpeech> ().gameObject;
		}



        PlayerPrefs.SetString("clickable", "false");
        PlayerPrefs.SetInt("Click", 1);

        instance = this;
        if (listHandleList != null)
        {
            // TextToSpeech.ins.DowloadStartingSound(inGameSounds, clips);
            // TextToSpeech.ins.DowloadStartingSound(infoTexts, infolips);
            // for (int i = 0; i < 4; i++)
            // {
            //     clips.Add(AudioFile.instance.inGame[i]);
            //     infolips.Add(AudioFile.instance.infoText[i]);
            // }

            populateRootList();
            StartCoroutine(StartingAccessibilty());
        }
    }
    public int checkListLength()
    {
        return root[mydirection].returnListLength();
    }
    IEnumerator StartingAccessibilty()
    {
        yield return new WaitUntil(() => TextToSpeech.ins.AudioDownloaded == TextToSpeech.ins.TotalAudioToDownload);
        if (LoadingScreen)
            LoadingScreen.SetActive(false);
        startAccessibilty();
    }
    public void startAccessibilty()
    {

        mydirection = Direction.MainMenu;
        root[mydirection].changeState(true);

        if (GameManager.Instance.GameNumber(92) && !GameManager.Instance.SceneNumber(1))
            SwitchToNextState();
    }
    ///<summary>
    /// this function is used to populate root from states which are gonna role in games
    ///</summary>
    public void populateRootList()
    {
        // listHandleList[2] = targetList[0];
        // listHandleList[3] = destiantionList[0];
        int i = 0;
        string[] allDirection = System.Enum.GetNames(typeof(Direction));
        if (allDirection.Length != listHandleList.Length)
        {
            Debug.LogError(" the number of states and provided objects are not equal");
            return;
        }

        foreach (Direction currDirrection in System.Enum.GetValues(typeof(Direction)))
        {
            root.Add(currDirrection, listHandleList[i].GetComponent<AcessibilityInterface>());
            i++;
        }
    }
    /// <summary>
    /// Game Ending
    /// </summary>
    public void EndGame()
    {
        if (GameManager.Instance.GameNumber(69))
        {
            Image image = GreenBox.GetComponent<Image>();
            var tempColor = image.color;
            tempColor.a = 1f;
            image.color = tempColor;
        };

        // In game number 92 end game is needed only in scene number 4
        if (GameManager.Instance.GameNumber(92) && GameManager.Instance.SceneNumber(1, 2, 3))
            return;

        root[mydirection].changeState(false);
        mydirection = Direction.Ending;
        root[mydirection].changeState(true);
    }
    public Direction pauseGame()
    {
        print("AccessibilityManager->pauseGame() called");

        Direction temp = mydirection;
        print("Mydirection: " + mydirection);
        print("Enablepause: " + enablepause);
        if (mydirection != Direction.MainMenu /*&& mydirection != Direction.GamePlay*/ && mydirection != Direction.Ending && enablepause)
        {
            print("pausing game");
            pausePanel.SetActive(true);
            root[mydirection].changeState(false);
            mydirection = Direction.pause;
            root[mydirection].changeState(true);
            ShowPausePanel = false;

        }
        return temp;
    }
    /// <summary>
    /// arslan - this function is psecifically used to switch back to target from destination
    /// </summary>
    public void unpauseGame()
    {
        print("unpausing game");
        root[mydirection].changeState(false);
        mydirection = tempDirection;
        root[mydirection].changeState(true);
        if (mydirection == Direction.GamePlay)
        {
            root[mydirection].changeState(false);
            mydirection = Direction.Target;
            root[mydirection].changeState(true);
        }
    }
    /// <summary>
    /// this function is used when user want to changes the target e.g when going from one level to second.
    /// </summary>
    int tempIncrement = 0;
    bool tempIsExactLevel;
    public void changeTarget(int increment = 1, bool isExactLevel = false)
    {
        Debug.Log("AccessibilityManager->changeTarget(" + increment + ")");
        if (GameManager.Instance.GameNumber(92))
            FreezeControlsHandler.Instance.FreezeControlls();

        if (!isExactLevel)
            targetCounter += increment; // increasing target couter
        else
            targetCounter = increment; //  direclty changing to new target couter

        tempIsExactLevel = isExactLevel;
        tempIncrement = increment;

        if (targetCounter < targetList.Length)
        {
            root[Direction.Target].changeState(false); // change state to false
            root[Direction.Target] = targetList[targetCounter].GetComponent<AcessibilityInterface>(); // replacing the target list.
            print("Target: " + targetList[targetCounter].name);

            if (levelsList.Length != 0)
                Invoke("changeGameplay", 4f);
            else StartCoroutine(Switch());

        }
        else
            EndGame();
    }

    //arslan - just fro special case if you  want to run this function from inspector
  
    public void changeGameplay()
    {
        while (!levelsList[LevelCounter])
        {
            print("waiting to active the level");
        }

        Debug.Log("Changing gameplay");
        //arsln
      
        if (!tempIsExactLevel)
            LevelCounter += tempIncrement;
        else
            LevelCounter = tempIncrement;

        if (totalLevels.Length > 0 && !totalLevels[LevelCounter])
        {
            totalLevels[LevelCounter] = true;
            completedLevels++;
        }

        root[Direction.GamePlay] = levelsList[LevelCounter].GetComponent<AcessibilityInterface>();
        print("changing level " + levelsList[LevelCounter].name + "    " + targetCounter);

        root[mydirection].changeState(false);
        mydirection = Direction.GamePlay;// reversing the state to GamePlay.
        root[mydirection].changeState(true);
        StartCoroutine(Switch());
    }
    /// <summary>
    /// this function is used when user want to changes the destination e.g when going from one level to second.
    /// </summary>
    public void changeDestination(int increment = 1)
    {
        //arsln
        if (GameManager.Instance.GameNumber(77))
        {
            if (targetCounter == 5)
                return;
        }
        destinationCounter += increment;
        if (destinationCounter < destiantionList.Length)
        {
            root[Direction.Destination] = destiantionList[destinationCounter].GetComponent<AcessibilityInterface>();
        }
    }
    /// <summary>
    /// this function is used to download in game sound for repetative use.
    /// </summary>
    void downloadInGameSounds()
    {

    }
    /// <summary>
    /// when user wants to shift forward from one state to another
    /// </summary>
    public void SwitchToNextState(GameObject obj = null, AccessibiltyObject aObj = null)
    {
        Debug.Log("AccessibilityManager->SwitchToNextState(" + obj + ", " + aObj + ")");
        
        if (obj != null)
            selectedTargetObject = obj;

        if (aObj != null)
            STAO = aObj;

        switch (mydirection)
        {
            case (Direction.MainMenu):
                //changed by arslan
                
                    Debug.Log("main menu");
                    root[mydirection].changeState(false);
                    mydirection = Direction.GamePlay;
                    root[mydirection].changeState(true);
                    if (autoExitGameplay)
                        StartCoroutine(Switch());
                
                break;

            case (Direction.GamePlay):
                Debug.Log("enum of gameplay");
                root[mydirection].changeState(false);
                mydirection = Direction.Target;
                root[mydirection].changeState(true);

                break;

            case (Direction.Target):
                Debug.Log("enum of target");
                print("root[" + mydirection + "].changeState(false)");
                root[mydirection].changeState(false);
                mydirection = Direction.Destination;
                print("root[" + mydirection + "].changeState(true)");
                root[mydirection].changeState(true);
                break;

            case (Direction.pause):
                Debug.Log("enum of pause");
                root[mydirection].changeState(false);
                mydirection = tempDirection;
                root[mydirection].changeState(true, true);
                break;

            default:
                print("invalid state is selected");
                break;
        }


    }
    IEnumerator Switch()
    {
        print("AccessibilityManager->Switch(" + LevelCounter + ")");

        if (placee == "first")
        {
            Debug.Log("***************** firstB");
            yield return new WaitForSeconds(3f);
            Debug.Log("***************** firstA");
        }
		else if (placee == "Level1Target1")
		{
			Debug.Log("***************** Level1");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("***************** Level1B");
		}
		else if (placee == "Level2Target1")
		{
			Debug.Log("***************** Level2");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("***************** Level2B");
		}
		else if (placee == "Level3Target1")
		{
			Debug.Log("***************** Level2");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("***************** Level2B");
		}
		else if (placee == "Level4Target1")
		{
			Debug.Log("***************** Level2");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("***************** Level2B");
		}
		else if (placee == "Level5Target1")
		{
			Debug.Log("***************** Level2");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("***************** Level2B");
		}
		else if (placee == "Level6Target1")
		{
			Debug.Log("***************** Level2");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("***************** Level2B");
		}
		else if (placee == "Level7Target1")
		{
			Debug.Log("***************** Level2");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("***************** Level2B");
		}
		else if (placee == "Level8Target1")
		{
			Debug.Log("***************** Level2");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("***************** Level2B");
		}
		else if (placee == "Level9Target1")
		{
			Debug.Log("***************** Level2");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("***************** Level2B");
		}
		else if (placee == "Level10Target1")
		{
			Debug.Log("***************** Level2");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("***************** Level2B");
		}
        else if (placee == "Final" || placee == "TargetFinal")
        {
            Debug.Log("***************** FinalB");
            yield return new WaitForSeconds(20f);
            Debug.Log("***************** FinalA");
        }


        if (autoExitGameplay)
        {
            mydirection = Direction.Target;
            Debug.Log("Mydirection(true): " + mydirection);
            root[mydirection].changeState(true);
        }
    }
    /// <summary>
    /// when user wants to shift backward from one state to another
    /// </summary>

    public void SwitchToPreviousState()
    {
        Debug.Log("AccessibilityManager->SwitchToPreviousState()");

        switch (mydirection)
        {
            case (Direction.Destination):

                root[mydirection].changeState(false);
                mydirection = Direction.Target;
                root[mydirection].changeState(true);

                break;

            // case (Direction.Target):

            //     root[mydirection].changestate(false);
            //     mydirection = direction.gameplay;
            //     root[mydirection].changestate(true);
            //     break;

            default:
                print("invalid state is selected");
                break;
        }

    }
    public void resetCurrentState()
    {
        // root[mydirection].changeState(false);
        // root[mydirection].changeState(true);
    }

    // this function is used to check if user want to go back and re select the target.
    public void revertBackToTarget()
    {
        print("going to revert");
        SwitchToPreviousState();
        if (GameManager.Instance.GameNumber(81))//arslan
        {
            root[mydirection].changeState(false);
            mydirection = Direction.Target;
            root[mydirection].changeState(true);
        }

        root[mydirection].revertOption();
    }
    //arslan - this function is written if there is case of reverting a destination back in case if you enable "remove destination" in destination. 
    // this function will revert the destination the same way target is able to.s
    public void revertDestination()
    {
        root[mydirection].revertOption();
        SwitchToPreviousState();
    }
    // this function is used when user select the target as a reponse we will send notification to all delegate subscribe to
    // DestinationSelected so they can perform any activity the games wants.
    public void AfterDestinationSelected(GameObject Target)
    {
        if (flag)
        {
            flag = false;
            if (isSingleDestination)
                DestinationSelected(Target, null);
            else
                DestinationSelected(selectedTargetObject, Target);  // selectedTargetObject = target, Target = Destination object

            StartCoroutine(enable());
        }
    }

    IEnumerator enable()
    {
        if (!GameManager.Instance.GameNumber(81))
            yield return new WaitForSeconds(2f);
        flag = true;
    }
    #region userInput Fuction
    //<--------- User controls -------- called from index.html file ----------------->
    public void swipeUp()
    {
        if (FreezeControlsHandler.Instance.isControllsFreezed)
            return;

        Debug.Log("Next");
        root[mydirection].moveBackward();
        EventController.instance.CountScreenInteractionWithoutCheck();
        checkActivity = true;
    }

    public void swipeUp(int dummy)
    {
        if (FreezeControlsHandler.Instance.isControllsFreezed)
            return;

        Debug.Log("Next");
        root[mydirection].moveBackward();
        checkActivity = true;
    }
    public void swipeDown()
    {
        if (FreezeControlsHandler.Instance.isControllsFreezed)
            return;

        Debug.Log("Prev");
        root[mydirection].moveForward();
        EventController.instance.CountScreenInteractionWithoutCheck();
        checkActivity = true;
    }
    public void select()
    {
        if (FreezeControlsHandler.Instance.isControllsFreezed)
            return;

        print("AccessibilityManager->select(Mydirection: " + mydirection + ")");
        EventController.instance.CountScreenInteractionWithoutCheck();
        Debug.Log("root[" + mydirection + "].select();");
        root[mydirection].select();
        checkActivity = true;

    }
    public void unSelect()
    {
        //        root[mydirection].unselect();
        //        EventController.instance.CountScreenInteractionWithoutCheck();
        //        checkActivity = true;
    }
    public void Close()
    {
        if (FreezeControlsHandler.Instance.isControllsFreezed)
            return;

        try
        {
            if (!targetList[LevelCounter].GetComponent<TargetScript>().enablePause)
                return;
        }
        catch (Exception ex)
        {
            Debug.LogError("Targetscript nor found: " + ex);
        }

        print("AccessibilityManager->Close() called");
        EventController.instance.CountScreenInteractionWithoutCheck();
        tempDirection = pauseGame();
    }

    // Variable to let info play while being controls are freezed
    bool isUrgentInfo = false;

    public void Info()
    {
        if (!isUrgentInfo && FreezeControlsHandler.Instance.isControllsFreezed)
            return;

        print("AccessibilityManager->Info(Mydirection: " + mydirection + ", " + (mydirection != Direction.Ending).ToString() + ")");
        
        if(!isUrgentInfo)
            EventController.instance.CountScreenInteractionWithoutCheck();

        if (mydirection != Direction.Ending)
        {
            //FreezeControlsHandler.Instance.UnFreezeControlls();
            root[mydirection].infoText();
            checkActivity = true;
        }
    }

    public void Info(bool isUrgentInfo)
    {
        this.isUrgentInfo = isUrgentInfo;
        Info();
        this.isUrgentInfo = false;
    }
    #endregion
    //<-----------------------------------------------user controls------------------------------------------>
    void Update()
    {
        PlayerPrefs.SetString("clickable", "false");
        PlayerPrefs.SetInt("Click", 1);

        if (FreezeControlsHandler.Instance.isControllsFreezed)
            return;

        ScreenTimeoutNotifier();

        bool down = Input.GetKeyDown(KeyCode.DownArrow);
        bool goBack = Input.GetKeyDown(KeyCode.Backspace);
        bool Up = Input.GetKeyDown(KeyCode.UpArrow);
        bool space = Input.GetKeyDown(KeyCode.Space);
        bool info = Input.GetKeyDown(KeyCode.I);
        bool close = Input.GetKeyDown(KeyCode.C);
        if (down)
        {
            swipeUp();
            checkActivity = true;
            down = false;


        }
        if (Up)
        {
            swipeDown();
            checkActivity = true;
            Up = false;

        }
        if (space && isSpaceEnabled)
        {
            select();
            checkActivity = true;
            space = false;
        }
        if (goBack)
        {
            unSelect();
            checkActivity = true;
            goBack = false;
        }
        if (info && ShowPausePanel)
        {
            Info();
            checkActivity = true;
            info = false;
        }
        if (close && ShowPausePanel && LastCheck)
        {
            Debug.Log("AccessibilityManager->c pressed");
            Close();
            // GameManager.Instance.Stop();
            //  ToggleNaviagtion(false);
            //  pausePanel.SetActive(true);

            close = false;
        }

    }
    ///<summary>
    ///the function is used in order to give functionality of perform some action after some time like play a sound 
    ///</summary>

    public void ScreenTimeoutNotifier()
    {
        //Debug.Log("AccessibilityManager->ScreenTimeoutNotifier() called: " + timeOutTimer);

        timeOutTimer += Time.deltaTime;
        // If screen is tapped, reset timer
        if (checkActivity)
        {
            timeOutTimer = 0.0f;
            checkActivity = false;
            //Dont active screensaver
        }
        // If timer reaches zero, start screensaver
        if (timeOutTimer > timeOut && LastCheck)
        {
            Debug.Log("AccessibilityManager->ScreenTimeoutNotifier() timeout");
            root[mydirection].infoText();
            timeOutTimer = -2.0f;
        }
    }

    IEnumerator main(List<AccessibiltyObject> accesabilityObject, List<GameObject> list)
    {
        Debug.Log("AccessibilityManager->main(" + TextToSpeech.ins.AudioDownloaded + ")");
        yield return new WaitUntil(() => TextToSpeech.ins.AudioDownloaded == TextToSpeech.ins.TotalAudioToDownload);
        print("size is " + AudioFile.instance.MainMenu.Count);
        for (int i = 0; i < list.Count; i++)
        {
            AccessibiltyObject obj = new AccessibiltyObject() { gameobject = list[i], clip = AudioFile.instance.MainMenu[i], text = AudioFile.instance.MainMenuString[i] };
            accesabilityObject.Add(obj);
        }
    }
    // [HideInInspector]
    public string placee = "";
    // arslan - this function is usedto populate all the destination accessibilty lists
    public void populateDestinationList(List<AccessibiltyObject> accesabilityObject, List<GameObject> list, string place, bool isSoundsDownloaded = false)
    {
        switch (place)
        {
            case "Destination2":
                populate(accesabilityObject, list, AudioFile.instance.Destination2, AudioFile.instance.Destination2Strings);
                break;
            case "Destination3":
                populate(accesabilityObject, list, AudioFile.instance.Destination3, AudioFile.instance.Destination3Strings);
                break;
            case "Destination4":
                populate(accesabilityObject, list, AudioFile.instance.Destination4, AudioFile.instance.Destination4Strings);
                break;
            case "Destination5":
                populate(accesabilityObject, list, AudioFile.instance.Destination5, AudioFile.instance.Destination5Strings);
                break;
            case "Destination6":
                populate(accesabilityObject, list, AudioFile.instance.Destination6, AudioFile.instance.Destination6Strings);
                break;
            case "Destination7":
                populate(accesabilityObject, list, AudioFile.instance.Destination7, AudioFile.instance.Destination7Strings);
                break;
            case "Destination8":
                populate(accesabilityObject, list, AudioFile.instance.Destination8, AudioFile.instance.Destination8Strings);
                break;


            default: break;
        }
    }
    //arslan - this function is use to take a accessibilty object and populate data in it and store in main accessibilty list
    void populate(List<AccessibiltyObject> accesabilityObject, List<GameObject> list, List<AudioClip> Clip, String[] Text)
    {
        for (int i = 0; i < list.Count; i++)
        {
            AccessibiltyObject obj = new AccessibiltyObject()
            {
                gameobject = list[i],
                clip = Clip[i],
                text = Text[i]
            };
            accesabilityObject.Add(obj);
        }
    }
    public void populateAccessibiltyList(List<AccessibiltyObject> accesabilityObject, List<GameObject> list, string place, bool isSoundsDownloaded = false)
    {

        Debug.Log("AccessibilityManager->populateAccessibiltyList( Place: " + place + ", Direction: " + mydirection + ")");
        placee = place;
        accesabilityObject.Clear();

        if (mydirection == Direction.MainMenu)
        {
            StartCoroutine(main(accesabilityObject, list));
        }
        else if (place == "List1")
        {
            print("********** Going in List1 **********");
            for (int i = 0; i < list.Count; i++)
            {
                AccessibiltyObject obj = new AccessibiltyObject() { gameobject = list[i], clip = AudioFile.instance.List1[i], text = AudioFile.instance.List1Strings[i] };
                accesabilityObject.Add(obj);
            }
        }
        else if (place == "Level1Target1")
        {
            print("********** Going in Level1Target1 **********");
            for (int i = 0; i < list.Count; i++)
            {
                AccessibiltyObject obj = new AccessibiltyObject()
                {
                    gameobject = list[i],
                    clip = AudioFile.instance.Level1Target1[i],
                    text = AudioFile.instance.Target1Strings[i]
                };
                accesabilityObject.Add(obj);
            }
		}  else if (place == "Level2Target1")
		{
			print("********** Going in Level2Target1 **********");
			for (int i = 0; i < list.Count; i++)
			{
				AccessibiltyObject obj = new AccessibiltyObject()
				{
					gameobject = list[i],
					clip = AudioFile.instance.Level2Target1[i],
					text = AudioFile.instance.Target2Strings[i]
				};
				accesabilityObject.Add(obj);
			}
		}  else if (place == "Level3Target1")
		{
			print("********** Going in Level3Target1 **********");
			for (int i = 0; i < list.Count; i++)
			{
				AccessibiltyObject obj = new AccessibiltyObject()
				{
					gameobject = list[i],
					clip = AudioFile.instance.Level3Target1[i],
					text = AudioFile.instance.Target3Strings[i]
				};
				accesabilityObject.Add(obj);
			}
		}  else if (place == "Level4Target1")
		{
			print("********** Going in Level4Target1 **********");
			for (int i = 0; i < list.Count; i++)
			{
				AccessibiltyObject obj = new AccessibiltyObject()
				{
					gameobject = list[i],
					clip = AudioFile.instance.Level4Target1[i],
					text = AudioFile.instance.Target4Strings[i]
				};
				accesabilityObject.Add(obj);
			}
		}  else if (place == "Level5Target1")
		{
			print("********** Going in Level5Target1 **********");
			for (int i = 0; i < list.Count; i++)
			{
				AccessibiltyObject obj = new AccessibiltyObject()
				{
					gameobject = list[i],
					clip = AudioFile.instance.Level5Target1[i],
					text = AudioFile.instance.Target5Strings[i]
				};
				accesabilityObject.Add(obj);
			}
		}  else if (place == "Level6Target1")
		{
			print("********** Going in Level6Target1 **********");
			for (int i = 0; i < list.Count; i++)
			{
				AccessibiltyObject obj = new AccessibiltyObject()
				{
					gameobject = list[i],
					clip = AudioFile.instance.Level6Target1[i],
					text = AudioFile.instance.Target6Strings[i]
				};
				accesabilityObject.Add(obj);
			}
		}  else if (place == "Level7Target1")
		{
			print("********** Going in Level7Target1 **********");
			for (int i = 0; i < list.Count; i++)
			{
				AccessibiltyObject obj = new AccessibiltyObject()
				{
					gameobject = list[i],
					clip = AudioFile.instance.Level7Target1[i],
					text = AudioFile.instance.Target7Strings[i]
				};
				accesabilityObject.Add(obj);
			}
		}  else if (place == "Level8Target1")
		{
			print("********** Going in Level8Target1 **********");
			for (int i = 0; i < list.Count; i++)
			{
				AccessibiltyObject obj = new AccessibiltyObject()
				{
					gameobject = list[i],
					clip = AudioFile.instance.Level8Target1[i],
					text = AudioFile.instance.Target8Strings[i]
				};
				accesabilityObject.Add(obj);
			}
		}  else if (place == "Level9Target1")
		{
			print("********** Going in Level9Target1 **********");
			for (int i = 0; i < list.Count; i++)
			{
				AccessibiltyObject obj = new AccessibiltyObject()
				{
					gameobject = list[i],
					clip = AudioFile.instance.Level9Target1[i],
					text = AudioFile.instance.Target9Strings[i]
				};
				accesabilityObject.Add(obj);
			}
		}  else if (place == "Level10Target1")
		{
			print("********** Going in Level10Target1 **********");
			for (int i = 0; i < list.Count; i++)
			{
				AccessibiltyObject obj = new AccessibiltyObject()
				{
					gameobject = list[i],
					clip = AudioFile.instance.Level10Target1[i],
					text = AudioFile.instance.Target10Strings[i]
				};
				accesabilityObject.Add(obj);
			}
		}
        else if (place == "ending" && mydirection != Direction.MainMenu && mydirection != Direction.GamePlay)
        {
            print("going in ending");
            for (int i = 0; i < list.Count; i++)
            {
                AccessibiltyObject obj = new AccessibiltyObject() { gameobject = list[i], clip = AudioFile.instance.ending[i], text = AudioFile.instance.endingString[i] };
                accesabilityObject.Add(obj);
            }
        }
        else if (place == "pause")
        {
            print("going in pause");
            for (int i = 0; i < list.Count; i++)
            {
                AccessibiltyObject obj = new AccessibiltyObject() { gameobject = list[i], clip = AudioFile.instance.pause[i], text = AudioFile.instance.pauseString[i] };
                accesabilityObject.Add(obj);
            }
        }
        //destination
        populateDestinationList(accesabilityObject, list, place, isSoundsDownloaded);
    }

    public void ToggleNaviagtion(bool state)
    {
        root[mydirection].toggleNavigation(state);
    }
    ///<summary>
    /// This function is to toggle GreenBox object on/off so at time of needs it can easily be switched on/off
    ///</summary>
    public void ToogleGreenBoxState(Boolean state)
    {
        this.root[mydirection].ToogleGreenBoxState(state);
    }
    #endregion

    public void PlayAudioAndShowCaption(string text, AudioClip clip, float offset = 0, AudioClip clip2 = null)
    {
        if (!clip)
        {
            Debug.Log("Clip was null so returned");
            CloseCaption.CCManager.instance.CreateCaption(text, 1);
            return;
        }
        TextToSpeech.ins.playAudio(clip);

		if (text.Length > 0) {
			try{
			CloseCaption.CCManager.instance.CreateCaption(text, clip.length - offset);
			}
			catch (Exception e) {
				Debug.LogError ("Error: " + e);
				FindObjectOfType<CloseCaption.CCManager>().CreateCaption(text, clip.length - offset);
			
			}
		
		}
    }
}