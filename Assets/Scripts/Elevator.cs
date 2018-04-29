using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
	private GameObject player;
	private bool riding;

	private int m_col;
	private int m_row;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update()
	{
		if (riding)
		{
			this.gameObject.transform.position = player.gameObject.transform.position;
		}
	}

	public void SetRiding()
	{
		riding = true;
	}

	public bool IsRiding()
	{
		return riding;
	}

	public void SetTile(int col, int row)
	{
		col = m_col;
		row = m_row;
	}

	public int GetCol()
	{
		return m_col;
	}

	public int GetRow()
	{
		return m_row;
	}
}
