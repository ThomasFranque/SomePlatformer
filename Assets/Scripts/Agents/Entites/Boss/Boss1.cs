using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : Enemy
{
	private Player _pScript => Player.Instance;
	private Vector3 _pPos => Player.Instance.transform.position;

	private Coroutine _currentAttack = null;
	private Vector3 _targetPosition;
	private float _yPositionLock;

	#region Air Plunge attack
	[Header("Air Plunge Properties")]
	[SerializeField] private float _airPlungeUpVelocity = 42.0f;
	[SerializeField] private float _airPlungeHeight = 24.0f;
	[SerializeField] private float _airPlungeInAirWait = 2.0f;
	[SerializeField] private float _airPlungeBeforeDropWait = .5f;
	[SerializeField] private float _airPlungeMaxWaitForGetNear = 5.0f;
	[SerializeField] private float _airPlungeDropSpeed= 18.0f;
	private Vector3 _airPlungeStoredPosition = default;
	private bool _airPlungeSoftMoveToTargetPos = false;
	private bool _airPlungeSoftMoveToPlayerX = false;
	private bool _airPlungeLockYPosition = false;
	#endregion

	private bool SoftMoveToTargetPosition => _airPlungeSoftMoveToTargetPos;
	private bool SoftMoveToPlayerX => _airPlungeSoftMoveToPlayerX;
	private bool LockYPosition => _airPlungeLockYPosition;

	// Start is called before the first frame update
	protected override void Start()
    {
		base.Start();

		PlungeAttack();
    }

	// Update is called once per frame
	protected override void Update()
    {
		base.Update();

		// AIR PLUNGE
		if (SoftMoveToTargetPosition)
			SmoothMoveTowardsTarget(2);
		if (SoftMoveToPlayerX)
			SmoothMoveTowardsTarget(3, _pPos, _colliderTouchingSolidGround ? (float?)transform.position.x : null ,_yPositionLock);
		if (LockYPosition)
			LockYPos();
		//

		Debug.Log(_colliderTouchingSolidGround);
	}

	private void PlungeAttack()
	{
		_currentAttack =  StartCoroutine(CPlungeAttack());
	}

	private void SetRBVelocity(float x, float y)
	{
		rb.velocity = new Vector2(x, y);
	}

	private void SmoothMoveTowardsTarget(float speed, Vector3 target = default, float? xLock = null, float? yLock = null)
	{
		if (target == default) target = _targetPosition;

		// Preparing variable for Lerp
		Vector3 _desiredPosition = new Vector3(
			target.x,
			target.y,
			transform.position.z);


		// Using Lerp to pan the camera 
		Vector3 smoothPosition =
			Vector3.Lerp(transform.position, _desiredPosition,
			speed * Time.deltaTime);

		if (xLock != null)
			smoothPosition.x = (float)xLock;
		if (yLock != null)
			smoothPosition.y = (float)yLock;


		transform.position = smoothPosition;
	}

	private void LockYPos()
	{
		transform.position = new Vector3(transform.position.x, _yPositionLock, transform.position.z);
	}

	private IEnumerator CPlungeAttack()
	{
		_airPlungeStoredPosition = transform.position;
		SetRBVelocity(0, _airPlungeUpVelocity);
		_useColliderAsTrigger = true;
		selfCol.isTrigger = true;
		// Jump UP
		yield return WaitForAirPlungeReachHeight();

		// Get player pos into _targetpos
		_targetPosition = new Vector3(_pPos.x, transform.position.y);
		_yPositionLock = transform.position.y;
		_airPlungeLockYPosition = true;

		// Wait before moving
		yield return new WaitForSeconds(_airPlungeInAirWait);

		// Move to gotten pos (set bool to true)
		rb.gravityScale = 0;
		_airPlungeLockYPosition = false;
		_airPlungeSoftMoveToPlayerX = true;


		_useColliderAsTrigger = false;
		selfCol.isTrigger = false;

		// Move towards player 
		yield return WaitForAirPlungeAirTimeEnd();

		_useColliderAsTrigger = true;
		selfCol.isTrigger = true;

		// Wait before drop
		yield return new WaitForSeconds(_airPlungeBeforeDropWait);
		_airPlungeSoftMoveToPlayerX = false;
		rb.gravityScale = 8;

		SetRBVelocity(0, -_airPlungeDropSpeed);

		yield return WaitForTouchGround();

		SetRBVelocity(0, 0);
		_yPositionLock = transform.position.y;
		_airPlungeLockYPosition = true;
		_useColliderAsTrigger = false;
		selfCol.isTrigger = false;

		yield return new WaitForSeconds(2.0f);

		_airPlungeLockYPosition = false;
		_currentAttack = null;

		PlungeAttack();
	}

	private Vector3 GetGroundPosition()
	{
		RaycastHit2D hit =
			Physics2D.Raycast(transform.position + _groundCheckOffset, Vector3.down, 600.0f, LayerMask.GetMask("Ground"));

		Vector3 hitPoint = hit.point;
		return hitPoint;
	}

	private IEnumerator WaitForAirPlungeReachHeight()
	{
		while (transform.position.y < _airPlungeStoredPosition.y + _airPlungeHeight) yield return null;
	}
	private IEnumerator WaitForAirPlungeAirTimeEnd()
	{
		float timeOfWaitStart = Time.time;
		while (transform.position.x - _pPos.x > 0.5f || -0.5 > transform.position.x - _pPos.x || 
			timeOfWaitStart - Time.time > _airPlungeMaxWaitForGetNear) yield return null;
	}
	private IEnumerator WaitForTouchGround()
	{
		while (!OnGround) yield return null;
	}

	protected override void OnTriggerEnterGroundCollision()
	{
		base.OnTriggerEnterGroundCollision();

		SetRBVelocity(0, 0);
		_useColliderAsTrigger = false;
		selfCol.isTrigger = false;
		Debug.Log("here");
	}
}
