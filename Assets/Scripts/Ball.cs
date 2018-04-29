using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Entity
{
	[SerializeField] private BallType ballType;
	[SerializeField] private GameObject snakePrefab;
	[SerializeField] private float enemyMoveCooldown;

	private Player player;

	public enum BallType
	{
		RED,
		GREEN,
		PURPLE
	}

	void Start()
	{
		entityList.Add(this);
		var grid = GameObject.FindGameObjectWithTag("LevelGrid");
		level = grid.GetComponent<Level>();
		anim = this.gameObject.GetComponent<Animator>();
		collider = this.gameObject.GetComponent<BoxCollider2D>();
		sprite = this.gameObject.GetComponent<SpriteRenderer>();
		aud = this.gameObject.GetComponent<AudioSource>();
		var p = GameObject.FindGameObjectWithTag("Player");
		player = p.GetComponent<Player>();
		state = State.SPAWN;
	}

	void Update()
	{
		switch (state)
		{
		case State.SPAWN:
			if (this.gameObject.transform.position.y <= fallTarget.y)
			{
				this.gameObject.transform.position = fallTarget;
				col = 1;
				row = (fallTarget.x < 0.0f) ? 0 : 1;
				StartCoroutine("Move");
				JumpLandCollisionCheck();
				collider.enabled = true;
			}
			else
			{
				Fall();
			}
			break;
		case State.FALL:
			if (this.gameObject.transform.position.y <= fallTarget.y)
			{
				if (this.ballType == BallType.PURPLE)
				{
					level.snakeInPlay = false;
				}
				Destroy(this.gameObject);
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

	protected override void JumpLandCollisionCheck()
	{
		/*
		var collision = level.GetTileResidentType(col, row);
		if (collision == Level.TileEntity.QBERT)
		{
			if (ballType != BallType.GREEN)
			{
				if (this.ballType == BallType.PURPLE)
				{
					level.snakeInPlay = false;
				}
				player.Cuss();
			}
			else
			{
				Destroy(this.gameObject);
			}
		}
		else
		{
			level.MoveEntityToTile(this.gameObject, col, row);
			state = State.IDLE;
			sprite.sprite = spriteSheet[direction];
			if (ballType == BallType.PURPLE && col == 6)
			{
				StopCoroutine("Move");
				StartCoroutine("HatchSnake");
			}
		}
		*/
		level.MoveEntityToTile(this.gameObject, col, row);
		state = State.IDLE;
		sprite.sprite = spriteSheet[direction];
		if (ballType == BallType.PURPLE && col == 6)
		{
			StopCoroutine("Move");
			StartCoroutine("HatchSnake");
		}
	}

	IEnumerator HatchSnake()
	{
		yield return new WaitForSeconds(1.5f);
		level.RemoveEntityFromTile(this.gameObject, col, row, false);
		var snake = Instantiate(snakePrefab, transform.position, Quaternion.identity);
		snake.GetComponent<Snake>().SetCoords(col, row);
		Destroy(this.gameObject);
	}

	IEnumerator Move()
	{
		while (true)
		{
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && state == State.IDLE)
			{
				if (col == 6)
				{
					int randint = Random.Range(0, 2);
					randint = randint * 2 + 4;
					Jump(randint);
					yield return new WaitForSeconds(enemyMoveCooldown);
				}
				if (level.GetTileResidentType(col + 1, row) != Level.TileEntity.QBERT && level.GetTileResidentType(col + 1, row) != Level.TileEntity.NONE)
				{
					if (level.GetTileResidentType(col + 1, row + 1) != Level.TileEntity.QBERT && level.GetTileResidentType(col + 1, row + 1) != Level.TileEntity.NONE)
					{
						yield return new WaitForSeconds(enemyMoveCooldown);
					}
					Jump(DIR_DOWNRIGHT);
					yield return new WaitForSeconds(enemyMoveCooldown);
				}
				else if (level.GetTileResidentType(col + 1, row + 1) != Level.TileEntity.QBERT && level.GetTileResidentType(col + 1, row + 1) != Level.TileEntity.NONE)
				{
					if (level.GetTileResidentType(col + 1, row) != Level.TileEntity.QBERT && level.GetTileResidentType(col + 1, row) != Level.TileEntity.NONE)
					{
						yield return new WaitForSeconds(enemyMoveCooldown);
					}
					Jump(DIR_DOWNLEFT);
					yield return new WaitForSeconds(enemyMoveCooldown);
				}
				else
				{
					int randint = Random.Range(0, 2);
					randint = randint * 2 + 4;
					Jump(randint);
					yield return new WaitForSeconds(enemyMoveCooldown);
				}
			}
			else
				yield return new WaitForSeconds(enemyMoveCooldown);
		}
	}

	public void SetFallTarget(Vector3 target)
	{
		fallTarget = target;
	}
}
