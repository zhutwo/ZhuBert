using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighScores : MonoBehaviour
{
	public static int[] highScores = new int[10];
	public static string[] initials = new string[10];
	[SerializeField] Text[] scoreTexts = new Text[10];
	[SerializeField] Text[] nameTexts = new Text[10];
	[SerializeField] Text prompt;
	[SerializeField] Text title;

	public static int newScore;
	int loc = 10;

	void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			nameTexts[i].text = initials[i];
			scoreTexts[i].text = highScores[i].ToString();
		}
		loc = 10;
		if (newScore > 0)
		{
			for (int i = 0; i < 10; i++)
			{
				if (newScore > highScores[i])
				{
					loc = i;
					title.text = "NEW HIGH SCORE";
					prompt.enabled = true;
					break;
				}
			}
			if (loc < 10)
			{
				for (int i = 9; i > loc; i--)
				{
					highScores[i] = highScores[i - 1];
					initials[i] = initials[i - 1];
				}
				highScores[loc] = newScore;
				newScore = 0;
				initials[loc] = "---";
				for (int i = 0; i < 10; i++)
				{
					nameTexts[i].text = initials[i];
					scoreTexts[i].text = highScores[i].ToString();
				}
				StartCoroutine("EnterInitials");
			}
		}
	}

	[RuntimeInitializeOnLoadMethod]
	public static void LoadScores()
	{
		highScores[0] = 8675309;
		initials[0] = "JEN";
		highScores[1] = 110475;
		initials[1] = "DAD";
		highScores[2] = 31400;
		initials[2] = "YYZ";
		highScores[3] = 10775;
		initials[3] = "ASH";
		highScores[4] = 8425;
		initials[4] = "PRO";
		highScores[5] = 2250;
		initials[5] = "YUI";
		highScores[6] = 575;
		initials[6] = "DDK";
		highScores[7] = 125;
		initials[7] = "HOG";
		highScores[8] = 25;
		initials[8] = "STU";
		highScores[9] = 10;
		initials[9] = "BOB";
	}

	IEnumerator EnterInitials()
	{
		int i = 0;
		string temp = "";
		while (i < 3)
		{
			foreach (char c in Input.inputString)
			{
				temp += c;
				nameTexts[loc].text = temp.ToUpper();
				initials[loc] = temp.ToUpper();
				i++;
			}
			yield return null;
		}
		yield break;
	}

	public static bool CheckHighScore(int score)
	{
		for (int i = 0; i < 10; i++)
		{
			if (highScores[i] < score)
			{
				return true;
			}
		}
		return false;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SceneManager.LoadScene("menu");
		}
	}
}
