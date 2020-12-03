using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPost : MonoBehaviour
{
    void OnEnable()
    {
        if (EventController.instance != null && !External.Instance.Preview)
            EventController.instance.currentGamePercentage();
        else
            Debug.LogError("Event Controller instance does not exist or you are in preview mode...");
    }
}
