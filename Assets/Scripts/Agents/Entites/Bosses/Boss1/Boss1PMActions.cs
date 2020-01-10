//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using HutongGames.PlayMaker;

//public class Boss1PMActions : Enemy
//{
//	// Use raycasts later to get room size
//	private const float _JUMP_BACK_ROOM_SPACE_TOTAL = 120;

//	[Header("Boss References")]
//	[SerializeField] private Transform _bossRoomCenter = null;
//	[SerializeField] private Transform _upRightBounceTransform = null;
//	[SerializeField] private Transform _downRightBounceTransform = null;
//	[SerializeField] private Transform _upLeftBounceTransform = null;
//	[SerializeField] private Transform _downLeftBounceTransform = null;
//	[Header("Boss Properties")]
//	[UnityEngine.Tooltip("Boss needs to have this health or lower to be on stage 2")]
//	[SerializeField] private uint _stage2RequiredHp = 20;
//	[UnityEngine.Tooltip("Boss needs to have this health or lower to be on stage 3")]
//	[SerializeField] private uint _stage3RequiredHp = 10;

//	// Boss
//	private PlayMakerFSM _bossFsm;
//	private FsmBool _fsmVulnerable;
//	private FsmBool _fsmPOnRightside;

//	private Vector2 _wallCheckBoxSize = new Vector2(2.6f, 8.0f);
//	private Vector3 _wallCheckRightOffset = new Vector2(12f, 16.11f);
//	private Vector3 _wallCheckLeftOffset = new Vector2(-12f, 16.11f);
//	private Vector3 _wallCheckUpOffset = new Vector2(0, 32f);
//	private Vector3 _reverseWallCheckOffset = new Vector2(-1.9f, 3.11f);
//	private bool OnRightWall => Physics2D.OverlapBox(
//			transform.position + (_wallCheckRightOffset),
//			_wallCheckBoxSize,
//			0,
//			LayerMask.GetMask("Ground")) != null;
//	private bool OnLeftWall => Physics2D.OverlapBox(
//			transform.position + (_wallCheckLeftOffset),
//			_wallCheckBoxSize,
//			0,
//			LayerMask.GetMask("Ground")) != null;
//	private bool OnRoof => Physics2D.OverlapBox(
//			transform.position + (_wallCheckUpOffset),
//			_groundCheckBoxSize,
//			0,
//			LayerMask.GetMask("Ground")) != null;

//	// Plunge
//	private FsmBool _fsmpHasRoomJumpSpace;
//	private FsmInt _fsmStage;

//	// Bounce
//	private FsmVector3 _fsmUpRightP;
//	private FsmVector3 _fsmUpLeftP;
//	private FsmVector3 _fsmDownRightP;
//	private FsmVector3 _fsmDownLeftP;
//	private FsmVector2 _fsmBounceDir;
//	private FsmBool _fsmIsBouncing;
//	private FsmGameObject _fsmBounceTarget;
//	private Vector2 _bounceDirection = new Vector2(1, 1);

//	private Vector3 UpRightBouncePos => _upRightBounceTransform.position;
//	private Vector3 UpLeftBouncePos => _upLeftBounceTransform.position;
//	private Vector3 DownRightBouncePos => _downRightBounceTransform.position;
//	private Vector3 DownLeftBouncePos => _downLeftBounceTransform.position;

//	private bool IsPlayerOnRightSide => transform.position.x - Player.Instance.transform.position.x < 0;

//	private bool HasRoomJumpBackSpace => transform.position.x < _bossRoomCenter.position.x + (_JUMP_BACK_ROOM_SPACE_TOTAL / 2) && transform.position.x > _bossRoomCenter.position.x - (_JUMP_BACK_ROOM_SPACE_TOTAL / 2);

//	private bool SecondStageReached => HP < _stage2RequiredHp;
//	private bool ThirdStageReached => HP < _stage3RequiredHp;

//	private uint CurrentStage
//	{
//		get
//		{
//			if (ThirdStageReached) return 3;
//			if (SecondStageReached) return 2;
//			return 1;
//		}
//	}

//	protected override void Start()
//	{
//		base.Start();

//		_groundCheckBoxSize = new Vector2(1.5f, 3);

