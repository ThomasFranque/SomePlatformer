using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class Boss1PMActions : Enemy
{
	// Use raycasts later to get room size
	private const float _ROOM_SPACE_TOTAL = 170;

	[Header("Boss References")]
	[SerializeField] private Transform _bossRoomCenter;
	[Header("Boss Properties")]
	[UnityEngine.Tooltip("Boss needs to have this health or lower to be on stage 2")]
	[SerializeField] private uint _stage2RequiredHp = 20;
	[UnityEngine.Tooltip("Boss needs to have this health or lower to be on stage 3")]
	[SerializeField] private uint _stage3RequiredHp = 10;

	private PlayMakerFSM _bossFsm;
	private FsmBool _fsmVulnerable;
	private FsmBool _fsmPOnRightside;
	private FsmBool _fsmpHasRoomSpace;
	private FsmInt _fsmStage;

	private bool IsPlayerOnRightSide => transform.position.x - Player.Instance.transform.position.x < 0;

	private bool HasRoomSpace => transform.position.x < _bossRoomCenter.position.x + (_ROOM_SPACE_TOTAL / 2) && transform.position.x > _bossRoomCenter.position.x - (_ROOM_SPACE_TOTAL / 2);

	private bool SecondStageReached => HP < _stage2RequiredHp;
	private bool ThirdStageReached => HP < _stage3RequiredHp;

	private uint CurrentStage
	{
		get
		{
			if (ThirdStageReached) return 3;
			if (SecondStageReached) return 2;
			return 1;
		}
	}

	protected override void Start()
	{
		base.Start();
		_bossFsm = GetComponent<PlayMakerFSM>();
		_fsmVulnerable = _bossFsm.FsmVariables.GetFsmBool("Vulnerable");
		_fsmPOnRightside = _bossFsm.FsmVariables.GetFsmBool("Player on Rightside");
		_fsmpHasRoomSpace = _bossFsm.FsmVariables.GetFsmBool("Has Room Space");
		_fsmStage = _bossFsm.FsmVariables.GetFsmInt("Stage");
		_fsmStage.Value = (int)CurrentStage;
	}

	protected override void Update()
	{
		base.Update();
		_canBeStomped = _fsmVulnerable.Value;
		_fsmPOnRightside.Value = IsPlayerOnRightSide;
		_fsmpHasRoomSpace.Value = HasRoomSpace;
	}

	protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
	{
		base.OnHit(cameFromRight, knockSpeed, dmg);
		UpdateFsmBossStage();
	}

	public override void KnockBack(bool cameFromRight, float knockSpeed) {}

	private void UpdateFsmBossStage()
	{
		_fsmStage.Value = (int)CurrentStage;
	}
}
