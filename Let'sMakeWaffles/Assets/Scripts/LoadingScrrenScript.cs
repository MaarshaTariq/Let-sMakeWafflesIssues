using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScrrenScript : MonoBehaviour
{

    // Use this for initialization

    public GameObject playbutton;
    public GameObject laodingText;
    void Start()
    {
        StartCoroutine(loading());
    }

    // Update is called once per frame
    IEnumerator loading()
    {
		Debug.Log("Screen is loading");
        yield return new WaitForSeconds(3f);

        laodingText.SetActive(false);
        playbutton.SetActive(true);
    }
}
