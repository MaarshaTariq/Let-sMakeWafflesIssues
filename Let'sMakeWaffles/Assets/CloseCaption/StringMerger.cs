using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CloseCaption {
	public class StringMerger : MonoBehaviour {

		public static string MergeStrings (string a)
		{
			string formatedTitle = FormatIfTitle(a);

			return formatedTitle == null ? MergeStrings (new string[] { a }) : formatedTitle;
		}

		public static string MergeStrings (string a, string b)
		{
			return MergeStrings (new string[] { a, b });
		}

		public static string MergeStrings (string a, string b, string c)
		{
//			Debug.Log (a);
//			Debug.Log (b);
//			Debug.Log (c);
			return MergeStrings (new string[] { a, b, c });
		}

		public static string MergeStrings (string[] allStrings)
		{
			if (allStrings.Length == 0)
				return "";

			string finalString = "";

			if (allStrings.Length == 1)
				finalString = allStrings [0];
			else {
				finalString = allStrings [0];

				for (int i = 1; i < allStrings.Length; i++) {
					finalString += " " + allStrings[i];
				}
			}

			finalString = finalString.ToLower ();
//			Debug.Log (finalString);
			finalString  = finalString.Replace ("kindly ", "");

			while ((finalString.Length >= 2) && (finalString.Substring (0, 1) == " ")) {
				finalString = finalString.Substring (1, finalString.Length - 1);
			}

			finalString = finalString.Replace ("  ", " ");
			finalString = finalString.Replace ("  ", " ");
			finalString = finalString.Replace (".", "");

			// char[] allCharacters = finalString.ToCharArray ();
			// allCharacters [0] = char.ToUpper (allCharacters [0]);
			// finalString = new string (allCharacters);

			finalString = char.ToUpper(finalString[0]) + finalString.Substring(1, finalString.Length - 1);

//			Debug.Log (finalString);
			return finalString;
		}

        static string FormatIfTitle(string text)
        {
			if(text.Equals("PACK FOR A TRIP BEACH"))
				return "Pack for a Trip: Beach";
			else if(text.Equals("PACK FOR A TRIP BIRTHDAY PARTY"))
				return "Pack for a Trip: Birthday Party";
            else if (text.Equals("PACK FOR A TRIP CAMPING"))
                return "Pack for a Trip: Camping";
            else if (text.Equals("PACK FOR A TRIP SKIING"))
                return "Pack for a Trip: Skiing";
            else if (text.Equals("PACK FOR A TRIP CAPITAL"))
                return "Pack for a Trip: Capital";

			return null;
        }

		#region All levels strings which are supposed to be in sentence case
		public static string[] commonStrings = {"press play to start", "pause","continue","play","play again","end game",};
		public static string [] gameNumber51 = {"navigate to select items", "navigate to place", "select bag to open", "stock the kitchen", "Bag"};

		public static bool AllowCapitalize1stLetter(string str)
		{
			for (int i = 0; i < commonStrings.Length; i++)
			{
				if(commonStrings[i].Equals(str))
					return true;
			}

			if(GameManager.Instance.GameNumber(51))
            {
                for (int i = 0; i < gameNumber51.Length; i++)
                {
                    if (gameNumber51[i].Equals(str))
                        return true;
                }
            }

			return false;
		}
		#endregion
	}
}