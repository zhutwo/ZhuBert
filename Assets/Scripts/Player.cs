using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
	[SerializeField] GameObject sailorMouth;
	[SerializeField] UIManager ui;
	[SerializeField] AudioClip fall;
	[SerializeField] AudioClip cuss;
	[SerializeField] AudioClip elev;
	[SerializeField] AudioClip bonus;
	private GameObject elevRiding;

	const KeyCode KEY_UPRIGHT = KeyCode.W;
	const KeyCode KEY_UPLEFT = KeyCode.Q;
	const KeyCode KEY_DOWNRIGHT = KeyCode.S;
	const KeyCode KEY_DOWNLEFT = KeyCode.A;

	private Vector3 elevTarget;
	private Vector3 elevDirection;
	private bool elevSpawn;
	private int lives;
	[SerializeField] private int score;

	void Start()
	{
		var grid = GameObject.FindGameObjectWithTag("LevelGrid");
		level = grid.GetComponent<Level>();
		anim = this.gameObject.GetComponent<Animator>();
		sprite = this.gameObject.GetComponent<SpriteRenderer>();
		collider = this.gameObject.GetComponent<BoxCollider2D>();
		aud = this.gameObject.GetComponent<AudioSource>();
		elevTarget = new Vector3(0.0f, 4.0f, 0.0f);
		state = State.IDLE;
		lives = 3;
		score = 0;
	}

	void Update()
	{
		switch (state)
		{
		case State.IDLE:
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				GetInput();
			}
			break;
		case State.ELEVRIDE:
			if (this.gameObject.transform.position.y >= elevTarget.y)
			{
				this.gameObject.transform.position = elevTarget;
				state = State.WAIT;
				StartCoroutine("ElevDismount");
			}
			else
			{
				elevDirection = elevTarget - this.gameObject.transform.position;
				elevDirection.Normalize();
				this.gameObject.transform.Translate(elevDirection * spawnSpeed * Time.deltaTime);
			}
			break;
		case State.SPAWN:
			if (this.gameObject.transform.position.y <= fallTarget.y)
			{
				this.gameObject.transform.position = fallTarget;
				if (elevSpawn)
				{
					ChangeTile(0, 0);
					level.RemoveElevator(col, row, (direction == DIR_UPRIGHT) ? true : false);
					elevSpawn = false;
				}
				col = 0;
				row = 0;
				if (direction < 4)
				{
					direction += 4;
				}
				sprite.sprite = spriteSheet[direction];
				level.MoveEntityToTile(this.gameObject, col, row);
				anim.SetInteger("Direction", direction);
				anim.SetInteger("State", 0);
				collider.enabled = true;
				state = State.IDLE;
			}
			else
			{
				Fall();
			}
			break;
		case State.FALL:
			if (this.gameObject.transform.position.y <= fallTarget.y)
			{
				lives--;
				if (lives <= 0)
				{
					
					StartCoroutine(GameOver(false));
				}
				else
				{
					StartCoroutine("CliffRespawn");
				}
			}
			else
			{
				Fall();
			}
			break;
		default:
			break;
		}
	}

	void GetInput()
	{
		if (Input.GetKeyDown(KEY_UPRIGHT) || Input.GetKeyDown(KeyCode.Keypad9))
		{
			if (level.HasElevator(col, row, true))
			{
				ElevJump(DIR_UPRIGHT);
			}
			else
			{
				Jump(DIR_UPRIGHT);
			}
		}
		else if (Input.GetKeyDown(KEY_UPLEFT) || Input.GetKeyDown(KeyCode.Keypad7))
		{
			if (level.HasElevator(col, row, false))
			{
				ElevJump(DIR_UPLEFT);
			}
			else
			{
				Jump(DIR_UPLEFT);
			}
		}
		else if (Input.GetKeyDown(KEY_DOWNRIGHT) || Input.GetKeyDown(KeyCode.Keypad3))
		{
			Jump(DIR_DOWNRIGHT);
		}
		else if (Input.GetKeyDown(KEY_DOWNLEFT) || Input.GetKeyDown(KeyCode.Keypad1))
		{
			Jump(DIR_DOWNLEFT);
		}
	}

	void ElevJump(int dir)
	{
		aud.Play();
		collider.enabled = false;
		direction = dir;
		state = State.ELEVJUMP;
		sprite.sprite = spriteSheet[direction + 1];
		anim.SetInteger("State", 3);
		anim.SetInteger("Direction", direction);
		level.RemoveEntityFromTile(this.gameObject, col, row);
	}

	void ElevJumpLand()
	{
		aud.PlayOneShot(elev);
		if (direction == DIR_UPRIGHT)
		{
			level.BindElevToPlayer(true);
		}
		else
		{
			level.BindElevToPlayer(false);
		}
	}

	void ElevDeployFinish()
	{
		state = State.ELEVRIDE;
	}

	IEnumerator ElevDismount()
	{
		yield return new WaitForSeconds(0.5f);
		level.HideElevator((direction == DIR_UPRIGHT) ? true : false);
		yield return new WaitForSeconds(0.5f);
		state = State.SPAWN;
		fallTarget = new Vector3(0.0f, 0.0f, 0.0f);
		elevSpawn = true;
	}

	IEnumerator CliffRespawn()
	{
		state = State.WAIT;
		this.gameObject.transform.position = level.GetTileVec3f(0, 0);
		sprite.sortingOrder = 2;
		sprite.enabled = false;
		level.StopAllCoroutines();
		Time.timeScale = 0.0f;
		yield return new WaitForSecondsRealtime(0.5f);
		level.ClearEntities();
		Time.timeScale = 1.0f;
		yield return new WaitForSeconds(0.5f);
		col = 0;
		row = 0;
		direction = DIR_DOWNLEFT;
		sprite.sprite = spriteSheet[direction];
		level.MoveEntityToTile(this.gameObject, col, row);
		anim.SetInteger("Direction", direction);
		anim.SetInteger("State", 0);
		sprite.enabled = true;
		collider.enabled = true;
		ui.LoseLife();
		state = State.IDLE;
		level.StartCoroutine(level.SpawnTimer());
	}

	IEnumerator GreenBallEffect()
	{
		level.FreezeEnemies(true);
		yield return new WaitForSeconds(5.0f);
		level.FreezeEnemies(false);
	}

	public void Cuss()
	{
		sailorMouth.SetActive(true);
		aud.PlayOneShot(cuss);
		state = State.WAIT;
		Time.timeScale = 0.0f;
		lives--;
		if (lives <= 0)
		{
			StopAllCoroutines();
			StartCoroutine(GameOver(true));
		}
		else
		{
			StartCoroutine("Death");
		}
	}

	IEnumerator Death()
	{
		yield return new WaitForSecondsRealtime(1.0f);
		sailorMouth.SetActive(false);
		sprite.enabled = false;
		level.ClearEntities();
		StartCoroutine("Life");
	}

	IEnumerator Life()
	{
		yield return new WaitForSecondsRealtime(0.4f);
		level.MoveEntityToTile(this.gameObject, col, row);
		this.gameObject.transform.position = level.GetTileVec3f(col, row);
		anim.SetInteger("Direction", direction);
		anim.SetInteger("State", 0);
		state = State.IDLE;
		direction = DIR_DOWNLEFT;
		sprite.sprite = spriteSheet[direction];
		sprite.enabled = true;
		ui.LoseLife();
		Time.timeScale = 1.0f;
	}

	IEnumerator GameOver(bool cuss)
	{
		if (cuss)
		{
			yield return new WaitForSecondsRealtime(1.0f);
			sailorMouth.SetActive(false);
		}
		Time.timeScale = 0.0f;
		yield return new WaitForSecondsRealtime(1.0f);
		level.ClearEntities();
		sprite.enabled = false;
		state = State.WAIT;
		level.StopAllCoroutines();
		ui.EnableGameOverText(true);
		yield return new WaitForSecondsRealtime(3.0f);
		Time.timeScale = 1.0f;
		if (HighScores.CheckHighScore(score))
		{
			HighScores.newScore = score;
			SceneManager.LoadScene("highscores");
		}
		else
		{
			SceneManager.LoadScene("menu");
		}
	}

	protected override void FallSound()
	{
		aud.PlayOneShot(fall);
	}

	protected override void JumpLandCollisionCheck()
	{
		/*
		var collision = level.GetTileResidentType(col, row);
		if (collision != Level.TileEntity.NONE && collision != Level.TileEntity.GREEN)
		{
			Cuss();
		}
		else
		{
			if (collision == Level.TileEntity.GREEN)
			{
				var green = level.GetTileResidentObject(col, row);
				level.RemoveEntityFromTile(green, col, row, true);
				IncreaseScore(100);
				StartCoroutine(GreenBallEffect());
			}
			level.MoveEntityToTile(this.gameObject, col, row);
			state = State.IDLE;
			sprite.sprite = spriteSheet[direction];
			ChangeTile(col, row);
		}
		*/
		level.MoveEntityToTile(this.gameObject, col, row);
		state = State.IDLE;
		sprite.sprite = spriteSheet[direction];
		ChangeTile(col, row);
	}

	public void IncreaseScore(int toAdd)
	{
		score += toAdd;
		ui.SetScore(score);
	}

	void ChangeTile(int col, int row)
	{
		if (level.ChangeTile(col, row))
		{
			IncreaseScore(25);
			level.CheckLevelComplete();
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		var ent = other.gameObject.GetComponent<Entity>();
		ent.Kill();
		if (ent.GetEntityType() != Level.TileEntity.GREEN)
		{
			Cuss();
		}
		else
		{
			IncreaseScore(100);
			aud.PlayOneShot(bonus);
			StartCoroutine(GreenBallEffect());
			Destroy(other.gameObject);
		}
	}
}
