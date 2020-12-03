using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloseCaption {

	public enum CCPriority {Lowest, Low, Medium, High, Highest} // Low for bg, High for dialogues
	public enum CCPlacement {Bottom, Top}

	public class CCManager : MonoBehaviour {

		public CCPlacement placementPos;
//		public List<Caption> captionsList;

		public Caption[] captionArray;

		public GameObject top, bottom;
		public Text topText, bottomText;

		public string[] seqStrings;
		public float[] seqTimes;
		[System.Serializable]
        public class SubSentencesLengths
        {
            public float[] subSentencesLengths;
        }
        public SubSentencesLengths[] subSentencesLengths;

		public static CCManager instance;
		void Awake()
		{
			instance = this;
//			captionsList = new List<Caption> ();
			captionArray = new Caption[5];
		}

		public void LowCaption(string displayText)
		{
			CreateCaption (CCPriority.Medium, displayText, 1f);
		}

		public void CreateCaption (int seqStringIndex, float displayTime)
		{
			if (seqStringIndex < seqStrings.Length) {
				string captionText = seqStrings [seqStringIndex];
				float captionTime = displayTime;

				if (seqStringIndex < seqTimes.Length) {
					if (seqTimes [seqStringIndex] > 0f)
						captionTime = seqTimes [seqStringIndex];
				}

				if (captionText.Contains ("Order:"))
					CreateCaption (CCPriority.High, seqStrings [seqStringIndex], captionTime);
				else if (captionText.Contains ("["))
					CreateCaption (CCPriority.Low, seqStrings [seqStringIndex], captionTime);
				else
					CreateCaption (CCPriority.Medium, seqStrings [seqStringIndex], captionTime);
				
			} else {
				Debug.LogError ("Index OutOfBound Exception: " + seqStringIndex);
			}
		}

		public void CreateCaption (CCPriority priority, int seqStringIndex, float displayTime)
		{
			if (seqStringIndex < seqStrings.Length)
				CreateCaption (priority, seqStrings [seqStringIndex], displayTime);
			else
				Debug.LogError ("Index OutOfBound Exception: " + seqStringIndex);
		}

		public void CreateCaption (CCPriority priority, int seqStringIndex, float displayTime, CCPlacement placement)
		{
			if (seqStringIndex < seqStrings.Length)
				CreateCaption (seqStrings [seqStringIndex], displayTime, priority, placement);
			else
				Debug.LogError ("Index OutOfBound Exception: " + seqStringIndex);
		}

		public void CreateCaption (string displayText, float displayTime, CCPriority priority = CCPriority.Medium, CCPlacement placement = CCPlacement.Bottom)
		{
			if(GameManager.Instance.GameNumber(93,94,95))
			 placement = CCPlacement.Top;

			UpdatePlacementPosition (placement);
			CreateCaption (priority, displayText, displayTime);
		}

		public void CreateCaption (CCPriority priority, string displayText, float displayTime)
		{
			if(GameManager.Instance != null && !GameManager.Instance.Accessibilty)
				return;


			#if UNITY_EDITOR
			if (displayTime <= 0f)
				displayTime = 1f;
			#endif

//			Debug.Log("prev "+displayTime );
			if (displayText == "You have spent $0. Use bills only.") {
				

				Debug.Log("now "+displayTime );
			}
			Caption caption = new Caption ();
			caption.priority = priority;
			caption.displayText = displayText;
			caption.endTime = Time.time + displayTime;
//			captionsList.Add (caption);

			captionArray [(int)caption.priority] = caption;

			Debug.Log ("Added: " + caption.priority.ToString () + ": " + caption.displayText + ", " + caption.endTime);
			UpdatePlacementPosition (placementPos);
			UpdateCaptions ();
		}

		public void UpdatePlacementPosition (CCPlacement newPlacementPos)
		{
			if(GameManager.Instance != null && !GameManager.Instance.Accessibilty)
				return;

			placementPos = newPlacementPos;
			// top = transform.GetChild(0).gameObject
			transform.GetChild(0).gameObject.SetActive (placementPos == CCPlacement.Top);
			// bottom = transform.GetChild(1).gameObject
			transform.GetChild(1).gameObject.SetActive (placementPos == CCPlacement.Bottom);
		}

		void Update()
		{
			if (Input.GetMouseButtonDown (0)) {
				UpdateCaptions ();
			}
		}

		void UpdateCaptions()
		{
			if(GameManager.Instance != null && !GameManager.Instance.Accessibilty)
				return;
				
//			Debug.Log ("UpdateCaptions");

//			for (int i = captionsList.Count - 1; i >= 0; i--) {
//				if ((captionsList [i].endTime <= Time.time) || (captionsList [i].displayText == "")) {
//					captionsList.Remove (captionsList [i]);
//				}
//			}
//			captionsList.Sort ((c1, c2) => c1.priority.CompareTo (c2.priority));

//			foreach (Caption caption in captionsList) {
//				Debug.Log (caption.priority.ToString () + ": " + caption.displayText + ", " + caption.endTime);
//			}

			for (int i = captionArray.Length - 1; i >= 0; i--) {
				if (captionArray [i].endTime > Time.time) {
					Invoke ("UpdateCaptions", captionArray [i].endTime - Time.time);
					topText.text = captionArray [i].displayText;
					bottomText.text = captionArray [i].displayText;
					return;
				}
			}

			top.SetActive (false);
			bottom.SetActive (false);
			/*
			// Update the last caption
			if (captionsList.Count > 0) {
				Invoke ("UpdateCaptions", captionsList [captionsList.Count - 1].endTime - Time.time);
				topText.text = captionsList [captionsList.Count - 1].displayText;
				bottomText.text = captionsList [captionsList.Count - 1].displayText;
			} else {
				top.SetActive (false);
				bottom.SetActive (false);
			}*/
		}

		public void CreateCaptionsWithSubSentences(string paragraph, int lengthsIndex = 0, float displayTime = 0)
        {
            string[] sentences = paragraph.Split('/');

            if (sentences.Length == 1)
                CreateCaption(paragraph, displayTime);
            else
				StartCoroutine(CreateCaptionsWithSubSentencesE(sentences, lengthsIndex));
        }

		public void CreateCaptionsWithSubSentences(int paragraphIndex, float displayTime = 0)
        {
            string[] sentences = seqStrings[paragraphIndex].Split('/');

            if (sentences.Length == 1)
                CreateCaption(paragraphIndex, displayTime);
            else
				StartCoroutine(CreateCaptionsWithSubSentencesE(sentences, paragraphIndex));
        }

        IEnumerator CreateCaptionsWithSubSentencesE(string[] sentences, int paragraphIndex)
        {
			for(int i = 0; i < sentences.Length; i++)
			{
				 CreateCaption(sentences[i], subSentencesLengths[paragraphIndex].subSentencesLengths[i]);
				 yield return new WaitForSeconds(subSentencesLengths[paragraphIndex].subSentencesLengths[i]);
			}
        }
	}

	[System.Serializable]
	public struct Caption {
		public CCPriority priority;
		public string displayText;
		public float endTime;
	}
}