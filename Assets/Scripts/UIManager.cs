using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
	[SerializeField] private Image[] lifeIcons = new Image[2];
	[SerializeField] private Image[] scoreIcons = new Image[8];
	[SerializeField] private Image[] bonusIcons = new Image[4];
	[SerializeField] private Sprite[] numerals = new Sprite[10];
	[SerializeField] private GameObject bonusBox;
	[SerializeField] private Text gameOverText;
	[SerializeField] private Text pauseText;
	[SerializeField] private Text flashText;
	[SerializeField] private Player player;

	private bool isPaused;
	private int extraLives = 2;
	public int score = 0;
	private int bonus = 0;
	[SerializeField] private int[] bonusDigits = new int[4];
	[SerializeField] private int[] scoreDigits = new int[8];

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!isPaused)
			{
				PauseGame();
				StartCoroutine("FlashyText");
			}
			else
			{
				StopCoroutine("FlashyText");
				UnPause();
			}
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (isPaused)
			{
				UnPause();
				SceneManager.LoadScene("menu");
			}
		}
	}

	public void SetBonus(int b)
	{
		bonusBox.SetActive(true);
		bonus = b;
		int temp = bonus;
		for (int i = 0; i < 4; i++)
		{
			bonusDigits[i] = (int)((temp / Mathf.Pow(10.0f, (float)(3 - i))) % 10);
			bonusIcons[i].sprite = numerals[bonusDigits[i]];
		}
	}

	public void SetScore(int s)
	{
		score = s;
		int temp = score;
		int count = temp.ToString().Length;
		for (int i = 0; i < count; i++)
		{
			scoreDigits[i] = (int)((temp / Mathf.Pow(10.0f, (float)(count - 1 - i))) % 10);
			scoreIcons[i].sprite = numerals[scoreDigits[i]];
			scoreIcons[i].enabled = true;
		}
	}

	public void AddScore(int s)
	{
		score += s;
		SetScore(score);
	}

	public void LoseLife()
	{
		extraLives--;
		lifeIcons[extraLives].enabled = false;
	}

	public void EnableGameOverText(bool enable)
	{
		gameOverText.enabled = enable;
	}

	IEnumerator FlashyText()
	{
		while (true)
		{
			if (flashText.enabled)
			{
				flashText.enabled = false;
			}
			else
			{
				flashText.enabled = true;
			}
			yield return new WaitForSecondsRealtime(0.5f);
		}
	}

	public void PauseGame()
	{
		Time.timeScale = 0.0f;
		player.enabled = false;
		pauseText.enabled = true;
		isPaused = true;
	}

	public void UnPause()
	{
		Time.timeScale = 1.0f;
		player.enabled = true;
		flashText.enabled = false;
		pauseText.enabled = false;
		isPaused = false;
	}
}
