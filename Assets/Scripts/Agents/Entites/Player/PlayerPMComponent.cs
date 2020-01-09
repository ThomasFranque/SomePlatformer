using UnityEngine;
using HutongGames.PlayMaker;

public class PlayerPMComponent : MonoBehaviour
{
	private const string _PLAYER_OBJECT_GLOBAL_NAME		= "Player Object";
	private const string _PLAYER_POSITION_GLOBAL_NAME	= "Player Position";
	private const string _PLAYER_RB_VELOCITY_GLOBAL_NAME= "Rigidbody Velocity";
	private const string _PLAYER_ATTACKING_GLOBAL_NAME	= "Attacking";
	private const string _PLAYER_DASHING_GLOBAL_NAME	= "Dashing";
	private const string _PLAYER_GROUNDED_GLOBAL_NAME	= "Grounded";
	private const string _PLAYER_PUSHING_WALL_GLOBAL_NAME = "Pushing Wall";
	private const string _PLAYER_WALL_SLIDING_GLOBAL_NAME = "Wall Sliding";

	private const string _GAME_DELTA_TIME_NAME = "Delta Time";

	private FsmGameObject _pObject;
	private FsmVector2 _pPositionGlobal;
	private FsmVector2 _pRBVelocityGlobal;
	private FsmBool _pPushingWallGlobal;
	private FsmBool _pWallSlidingGlobal;
	private FsmBool _pAttackingGlobal;
	private FsmBool _pDashingGlobal;
	private FsmBool _pGroundedGlobal;

	private FsmFloat _gDeltaTime;

	private Player _pScript;

    // Start is called before the first frame update
    void Start()
    {
		_pScript = GetComponent<Player>();
		GetGlobals();
    }

    // Update is called once per frame
    void Update()
    {
		UpdateGlobals();
    }

	private void GetGlobals()
	{
		_pPositionGlobal = FsmVariables.GlobalVariables.FindFsmVector2(_PLAYER_POSITION_GLOBAL_NAME);
		_pRBVelocityGlobal = FsmVariables.GlobalVariables.FindFsmVector2(_PLAYER_RB_VELOCITY_GLOBAL_NAME);
		_pPushingWallGlobal = FsmVariables.GlobalVariables.FindFsmBool(_PLAYER_PUSHING_WALL_GLOBAL_NAME);
		_pWallSlidingGlobal = FsmVariables.GlobalVariables.FindFsmBool(_PLAYER_WALL_SLIDING_GLOBAL_NAME);
		_pAttackingGlobal = FsmVariables.GlobalVariables.FindFsmBool(_PLAYER_ATTACKING_GLOBAL_NAME);
		_pDashingGlobal = FsmVariables.GlobalVariables.FindFsmBool(_PLAYER_DASHING_GLOBAL_NAME);
		_pGroundedGlobal = FsmVariables.GlobalVariables.FindFsmBool(_PLAYER_GROUNDED_GLOBAL_NAME);

		_gDeltaTime = FsmVariables.GlobalVariables.FindFsmFloat(_GAME_DELTA_TIME_NAME);

		_pObject = FsmVariables.GlobalVariables.FindFsmGameObject(_PLAYER_OBJECT_GLOBAL_NAME);
		_pObject.Value = gameObject;
	}

	private void UpdateGlobals()
	{
		_pPositionGlobal.Value = transform.position;
		_pRBVelocityGlobal.Value = _pScript.RigidBody2D.velocity;
		_pPushingWallGlobal.Value = _pScript.PushingWall;
		_pWallSlidingGlobal.Value = _pScript.WallSlinding;
		_pAttackingGlobal.Value = _pScript.IsInAttackChain;
		_pDashingGlobal.Value = _pScript.IsDashing;
		_pGroundedGlobal.Value = _pScript.OnGround;

		_gDeltaTime.Value = Time.deltaTime;
	}
}
