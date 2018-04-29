using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
	[SerializeField] private UIManager ui;
	[SerializeField] private Tilemap pyramid;
	[SerializeField] private Tile changeTile;
	[SerializeField] private Tile winTile;
	[SerializeField] private Tile baseTile;
	[SerializeField] private Sprite changeTileUI;
	[SerializeField] private Sprite winTileUI;
	[SerializeField] private Sprite baseTileUI;
	[SerializeField] private Image tileUI;
	[SerializeField] private const float spawnCooldown = 4.0f;
	[SerializeField] private Transform rightSpawn;
	[SerializeField] private Transform leftSpawn;
	[SerializeField] public GameObject elevRight;
	[SerializeField] public GameObject elevLeft;
	[SerializeField] private GameObject elevPrefab;
	[SerializeField] private GameObject redPrefab;
	[SerializeField] private GameObject greenPrefab;
	[SerializeField] private GameObject purplePrefab;
	[SerializeField] private AudioClip elevBonus;
	private TileData[][] tileArray;
	private GameObject player;
	private AudioSource aud;


	private int tilesLeft;
	private int snakeSpawnCounter;
	public bool snakeInPlay;

	public enum TileEntity
	{
		NONE,
		QBERT,
		SNAKE,
		RED,
		PURPLE,
		GREEN
	}

	public struct TileData
	{
		public Vector3 vec3f;
		public Vector3Int vec3int;
		public TileEntity residentType;
		public GameObject residentObject;
		public bool tileChanged;
		public bool hasElevLeft;
		public bool hasElevRight;
	}

	void Start()
	{
		// creating array of tile coordinates by pyramid column
		tileArray = new TileData[7][];
		for (int i = 0; i < 7; i++)
		{
			// creating arrays for each pyramid row
			tileArray[i] = new TileData[i + 1];
			for (int j = 0; j < (i + 1); j++)
			{
				int x = i * (-2) + j * 4;
				int y = i * (-3);
				tileArray[i][j].vec3int = new Vector3Int(x, y, 0);
				tileArray[i][j].vec3f = new Vector3((float)x, (float)y, 0.0f);
				tileArray[i][j].residentType = TileEntity.NONE;
			}
		}
		AddElevator(4, 4, true);
		AddElevator(4, 0, false);
		StartCoroutine(SpawnTimer());
		player = GameObject.FindGameObjectWithTag("Player");
		aud = gameObject.GetComponent<AudioSource>();
		MoveEntityToTile(player, 0, 0);
		tilesLeft = 28;
	}

	public void PauseSpawns(float seconds)
	{
		StopAllCoroutines();
		StartCoroutine(SpawnTimer(seconds));
	}

	IEnumerator Win()
	{
		Time.timeScale = 0.0f;
		aud.Play();
		player.GetComponent<Player>().state = Entity.State.WAIT;
		Tile toSet = baseTile;
		Sprite toSetUI = baseTileUI;
		int index = 3;
		while (index < 21)
		{
			switch (index % 3)
			{
			case 0:
				toSet = baseTile;
				toSetUI = baseTileUI;
				break;
			case 1:
				toSet = winTile;
				toSetUI = winTileUI;
				break;
			case 2:
				toSet = changeTile;
				toSetUI = changeTileUI;
				break;
			}
			index++;
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < (i + 1); j++)
				{
					pyramid.SetTile(tileArray[i][j].vec3int, toSet);
				}
			}
			tileUI.sprite = toSetUI;
			yield return new WaitForSecondsRealtime(0.1f);
		}
		yield return new WaitForSecondsRealtime(0.5f);
		ClearEntities();
		player.SetActive(false);
		int bonus = 1000;
		ui.SetBonus(bonus);
		ui.AddScore(bonus);
		yield return new WaitForSecondsRealtime(1.0f);
		if (elevRight != null)
		{
			bonus += 100;
			ui.SetBonus(bonus);
			ui.AddScore(100);
			elevRight.SetActive(false);
			aud.PlayOneShot(elevBonus);
			yield return new WaitForSecondsRealtime(0.5f);
		}
		if (elevLeft != null)
		{
			bonus += 100;
			ui.SetBonus(bonus);
			ui.AddScore(100);
			elevLeft.SetActive(false);
			aud.PlayOneShot(elevBonus);
			yield return new WaitForSecondsRealtime(0.5f);
		}
		yield return new WaitForSecondsRealtime(0.5f);
		Time.timeScale = 1.0f;
		if (HighScores.CheckHighScore(ui.score))
		{
			HighScores.newScore = ui.score;
			SceneManager.LoadScene("highscores");
		}
		else
		{
			SceneManager.LoadScene("menu");
		}
	}

	public IEnumerator SpawnTimer(float delay = spawnCooldown)
	{
		yield return new WaitForSeconds(delay);
		while (true)
		{
			int randint = Random.Range(0, 4);
			if (!snakeInPlay)
			{
				snakeSpawnCounter++;
				if (randint <= snakeSpawnCounter)
				{
					SpawnBall(Ball.BallType.PURPLE);
					yield return new WaitForSeconds(spawnCooldown);
				}
			}
			if (randint < 3)
			{
				SpawnBall(Ball.BallType.RED);
			}
			else
			{
				SpawnBall(Ball.BallType.GREEN);
			}
			yield return new WaitForSeconds(spawnCooldown);
		}
	}

	void SpawnBall(Ball.BallType type)
	{
		int spawnSide = Random.Range(0, 2);
		Vector3 spawnPos;
		GameObject newBall;
		if (spawnSide == 0)
		{
			spawnPos = rightSpawn.position;
		}
		else
		{
			spawnPos = leftSpawn.position;
		}
		switch (type)
		{
		case Ball.BallType.GREEN:
			newBall = Instantiate(greenPrefab, spawnPos, Quaternion.identity);
			spawnPos.y -= 7.0f;
			newBall.GetComponent<Ball>().SetFallTarget(spawnPos);
			break;
		case Ball.BallType.RED:
			newBall = Instantiate(redPrefab, spawnPos, Quaternion.identity);
			spawnPos.y -= 7.0f;
			newBall.GetComponent<Ball>().SetFallTarget(spawnPos);
			break;
		case Ball.BallType.PURPLE:
			newBall = Instantiate(purplePrefab, spawnPos, Quaternion.identity);
			spawnPos.y -= 7.0f;
			newBall.GetComponent<Ball>().SetFallTarget(spawnPos);
			snakeInPlay = true;
			snakeSpawnCounter = 0;
			break;
		}

	}

	public void AddElevator(int col, int row, bool onRightSide)
	{
		if (onRightSide)
		{
			if (!tileArray[col][row].hasElevRight)
			{
				Vector3 spawn = GetTileVec3f(col, row);
				spawn.y += 2.0f;
				spawn.x += 2.0f;
				elevRight = Instantiate(elevPrefab, spawn, Quaternion.identity);
				elevRight.GetComponent<Elevator>().SetTile(col, row);
				tileArray[col][row].hasElevRight = true;
			}
		}
		else
		{
			if (!tileArray[col][row].hasElevLeft)
			{
				Vector3 spawn = GetTileVec3f(col, row);
				spawn.y += 2.0f;
				spawn.x -= 2.0f;
				elevLeft = Instantiate(elevPrefab, spawn, Quaternion.identity);
				elevLeft.GetComponent<Elevator>().SetTile(col, row);
				tileArray[col][row].hasElevLeft = true;
			}
		}
	}

	public void RemoveElevator(int col, int row, bool onRightSide)
	{
		if (onRightSide)
		{
			tileArray[col][row].hasElevRight = false;
			Destroy(elevRight);
		}
		else
		{
			tileArray[col][row].hasElevLeft = false;
			Destroy(elevLeft);
		}
	}

	public void HideElevator(bool onRightSide)
	{
		if (onRightSide)
		{
			elevRight.GetComponent<SpriteRenderer>().enabled = false;
		}
		else
		{
			elevLeft.GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	public bool HasElevator(int col, int row, bool onRightSide)
	{
		if (onRightSide)
		{
			return tileArray[col][row].hasElevRight;
		}
		else
		{
			return tileArray[col][row].hasElevLeft;
		}
	}

	public void BindElevToPlayer(bool onRightSide)
	{
		if (onRightSide)
		{
			elevRight.GetComponent<Elevator>().SetRiding();
		}
		else
		{
			elevLeft.GetComponent<Elevator>().SetRiding();
		}
	}

	public bool ChangeTile(int col, int row)
	{
		if (!tileArray[col][row].tileChanged)
		{
			pyramid.SetTile(tileArray[col][row].vec3int, changeTile);
			tileArray[col][row].tileChanged = true;
			tilesLeft--;
			return true;
		}
		return false;
	}

	public void CheckLevelComplete()
	{
		if (tilesLeft <= 0)
		{
			StopAllCoroutines();
			StartCoroutine(Win());
		}
	}

	public Vector3 GetTileVec3f(int col, int row)
	{
		int x = col * (-2) + row * 4;
		int y = col * (-3);
		Vector3 vec3 = new Vector3((float)x, (float)y, 0.0f);
		return vec3;
		//return tileArray[col][row].vec3f;
	}

	public TileEntity GetTileResidentType(int col, int row)
	{
		return tileArray[col][row].residentType;
	}

	public GameObject GetTileResidentObject(int col, int row)
	{
		return tileArray[col][row].residentObject;
	}

	public void MoveEntityToTile(GameObject newResident, int col, int row)
	{
		tileArray[col][row].residentObject = newResident;
		tileArray[col][row].residentType = newResident.GetComponent<Entity>().GetEntityType();
	}

	public void RemoveEntityFromTile(GameObject oldResident, int col, int row, bool destroy = false)
	{
		if (tileArray[col][row].residentObject == oldResident)
		{
			tileArray[col][row].residentType = TileEntity.NONE;
			if (destroy)
			{
				Destroy(tileArray[col][row].residentObject);
			}
			else
			{
				tileArray[col][row].residentObject = null;
			}
		}
	}

	public void ClearEntities()
	{
		if (snakeInPlay)
		{
			snakeInPlay = false;
		}
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < (i + 1); j++)
			{
				if (tileArray[i][j].residentType != TileEntity.QBERT || tileArray[i][j].residentType != TileEntity.NONE)
				{
					var e = GetTileResidentObject(i, j);
					RemoveEntityFromTile(e, i, j, false);
				}
			}
		}
		foreach (Entity e in Entity.entityList)
		{
			Destroy(e.gameObject);
		}
	}

	public void FreezeEnemies(bool freeze)
	{
		foreach (Entity e in Entity.entityList)
		{
			if (e.GetEntityType() != Level.TileEntity.QBERT)
			{
				e.FreezeAnim(freeze);
			}
		}
		if (freeze)
		{
			StopAllCoroutines();
		}
		else
		{
			StartCoroutine(SpawnTimer());
		}
	}
}
