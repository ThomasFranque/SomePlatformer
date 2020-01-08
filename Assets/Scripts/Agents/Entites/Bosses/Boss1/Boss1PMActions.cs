using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class Boss1PMActions : Enemy
{
	private PlayMakerFSM _bossFsm;
	private FsmBool _vulnerable;

	private byte currentStage = 1; // /3

	protected override void Start()
	{
		base.Start();
		_bossFsm = GetComponent<PlayMakerFSM>();
		_vulnerable = _bossFsm.FsmVariables.GetFsmBool("Vulnerable");
	}

	protected override void Update()
	{
		base.Update();
		_canBeStomped = _vulnerable.Value;
	}

	public override void KnockBack(bool cameFromRight, float knockSpeed)
	{
		
	}
}
