using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

	[SerializeField] GameObject qbertAnim;
	[SerializeField] AudioSource music;
	[SerializeField] float animSpeed;
	[SerializeField] Text credits;
	[SerializeField] Text insertcoin;

	static int numCredits;
	Color def;

	// Use this for initialization
	void Start()
	{
		Time.timeScale = 1.0f;
		credits.text = "CREDITS: " + numCredits.ToString();
		music.PlayDelayed(1.2f);
		StartCoroutine("InsertCoin");
		def = credits.color;
	}
	
	// Update is called once per frame
	void Update()
	{
		qbertAnim.transform.Translate(Vector3.right * animSpeed * Time.deltaTime);
		if (qbertAnim.transform.position.x >= 28.0f)
		{
			qbertAnim.transform.position = new Vector3(-28.0f, -6.0f);
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	IEnumerator InsertCoin()
	{
		while (true)
		{
			if (insertcoin.enabled)
			{
				insertcoin.enabled = false;
			}
			else
			{
				insertcoin.enabled = true;
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void AddCredits()
	{
		numCredits++;
		credits.text = "CREDITS: " + numCredits.ToString();
		credits.color = def;
	}

	public void HighScores()
	{
		StopAllCoroutines();
		SceneManager.LoadScene("highscores");
	}

	public void StartGame()
	{
		if (numCredits > 0)
		{
			numCredits--;
			StopAllCoroutines();
			SceneManager.LoadScene("level1");
		}
		else
		{
			credits.color = Color.red;
		}
	}
}
