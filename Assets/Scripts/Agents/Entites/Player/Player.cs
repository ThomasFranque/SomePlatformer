using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
	public static Player Instance { get; private set; } = null;
	public static Interactible InteractableInRange { get; set; }

	private const float _IGNORE_HAXIS_WALL_JUMP_TIME = 1f;
	private const float _WALL_SLIDE_FALLOFF_FACTOR = 10.5f;
	private const byte _MAX_WALL_JUMP_CONSECUTIVE_JUMPS = 5;

	// variables
	private MeleeWeapon _meleeWeapon;
	private PlayerInventory _inventory;
	private PlayerSound _pSound;

	private Coroutine _ignoreHAxisCor;

	private Action<int> _UIHPUpdate;

	private Vector2 _wallCheckBoxSize = new Vector2(.3f, 3.0f);
	private Vector3 _wallCheckOffset = new Vector2(1.9f, 3.11f);
	private Vector3 _reverseWallCheckOffset = new Vector2(-1.9f, 3.11f);
	//private float _attackChainTime;
	private float _chainAttackTimer = 0.50f;
	private float _dashY = 0;
	private float hAxis, timeOfJump;
	private float timeOfDash;
	private float timeOfAttack;
	private byte _timesWallJumped = 1; // Consecutive times
	private bool jumpPressed, crouchPressed, attackPressed, interactionPressed, dashPressed;
	private bool canMove, canGroundJump, canWallJump, _ignoreHAxis;
	private bool _cantSlideWall;
	private bool _canAirAttack;
	private bool _dashVelocityChange;
	private bool _hitGroundAfterDash;
	private bool _stompJump;

	[Header("Player")]
	[Header("Inputs")]
	[SerializeField] private KeyCode _upInput = KeyCode.W;
	[SerializeField] private KeyCode _downInput = KeyCode.S;
	[SerializeField] private KeyCode _jumpInput = KeyCode.Space;
	[SerializeField] private KeyCode _interactInput = KeyCode.E;
	[SerializeField] private KeyCode _attackInput = KeyCode.L;
	[SerializeField] private KeyCode _dashInput1 = KeyCode.RightShift;
	[SerializeField] private KeyCode _dashInput2 = KeyCode.M;

	[Header("--- Player Properties ---")]
	[SerializeField] private float moveSpeed = 42.0f;
	[Range(0, 10)]
	[SerializeField] private float jumpTime = 0.15f;
	[Range(0, 300)]
	[SerializeField] private float jumpVelocity = 30.0f;
	[SerializeField] private float dashTime = 2.0f;
	[SerializeField] private float dashVelocity = 90.0f;
	[Tooltip("Cool-down additional to dash time.")]
	[SerializeField] private float dashCooldown = 2.0f;
	[SerializeField] private float attackDuration = .2f;
	[Tooltip("Must be higher than attack duration to work")]
	[SerializeField] private float attackChainDuration = .5f;

	[Header("--- Player References ---")]
	[SerializeField] private ParticleSystem attackPSystem = null;

	[Header("--- Dev Properties ---")]
	[SerializeField] private bool _readInputs = true;
	private bool _interactionReadInputs = true;

	public 	PlayerInventory Inventory => _inventory;
	private float GetElapsedTime(float timeToElapse) => Time.time - timeToElapse;
	// Properties
	public Rigidbody2D RigidBody2D => _rb;
	private bool HAxisFullyPressed => !(hAxis > -0.99f && hAxis < 0.99f);
	private bool ReadInput => _readInputs && !KnockedBack && _interactionReadInputs;
	public	bool PushingWall => OnGround && OnWall && hAxis != 0;
	public	bool WallSlinding => OnWall && !OnGround && _rb.velocity.y <= 0.0f;
	public  bool IsDashing => (GetElapsedTime(timeOfDash)) < dashTime;
	private bool CanSlideOnWall => OnWall && canWallJump && !_cantSlideWall && !KnockedBack;
	private bool GroundJumpAllowed => ((OnGround && !OnWall) || (OnGround && OnWall)) && canGroundJump;
	private bool WallJumpAllowed => WallSlinding && canWallJump && !_cantSlideWall;
	private bool IsJumping => (GetElapsedTime(timeOfJump)) < jumpTime && _rb.velocity[1] > 0 || _stompJump;
	private bool DashOnCooldown => (GetElapsedTime(timeOfDash)) < dashCooldown + dashTime;
	private bool CanDash => !GroundAttackAnimationPlaying && !DashOnCooldown && _hitGroundAfterDash;
	public bool GroundAttackAnimationPlaying
	{
		get =>	(_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1")) ||
				(_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2")) ||
				(_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3")) ||
				_meleeWeapon.OnCooldown;
	}
	public bool OnWall => Physics2D.OverlapBox(
				transform.position + (transform.rotation == Quaternion.identity ? _wallCheckOffset : _reverseWallCheckOffset),
				_wallCheckBoxSize,
				0,
				LayerMask.GetMask("Ground")) != null;
	public bool AirAttackAnimationPlaying => _anim.GetCurrentAnimatorStateInfo(0).IsName("AirAttack");
	public bool IsInAttackChain => (GetElapsedTime(timeOfAttack)) < attackChainDuration;
	private bool CanGroundAttack => !_meleeWeapon.OnCooldown && (GetElapsedTime(timeOfAttack)) > attackDuration && !OnWall;


	// Called before Start
	private void Awake()
	{
		InteractableInRange = null;
		Instance = this;
		_inventory = new PlayerInventory();
	}
	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();

		_anim = GetComponent<Animator>();
		_meleeWeapon = GetComponentInChildren<MeleeWeapon>();
		_pSound = GetComponent<PlayerSound>();

		canMove = true;
		canGroundJump = true;
		canWallJump = true;
		_cantSlideWall = false;
		_ignoreHAxis = false;
		_ignoreHAxisCor = null;
		timeOfJump = -1500.0f;
		timeOfDash = -1500.0f;

		_groundCheckBoxSize = new Vector2(1.5f, .25f);
	}
	// Called when physics update
	private void FixedUpdate()
	{
		UpdateRBVelocity();
	}
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		UpdateInputs();
		DoPlayerActions();
		CapMaxYVelocity();
		UpdateAnimator();
		UpdateDashY();
		UpdateWalkSound();

		// TEMP DEATH PROCEDURE //
		if (HP <= 0 && !Dead)
			OnDeath();
	}

	// Custom 
	private void InteractWithinRange()
	{
		InteractableInRange?.Interact(this);
	}
	private void UpdateRBVelocity()
	{
		//! //////////////////////////////////////////
		//! REFACTURE THIS METHOD, LIKE, URGENTLY ////
		//! //////////////////////////////////////////
		
		Vector2 currentVelocity = _rb.velocity;

		if (!IsDashing)
		{

			// ONLY ACTIVATES WHEN AFTER DASH ENDS
			if (_dashVelocityChange)
				currentVelocity = OnDashEnd(_stompJump || KnockedBack);
			// PROCEEDS
			else if (!IsInAttackChain && !KnockedBack && hAxis != 0 && !AirAttackAnimationPlaying && !dashPressed)
			{
				if (!_ignoreHAxis)
					currentVelocity = new Vector2(hAxis * moveSpeed, currentVelocity.y);
				else
				{
					float newXMoveSpeed = currentVelocity.x + hAxis * moveSpeed / 5;
					if (newXMoveSpeed > 40.0f)
						newXMoveSpeed = 40.0f;
					else if (newXMoveSpeed < -40.0f)
						newXMoveSpeed = -40.0f;

					currentVelocity = new Vector2(newXMoveSpeed, currentVelocity.y);
				}
			}

			// Movement procedures // TO BE ADDED TO THEIR OWN METHOD
			//Wall Jump
			// Ground Jump procedure
			if (jumpPressed)
			{
				// Jump intention
				if (GroundJumpAllowed)
				{
					_rb.gravityScale = 10.0f;
					currentVelocity.y = jumpVelocity;
					timeOfJump = Time.time;
					canGroundJump = false;
					canWallJump = false;
					_pSound?.PlayJumpSound();
				}
				// Wall jump
				else if (WallJumpAllowed)
				{
					_rb.gravityScale = 10.0f;
					currentVelocity.y = jumpVelocity / 1.3f;
					timeOfJump = Time.time + jumpTime / 2;
					canWallJump = false;
					canGroundJump = false;

					bool turnedRight = ForceRotate();

					currentVelocity.x = turnedRight ? jumpVelocity / 2.0f : -jumpVelocity / 2.5f;

					_ignoreHAxisCor = StartCoroutine(CIgnoreHAxisFor(_IGNORE_HAXIS_WALL_JUMP_TIME));

					_timesWallJumped++;

					_pSound?.PlayWallJumpSound();
				}
				// Rising and pressing space
				else if (IsJumping)
				{
					_rb.gravityScale = 20f;
				}
				// Falling with space pressed
				else
				{
					if (!OnGround && currentVelocity[1] > 0)
						currentVelocity[1] -= 4;
					_rb.gravityScale = 30.0f;
				}
			}
			// Jump not pressed
			else
			{
				timeOfJump = -500.0f;
				if (_ignoreHAxisCor != null)
				{
					_ignoreHAxis = false;
					StopCoroutine(_ignoreHAxisCor);
					_ignoreHAxisCor = null;
				}

				// Slide on wall
				if (CanSlideOnWall)
				{
					if (_timesWallJumped >= _MAX_WALL_JUMP_CONSECUTIVE_JUMPS)
						_cantSlideWall = true;

					currentVelocity[0] = 0;
					currentVelocity[1] = -1 * (_WALL_SLIDE_FALLOFF_FACTOR * _timesWallJumped);

					_rb.gravityScale = 0.0f;
				}
				else
				{
					_rb.gravityScale = 60.0f;
				}

				// MAKE METHOD ONLAND() IN THE FUTURE
				if (OnGround)
				{
					_timesWallJumped = 1;
					_cantSlideWall = false;
					_canAirAttack = true;
					_hitGroundAfterDash = true;
					_stompJump = false;
					_rb.gravityScale = 60.0f; 
				}

				// else if (!OnGround && !OnWall)
				// {
				// 	if (Input.GetKeyDown(_downInput))
				// 	{
				// 		currentVelocity[0] = 0;
				// 		currentVelocity[1] = -150;
				// 		Debug.Log("Ground Pound");
				// 	}
				// }

				if (OnWall) _hitGroundAfterDash = true;
			}

			if (WallSlinding || !canMove)
				currentVelocity[0] = 0;
		}
		// Is dashing
		else
		{
			if (OnWall) StopDash();

			currentVelocity[0] = transform.rotation == Quaternion.identity ? dashVelocity : -dashVelocity;
		}

		SetVelocity(currentVelocity);
	}
	private void DoPlayerActions()
	{
		// Attack
		if (attackPressed)
			Attack();

		UpdateChainTime();

		// Prevent continuous ground jumping
		if (!jumpPressed && !canGroundJump && OnGround)
			canGroundJump = true;
		if (!jumpPressed && !canWallJump && (OnWall || OnGround))
			canWallJump = true;

		// Interaction
		if (interactionPressed && !IsDashing)
			InteractWithinRange();
		// Dash
		else if (dashPressed && CanDash)
		{
			if (WallSlinding) ForceRotate();
			OnDash();
		}

		// Rotation
		if (WallSlinding && HAxisFullyPressed && !IsDashing)
			UpdateRotation();
		else if (!IsInAttackChain && !WallSlinding && !IsDashing)
			UpdateRotation();
	}
	private void UpdateAnimator()
	{
		_anim.SetFloat("hAxis", hAxis);
		_anim.SetFloat("yVeloc", _rb.velocity.y);
		_anim.SetBool("grounded", OnGround);
		_anim.SetBool("canMove", canMove);
		_anim.SetBool("crouched", crouchPressed);
		_anim.SetBool("wallGrab", WallSlinding && CanSlideOnWall);
		_anim.SetBool("pushingWall", PushingWall);
		_anim.SetBool("AttackChainEnd", !IsInAttackChain);
		_anim.SetBool("Dashing", IsDashing);

		if (hAxis >= 0.1f || hAxis <= -0.1f)
			_anim.speed = hAxis > 0 ? 1 * hAxis : 1 * -hAxis;
		else
			_anim.speed = 1;
	}
	private void UpdateInputs()
	{
		// Get inputs
		if (ReadInput)
		{
			hAxis = Input.GetAxis("Horizontal");
			jumpPressed = Input.GetKey(_jumpInput) || Input.GetKey(_upInput);
			crouchPressed = Input.GetKey(_downInput) && !jumpPressed && OnGround;
			interactionPressed = Input.GetKeyDown(_interactInput);
			canMove = !(crouchPressed);

			attackPressed = Input.GetKeyDown(_attackInput) && canMove;
			dashPressed = Input.GetKeyDown(_dashInput1) || Input.GetKeyDown(_dashInput2);
		}
	}
	private void UpdateChainTime()
	{
		// Chain attack countdown
		//if (_attackChainTime > 0)
		//{
		//	_attackChainTime -= Time.deltaTime;
		//	_anim.SetBool("AttackChainEnd", false);
		//}
		//else
		//	_anim.SetBool("AttackChainEnd", true);
	}

	private void UpdateWalkSound()
	{
		if (hAxis != 0 && OnGround && !OnWall && !IsDashing)
			_pSound?.WhileWalkInputPressed();
		else 
			_pSound?.WhileNotWalkInputPressed();
	}

	private void Attack()
	{
		if (CanGroundAttack)
		{
			timeOfAttack = Time.time;
			Vector2 currentVelocity = _rb.velocity;

			if (OnGround)
			{
					_anim.SetTrigger("Attack");

					_meleeWeapon.GroundAttack(currentVelocity, attackPSystem);
			}
			else if (_canAirAttack)
			{
					_anim.SetTrigger("AirAttack");
					_canAirAttack = false;
					_meleeWeapon.AirAttack(currentVelocity, attackPSystem);
			}
		}
	}
	private void UpdateRotation()
	{
		// Rotate object
		if ((hAxis < 0.0f) && (transform.right.x > 0.0f))
		{
			transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
		}
		else if ((hAxis > 0.0f) && (transform.right.x < 0.0f))
		{
			transform.rotation = Quaternion.identity;
		}
	}
	private bool ForceRotate() // Returns true if new rotation is turned right
	{
		// Rotate object
		if (transform.rotation == Quaternion.identity)
		{
			transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
			return false;
		}

		transform.rotation = Quaternion.identity;
		return true;
	}
	private void OnDash()
	{
		_dashVelocityChange = true;
		_hitGroundAfterDash = false;
		_dashY = transform.position.y;
		timeOfDash = Time.time;

		_pSound.PlayDashSound();
	}
	private Vector2 OnDashEnd(bool suppress)
	{
		_dashVelocityChange = false;
		if (!suppress)
			return new Vector2(transform.rotation == Quaternion.identity ? dashVelocity / 2 : -dashVelocity / 2, -10);
		else
			return _rb.velocity;
	}
	private void StopDash()
	{
		timeOfDash -= dashTime;
	}
	private void UpdateDashY()
	{
		if (IsDashing)
			transform.position = new Vector3(transform.position.x, _dashY, transform.position.z);
	}
	public void AddUIHPListener(Action<int> hpUpdate)
	{
		_UIHPUpdate += hpUpdate;
	}
	public void Heal(byte amount)
	{
		if (HP < 3)
			HP += amount;
	}
	public void DoStomp(float newYSpeed)
	{
		if (IsDashing)
			StopDash();
		_stompJump = true;
		SetVelocity(new Vector2(_rb.velocity.x, newYSpeed));
	} 
	public void SetVelocity(Vector2 newVelocity)
	{
		_rb.velocity = newVelocity;
	}
	public void SetInputReading(bool active)
	{
		_readInputs = active;

		if (!active)
		{
			hAxis = 0.0f;
			jumpPressed = false;
			attackPressed = false;
			crouchPressed = false;
			interactionPressed = false;
			canMove = true;
		}
	}

	public void SetInteractionInputReading(bool active)
	{
		_interactionReadInputs = active;

		if (!active)
		{
			hAxis = 0.0f;
			jumpPressed = false;
			attackPressed = false;
			crouchPressed = false;
			interactionPressed = false;
			canMove = true;
		}
	}

	public override void KnockBack(bool cameFromRight, float knockSpeed)
	{
		if (_invulnerable) return;

		base.KnockBack(cameFromRight, knockSpeed);
	}
	protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
	{
		if (!_invulnerable)
		{
			base.OnHit(cameFromRight, knockSpeed, dmg);

			CameraActions.ActiveCamera.Shake(40 * dmg, 30 * dmg);
			_UIHPUpdate?.Invoke(HP);

			if (IsDashing)
				StopDash();

			if (HP <= 0 && !Dead)
			{
				OnDeath();
			}
			else
			{
				deathParticle.Emit(UnityEngine.Random.Range(15, 25));
				SetInvulnerability(true);
				StartCoroutine(CIgnoreInputFor(0.8f));
			}
		}
	}
	protected override void OnDeath(byte dmg = 1)
	{
		deathParticle.Emit(UnityEngine.Random.Range(95, 105));
		base.OnDeath();
	}

	// Enumeratorz
	private IEnumerator CIgnoreHAxisFor(float seconds)
	{
		_ignoreHAxis = true;
		yield return new WaitForSeconds(seconds);
		_ignoreHAxis = false;
	}
	private IEnumerator CIgnoreInputFor(float seconds)
	{
		SetInputReading(false);
		yield return new WaitForSeconds(seconds);
		SetInputReading(true);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(transform.position, new Vector3(_groundCheckBoxSize.x, _groundCheckBoxSize.y, 1));
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(transform.position + _wallCheckOffset, new Vector3(_wallCheckBoxSize.x, _wallCheckBoxSize.y, 1));
	}
}