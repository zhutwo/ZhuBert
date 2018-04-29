using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Entity
{
	[SerializeField] private float enemyMoveCooldown;
	[SerializeField] private AudioClip fall;

	private Player player;

	void Start()
	{
		aud = this.gameObject.GetComponent<AudioSource>();
		entityList.Add(this);
		var grid = GameObject.FindGameObjectWithTag("LevelGrid");
		level = grid.GetComponent<Level>();
		anim = this.gameObject.GetComponent<Animator>();
		collider = this.gameObject.GetComponent<BoxCollider2D>();
		sprite = this.gameObject.GetComponent<SpriteRenderer>();
		var p = GameObject.FindGameObjectWithTag("Player");
		player = p.GetComponent<Player>();
		state = State.SPAWN;
	}

	void Update()
	{
		if (state == State.SPAWN)
		{
			if (level.GetTileResidentType(col, row) != Level.TileEntity.SNAKE)
			{
				level.MoveEntityToTile(this.gameObject, col, row);
				state = State.IDLE;
				StartCoroutine("Move");
			}
		}
		else if (state == State.FALL)
		{
			if (this.gameObject.transform.position.y <= fallTarget.y)
			{
				level.snakeInPlay = false;
				player.IncreaseScore(500);
				level.ClearEntities();
				level.PauseSpawns(5.0f);
				Destroy(this.gameObject);
			}
			else
			{
				Fall();
			}
		}
	}

	protected override void JumpLandCollisionCheck()
	{
		/*
		var collision = level.GetTileResidentType(col, row);
		if (collision == Level.TileEntity.QBERT)
		{
			level.snakeInPlay = false;
			player.Cuss();
		}
		else
		{
			level.MoveEntityToTile(this.gameObject, col, row);
			state = State.IDLE;
			sprite.sprite = spriteSheet[direction];
		}
		*/
		level.MoveEntityToTile(this.gameObject, col, row);
		state = State.IDLE;
		sprite.sprite = spriteSheet[direction];
	}

	IEnumerator Move()
	{
		while (true)
		{
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && state == State.IDLE)
			{
				if (level.HasElevator(col, row, true))
				{
					bool riding = level.elevRight.GetComponent<Elevator>().IsRiding();
					if (riding)
					{
						aud.PlayOneShot(fall);
						Jump(DIR_UPRIGHT);
						yield break;
					}
				}
				else if (level.HasElevator(col, row, false))
				{
					bool riding = level.elevLeft.GetComponent<Elevator>().IsRiding();
					if (riding)
					{
						aud.PlayOneShot(fall);
						Jump(DIR_UPLEFT);
						yield break;
					}
				}
				int moveDir = 0;
				int randint = Random.Range(0, 2);
				int playerCol = player.GetCol();
				int playerRow = player.GetRow();
				if ((playerCol - col) % 2 == 0 && ((playerCol - col) / 2) + row == playerRow)
				{
					if (playerCol > col)
					{
						if (randint == 0)
						{
							moveDir = DIR_DOWNLEFT;
						}
						else
						{
							moveDir = DIR_DOWNRIGHT;
						}
					}
					else if (playerCol < col)
					{
						if (randint == 0)
						{
							moveDir = DIR_UPLEFT;
						}
						else
						{
							moveDir = DIR_DOWNLEFT;
						}
					}
				}
				else if (playerCol == col)
				{
					if (playerRow > row)
					{
						if (col == 6)
						{
							moveDir = DIR_UPRIGHT;
						}
						else
						{
							if (randint == 0)
							{
								moveDir = DIR_UPRIGHT;
							}
							else
							{
								moveDir = DIR_DOWNRIGHT;
							}
						}
					}
					else if (playerRow < row)
					{
						if (col == 6)
						{
							moveDir = DIR_UPLEFT;
						}
						else
						{
							if (randint == 0)
							{
								moveDir = DIR_UPLEFT;
							}
							else
							{
								moveDir = DIR_DOWNLEFT;
							}
						}
					}
				}
				else if (playerCol > col && playerRow > row)
				{
					moveDir = DIR_DOWNRIGHT;
				}
				else if (playerCol < col && playerRow < row)
				{
					moveDir = DIR_UPLEFT;
				}
				else if (playerCol < col && playerRow >= row)
				{
					moveDir = DIR_UPRIGHT;
				}
				else if (playerCol > col && playerRow <= row)
				{
					moveDir = DIR_DOWNLEFT;
				}
				Jump(moveDir);
			}
			yield return new WaitForSeconds(enemyMoveCooldown);
		}
	}
}
