using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionVerification : MonoBehaviour
{
    bool isCorrectOption;
    int correctCount = 0;
    public int maxObjectsToSelect;
    public int chanageTargetBy;
    public int chanageDestinationBy;

    public Destination destination;
    public TargetScript target;
    void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Accessibilty && AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.DestinationSelected += check;
//            AccessibilityManager.instance.levelCompleted += LevleCompletedAccessibility;
        }

    }

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.Accessibilty && AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.DestinationSelected += check;
            //  AccessibilityManager.instance.levelCompleted += LevleCompletedAccessibility;
        }
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if (GameManager.Instance.Accessibilty)
        {
            AccessibilityManager.instance.DestinationSelected -= check;
            // AccessibilityManager.instance.levelCompleted -= LevleCompletedAccessibility;
        }

    }

    public void Check(bool isCorrectOption)
    {
        if (!GameManager.Instance.Accessibilty)
            return;

        this.isCorrectOption = isCorrectOption;
        check(null, null);  // because in this game functionality is implemented in onClick, so no target or destination required
    }

    public void Check(int chanageTargetBy)
    {
        if (!GameManager.Instance.Accessibilty)
            return;

        this.chanageTargetBy = chanageTargetBy;
        this.isCorrectOption = true;
        check(null, null);  // because in this game functionality is implemented in onClick, so no target or destination required
    }

    public void AutoCheck()
    {
        if (!GameManager.Instance.Accessibilty)
            return;

        if(GameManager.Instance.GameNumber(70))
            isCorrectOption = transform.GetChild(0).GetChild(0).tag == "Correct";

        check(null, null); 
    }

    void check(GameObject target, GameObject Destination)
    {
        Debug.Log("SelectionVerification->check(" + isCorrectOption + ")");

        if (isCorrectOption)
        {
            correctCount++;

            FreezeControlsHandler.Instance.FreezeControlls();

            if (GameManager.Instance.Accessibilty)
            {
                AccessibilityManager.instance.ToggleNaviagtion(false);

                //     AccessibilityManager.instance.SwitchToNextState(null);

                //if(GameManager.Instance.GameNumber(70))
                  //  Game70();
                //else
               // {
                    AccessibilityManager.instance.changeTarget(chanageTargetBy);
                    AccessibilityManager.instance.changeDestination(chanageDestinationBy);
                //}
            }
        }
        else
        {
            if (GameManager.Instance.Accessibilty)
            {
                // AccessibilityManager.instance.ToggleNaviagtion(false);
                // StartCoroutine(playAccessiblrSound());
            }
        }

        this.isCorrectOption = false;
    }

    IEnumerator playAccessiblrSound()
    {
        yield return new WaitForSeconds(2f);

        AccessibilityManager.instance.ToggleNaviagtion(true);
    }

    public void SwitchToNextState(GameObject selectedTarget)
    {
        if(GameManager.Instance.Accessibilty)
        {
            Debug.Log("SelectionVerification->SwitchToNextState(" + selectedTarget + ")");
            AccessibilityManager.instance.SwitchToNextState(selectedTarget);

            if(GameManager.Instance.GameNumber(51))
            {
                Debug.Log("AccessibilityManager.instance.populateAccessibiltyList");
                AccessibilityManager.instance.populateAccessibiltyList(destination.accesabilityObject, destination.list, destination.uniqueText);
            }
        }
    }

    public void SwitchToPrevState()
    {
        AccessibilityManager.instance.SwitchToPreviousState();
    }

    public void SwitchToNextLevel()
    {
        if(GameManager.Instance.Accessibilty)
        {
            Debug.Log("SelectionVerification->SwitchToNextLevel()");
            AccessibilityManager.instance.changeTarget();

            if(GameManager.Instance.GameNumber(62))
            {
                Debug.Log("AccessibilityManager.instance.populateAccessibiltyList");
                AccessibilityManager.instance.populateAccessibiltyList(target.accesabilityObject, target.list, target.uniqueText);
            }
        }
    }

    #region Game Numbers
  //  public void Game62()
//    {
//        if(!GameManager.Instance.Accessibilty)
//            return;
//
//        // Calling Show Score function by using extera resorces in current list from TrayTrigger
//        AccessibilityManager.instance.levelsList[AccessibilityManager.instance.LevelCounter].
//        GetComponent<listhandler>().extraResource[0].GetComponent<TrayTrigger>().ShowScore();
//    }

//    public void Game65(GameObject selectedObject)
//    {
//        if(!GameManager.Instance.Accessibilty)
//            return;
//
//        Debug.Log("selectedObject: " + selectedObject.name);
//        
//        AccessibilityManager.instance.levelsList[AccessibilityManager.instance.LevelCounter].GetComponent<listhandler>().
//        extraResource[0].GetComponent<ErrandsController>().PutInsideAccessibility(selectedObject);
//    }

//     private void Game70()
//    {
//        //Debug.Log("SelectionVerification->Game70(" + DinoLevelController.controller.LevelNumber + ", " + AssetDownloader.LevelArray.Count + ")");
//        
//        if (DinoLevelController.controller.LevelNumber + 1 == AssetDownloader.LevelArray.Count)
//            AccessibilityManager.instance.changeTarget();
//        else
//        {
//            AccessibilityManager.instance.changeTarget(chanageTargetBy);
//            AccessibilityManager.instance.changeDestination(chanageDestinationBy);
//            transform.parent.GetComponent<TargetScript>().OnEnableFunctionality();
//        }
//    }
    #endregion
}