//		_bossFsm = GetComponent<PlayMakerFSM>();
//		_fsmVulnerable = _bossFsm.FsmVariables.GetFsmBool("Vulnerable");
//		_fsmPOnRightside = _bossFsm.FsmVariables.GetFsmBool("Player on Rightside");
//		_fsmpHasRoomJumpSpace = _bossFsm.FsmVariables.GetFsmBool("Has Room Jump Back Space");
//		_fsmStage = _bossFsm.FsmVariables.GetFsmInt("Boss Stage");
//		_fsmUpRightP = _bossFsm.FsmVariables.GetFsmVector3("Up Right Bounce W Pos");
//		_fsmUpLeftP = _bossFsm.FsmVariables.GetFsmVector3("Up Left Bounce W Pos");
//		_fsmDownRightP = _bossFsm.FsmVariables.GetFsmVector3("Down Right Bounce W Pos");
//		_fsmDownLeftP = _bossFsm.FsmVariables.GetFsmVector3("Down Left Bounce W Pos");
//		_fsmBounceTarget = _bossFsm.FsmVariables.GetFsmGameObject("Bounce Target Object");
//		_fsmBounceDir = _bossFsm.FsmVariables.GetFsmVector2("Bounce Direction");
//		_fsmIsBouncing = _bossFsm.FsmVariables.GetFsmBool("Bouncing");

//		_fsmStage.Value = (int)CurrentStage;
//		_fsmBounceDir.Value = _bounceDirection;
//		_fsmBounceTarget.Value = _upRightBounceTransform.gameObject;
//	}

//	protected override void Update()
//	{
//		base.Update();
//		_canBeStomped = _fsmVulnerable.Value;
//		_fsmPOnRightside.Value = IsPlayerOnRightSide;
//		_fsmpHasRoomJumpSpace.Value = HasRoomJumpBackSpace;

//		if (CurrentStage > 1)
//		{
//			_fsmUpRightP.Value = UpRightBouncePos;
//			_fsmUpLeftP.Value = UpLeftBouncePos;
//			_fsmDownRightP.Value = DownRightBouncePos;
//			_fsmDownLeftP.Value = DownLeftBouncePos;
//		}
//		Debug.Log(OnGround);
//	}

//	private void OnBounceHitWall()
//	{
//		// MAKE METHOD TO DETERMINE WHICH DIRECTION TO GO

//		if (OnRightWall)
//			_bounceDirection.x = -1;
//		else if (OnLeftWall)
//			_bounceDirection.x = 1;
//		if (OnGround)
//			_bounceDirection.y = 1;
//		else if (OnRoof)
//			_bounceDirection.y = -1;


//		if (_bounceDirection.x == 1)
//		{
//			if (_bounceDirection.y == 1)
//				_fsmBounceTarget.Value = _upRightBounceTransform.gameObject;
//			else
//				_fsmBounceTarget.Value = _downRightBounceTransform.gameObject;
//		}
//		else
//		{
//			if (_bounceDirection.y == 1)
//				_fsmBounceTarget.Value = _upLeftBounceTransform.gameObject;
//			else
//				_fsmBounceTarget.Value = _downLeftBounceTransform.gameObject;
//		}

//		Debug.Log(_fsmBounceTarget.Value.name);

//		_fsmBounceDir.Value = _bounceDirection;
//	}

//	protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
//	{
//		base.OnHit(cameFromRight, knockSpeed, dmg);
//		UpdateFsmBossStage();
//	}

//	public override void KnockBack(bool cameFromRight, float knockSpeed) { }

//	private void UpdateFsmBossStage()
//	{
//		_fsmStage.Value = (int)CurrentStage;
//	}

//	protected override void OnPlayerStomp(Player p)
//	{
//		base.OnPlayerStomp(p);

//		if (_fsmIsBouncing.Value)
//		{
//			_fsmIsBouncing.Value = false;
//			PlayMakerFSM.BroadcastEvent("BOSS BOUNCE HIT");
//		}

//	}

//	protected override void OnEnterGroundCollision()
//	{
//		base.OnEnterGroundCollision();
//		//if (CurrentStage == 2 && _fsmIsBouncing.Value)
//		//	OnBounceHitWall();
//		if (_fsmIsBouncing.Value)
//			OnBounceHitWall();
//	}

//	private void OnDrawGizmos()
//	{

//		Gizmos.color = Color.yellow;
//		Gizmos.DrawCube(transform.position, new Vector3(_groundCheckBoxSize.x, _groundCheckBoxSize.y, 1));
//		Gizmos.DrawCube(transform.position + _wallCheckUpOffset, new Vector3(_groundCheckBoxSize.x, _groundCheckBoxSize.y, 1));
//		Gizmos.color = Color.green;

//		Gizmos.DrawCube(transform.position + _wallCheckRightOffset, new Vector3(_wallCheckBoxSize.x, _wallCheckBoxSize.y, 1));
//		Gizmos.color = Color.cyan;
//		Gizmos.DrawCube(transform.position + _wallCheckLeftOffset, new Vector3(_wallCheckBoxSize.x, _wallCheckBoxSize.y, 1));
//	}

//}
