using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayController : MonoBehaviour {

public Sprite[] spriteArray;
public GameObject img;
    public GameObject[] answers;
    public Transform[] answerTransforms;
    [HideInInspector]//Hiding it for now Because currently we are using GameManager to activate Next panels.
	public GameObject completionScreen;
	public int nextLevelIndex;
	public float endDelay;
	public int soundIndex;
	public InfoManager infoMngr;
    public static GamePlayController gamePlayController;

public void Awake()
{
    
        Debug.Log("initiailizing: "+this.gameObject.name);
        gamePlayController =this;
}
	// Use this for initialization
	void Start () {
		infoMngr.SetSoundIndex (soundIndex); 
		GameManager.Instance.ClickOn(2);
		RandomizeAnswers ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void RandomizeAnswers()
	{

        for (int i = 0; i < answerTransforms.Length; i++)
        {
            //Shuffling Answers list
            int rnd = Random.Range(0, answerTransforms.Length);
            Transform tempGO = answerTransforms[rnd];
            answerTransforms[rnd] = answerTransforms[i];
            answerTransforms[i] = tempGO;
            
        }

        if (answers.Length == answerTransforms.Length)
        {
            for (int i = 0; i < answers.Length; i++)
            {
                answers[i].gameObject.transform.localPosition = answerTransforms[i].gameObject.transform.localPosition;
            }

        }
        else
        {
            Debug.LogError("Answers and AnswersTransform Length doesnt matches.");

        }

        #region PreviousLogic
        //      int random;
        //for (int i = 0; i < 3 ;) {
        //	random = Random.Range (0, 3);
        //	if (CheckRandom (random)) {
        //		answers [i].gameObject.transform.localPosition = answerTransforms [random].gameObject.transform.localPosition;
        //		answers [i].transform.SetSiblingIndex (random);
        //		//Debug.Log ("Random " + random +" on "+i);
        //		randomCheck [random] = true;
        //		i++;
        //	}
        //}
        #endregion
    }
 public void optionsDisselection(){
      for (int i = 0; i < answers.Length; i++)
            {
                answers[i].GetComponent<Image>().enabled=false;
            }
 }
    
    public void ButtonClick(int ind)
    {
		if (GameManager.Instance.CanClick ())
        {
            switch (ind)
            {
                case 1://Correct option Selection

                    GameManager.Instance.ClickOff();
                    SoundManager.instance.PlaySound(15);
                    answers[2].GetComponent<Image>().enabled = true;
                    SoundManager.instance.PlaySound(Random.Range(1,11));
                    Invoke("ProgressLevel", endDelay);//EndDelay Being used here before level completion 
                    break;

                case 2://Incorrect option Selection

                    SoundManager.instance.PlaySound(16);

                    break;

                default:
                    Debug.Log("The Selection is neither Correct nor Incorrect");
                    break;

                        


            }
		}
	}

   
    void ProgressLevel()
	{
        changeIngredients();
		//completionScreen.SetActive (true);Commenting Cuz its already being done through Coroutine
		StartCoroutine(GameManager.Instance.StartNewLevel (nextLevelIndex, 0));//Sending Delay 0 because we ar eusing EndDelay before caling this function.
	}
    public void changeIngredients(){
        int level=0;
        for(int k=0; k<spriteArray.Length; k++){
           
            if(k== GameManager.Instance.levelCounter)
           {
             img.GetComponentInChildren<Image>().sprite=spriteArray[k];


           }
            
            
        }
    }
    
   
}
