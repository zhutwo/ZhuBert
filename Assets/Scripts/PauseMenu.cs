using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

	[SerializeField] private Text pauseText;
	[SerializeField] private Text flashText;
	[SerializeField] private Player player;

	private bool isPaused;

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
		Cursor.visible = true;
		Time.timeScale = 0.0f;
		player.enabled = false;
		pauseText.enabled = true;
		isPaused = true;
	}

	public void UnPause()
	{
		Cursor.visible = false;
		Time.timeScale = 1.0f;
		player.enabled = true;
		flashText.enabled = false;
		pauseText.enabled = false;
		isPaused = false;
	}
}
