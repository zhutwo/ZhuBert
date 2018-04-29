using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	[SerializeField] protected Sprite[] spriteSheet = new Sprite[8];
	[SerializeField] protected Level.TileEntity tileEntity;
	[SerializeField] protected float fallSpeed;
	[SerializeField] protected float spawnSpeed;

	public static List<Entity> entityList = new List<Entity>();

	protected Animator anim;
	protected SpriteRenderer sprite;
	protected Level level;
	protected Vector3 fallTarget;
	protected Vector3 fallDirection;
	protected BoxCollider2D collider;
	protected AudioSource aud;

	protected int col;
	protected int row;

	protected int direction;
	public State state;

	protected const int DIR_UPRIGHT = 0;
	protected const int DIR_UPLEFT = 2;
	protected const int DIR_DOWNRIGHT = 4;
	protected const int DIR_DOWNLEFT = 6;

	public enum State
	{
		WAIT,
		JUMP,
		FALL,
		ELEVJUMP,
		ELEVRIDE,
		SPAWN,
		IDLE
	}

	void Start()
	{
		entityList.Add(this);
		var grid = GameObject.FindGameObjectWithTag("LevelGrid");
		level = grid.GetComponent<Level>();
		anim = this.gameObject.GetComponent<Animator>();
		sprite = this.gameObject.GetComponent<SpriteRenderer>();
		aud = this.gameObject.GetComponent<AudioSource>();
	}

	void OnDestroy()
	{
		entityList.Remove(this);
	}

	public void Kill()
	{
		level.RemoveEntityFromTile(this.gameObject, col, row);
	}

	protected void Jump(int dir)
	{
		state = State.JUMP;
		direction = dir;
		sprite.sprite = spriteSheet[direction + 1];
		anim.SetInteger("Direction", direction);
		anim.SetInteger("State", 1);
		level.RemoveEntityFromTile(this.gameObject, col, row);
		aud.Play();
	}

	protected void Fall()
	{
		fallDirection = fallTarget - this.gameObject.transform.position;
		fallDirection.Normalize();
		if (state == State.FALL)
		{
			this.gameObject.transform.Translate(fallDirection * fallSpeed * Time.deltaTime);
		}
		else
		{
			this.gameObject.transform.Translate(fallDirection * spawnSpeed * Time.deltaTime);
		}
	}

	void JumpLand()
	{
		anim.SetInteger("State", 0);
		switch (direction)
		{
		case DIR_UPRIGHT:
			col--;
			break;
		case DIR_UPLEFT:
			col--;
			row--;
			break;
		case DIR_DOWNRIGHT:
			col++;
			row++;
			break;
		case DIR_DOWNLEFT:
			col++;
			break;
		}
		//print(tileEntity.ToString() + ", col: " + col.ToString() + ", row: " + row.ToString());
		if (CliffJumpCheck(col, row))
		{
			FallSound();
			collider.enabled = false;
			state = State.FALL;
			fallTarget = this.gameObject.transform.position;
			fallTarget.y = -32.0f;
			sprite.sortingOrder = 0;
		}
		else
		{
			JumpLandCollisionCheck();
		}
	}

	public int GetCol()
	{
		return col;
	}

	public int GetRow()
	{
		return row;
	}

	public void SetCoords(int c, int r)
	{
		col = c;
		row = r;
	}

	public Level.TileEntity GetEntityType()
	{
		return tileEntity;
	}

	protected virtual void FallSound()
	{
	}

	protected virtual void JumpLandCollisionCheck()
	{
	}

	protected bool CliffJumpCheck(int col, int row)
	{
		if (col < 0 || col > 6 || row < 0 || row > col)
		{
			return true;
		}
		return false;
	}

	public void FreezeAnim(bool freeze)
	{
		anim.enabled = !freeze;
	}
}
