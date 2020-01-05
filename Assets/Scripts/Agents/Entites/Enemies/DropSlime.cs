using UnityEngine;

public class DropSlime : Enemy
{
	[Header("SLIME VISION")]
	[SerializeField] private Transform _sightEndLeft;
	[SerializeField] private Transform _sightEndRight;
	[Header("SLIME SETTINGS")]
	[SerializeField] private bool _autoGroundSights = true;
	[SerializeField] private bool _doHangingBehaviour = true;
	[SerializeField] private bool _doGroundBehaviour = true;
	[SerializeField] private bool _startGrounded = false;

	private ParticleSystem _thrustSystem;

	private float _dropSpeed = -95.0f;
	private bool _grounded = false;

	// USE CONSTS WHEN FINISHED
	private Vector3 _bottomColOffset = new Vector2(0, -8.5f);
	private Vector2 _bottomColSize = new Vector2(0, 6.0f);
	private Vector3 _preWarnOffset = new Vector3(4.4f, 0.0f, 0.0f);
	private Vector3 _groundedVisionRange = new Vector3(0.0f, 26.0f, 0.0f);
	private Vector3 _groundPos;

	private bool TriggerAttack
	{
		get
		{
			RaycastHit2D hit =
			Physics2D.Linecast(transform.position, _sightEndLeft.position, LayerMask.GetMask("Player"));
			if (!hit)
				hit =
			Physics2D.Linecast(transform.position, _sightEndRight.position, LayerMask.GetMask("Player"));

			return hit;
		}
	}

	private bool PlayerPreSightedRight
	{
		get
		{
			RaycastHit2D hit =
			Physics2D.Linecast(transform.position, _sightEndRight.position + _preWarnOffset, LayerMask.GetMask("Player"), 23);			

			return hit;
		}
	}

	private bool PlayerPreSightedLeft
	{
		get
		{
			RaycastHit2D hit =
			Physics2D.Linecast(transform.position, _sightEndLeft.position - _preWarnOffset, LayerMask.GetMask("Player"));

			return hit;
		}
	}

	private bool GroundAttackAnimPlaying => _anim.GetCurrentAnimatorStateInfo(0).IsName("GroundAttack");

	// Start is called before the first frame update
	protected override void Start()
    {
		base.Start();
		_thrustSystem = GetComponentInChildren<ParticleSystem>();

		_groundCheckOffset = new Vector2(0, -12f);
		_bottomColSize[0] = (selfCol as CapsuleCollider2D).size[0];
		rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
		_groundPos = GetGroundPosition();

		if (_autoGroundSights)
			GroundVisionObjects();
		if (_startGrounded)
			OnLand();
	}

	// Update is called once per frame
	protected override void Update()
    {
		base.Update();

		// While hanging
		if (!_grounded && _doHangingBehaviour)
		{
			HangingBehaviour();
		}
		else if (_grounded && _doGroundBehaviour)
		{
			GroundedBehaviour();
		}

	}

	private void HangingBehaviour()
	{
		if (transform.position.y + (_groundCheckOffset.y * transform.localScale.x) <= _groundPos.y)
		{
			_grounded = true;
			OnLand();
		}
		else if (TriggerAttack)
		{
			OnDrop();
		}
		else if (PlayerPreSightedLeft || PlayerPreSightedRight)
		{
			WhileSighted();
		}
		else
			WhileTopIdle();
	}

	private void GroundedBehaviour()
	{
		if (TriggerAttack && !GroundAttackAnimPlaying)
			_anim.SetTrigger("Attack");
	}

	public override void KnockBack(bool cameFromRight, float knockSpeed) { }

	private void WhileTopIdle()
	{
		_anim.SetTrigger("Reset");

	}

	private void WhileSighted()
	{
		_anim.SetTrigger("PreWarn");
	}

	private void OnDrop()
	{
		if (rb.velocity.y >= 0)
		{
			rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
			rb.velocity = new Vector2(0, _dropSpeed);
			_thrustSystem.Emit(5);
			_anim.SetTrigger("Attack");
		}
	}

	private void OnLand()
	{
		transform.position = _groundPos - (_groundCheckOffset * transform.localScale.x);
		rb.velocity = new Vector2(0, 0);
		rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
		selfCol.offset = _bottomColOffset;
		(selfCol as CapsuleCollider2D).size = _bottomColSize;

		_sightEndLeft.position = transform.position + _groundedVisionRange;
		_sightEndRight.position = transform.position + _groundedVisionRange;

		_anim.SetTrigger("ReachBottom");
	}

	private Vector3 GetGroundPosition()
	{
		RaycastHit2D hit =
			Physics2D.Raycast(transform.position + _groundCheckOffset, Vector3.down, 600.0f, LayerMask.GetMask("Ground"));

		Vector3 hitPoint = hit.point;
		return hitPoint;
	}

	private void GroundVisionObjects()
	{
		Vector3 tempPos = _sightEndLeft.position;
		tempPos.y = _groundPos.y;
		_sightEndLeft.position = tempPos;
		tempPos.x = _sightEndRight.position.x;
		_sightEndRight.position = tempPos;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, _sightEndLeft.position);
		Gizmos.DrawLine(transform.position, _sightEndRight.position);
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(transform.position, _sightEndRight.position + _preWarnOffset);
		Gizmos.DrawLine(transform.position, _sightEndLeft.position - _preWarnOffset);
	}
}
