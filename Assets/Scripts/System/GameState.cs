using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameState
{
	// Player related
	public Vector2 PlayerPos { get; }
	public int PlayerHP { get; }

	// Additional info
	public float RunTime { get; }

	public GameState(Vector2 pPos, int pHP, float time)
	{
		PlayerPos = pPos;
		PlayerHP = pHP;
		RunTime = time;
	}

	public override string ToString()
	{
		return $"{PlayerPos}	{PlayerHP}	{RunTime}";
	}
}
