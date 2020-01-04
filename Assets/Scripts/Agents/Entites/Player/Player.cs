using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
	public static Player Instance = null;

	public static Interactible InteractableInRange { get; set; }

	private const float _IGNORE_HAXIS_WALL_JUMP_TIME = 1f;
	private const float _WALL_SLIDE_FALLOFF_FACTOR = 10.5f;
	private const byte _MAX_WALL_JUMP_CONSECUTIVE_JUMPS = 5;

	[Header("Player")]

	[Header("Inputs")]
	[SerializeField] private KeyCode _upInput = default;
	[SerializeField] private KeyCode _downInput = default;
	[SerializeField] private KeyCode _jumpInput = default;
	[SerializeField] private KeyCode _interactInput = default;
	[SerializeField] private KeyCode _attackInput = default;

	// variables
	private MeleeWeapon _meleeWeapon;

	private bool jumpPressed, crouchPressed, attackPressed, interactionPressed;
	private bool canMove, canGroundJump, canWallJump, _ignoreHAxis;
	private bool _cantSlideWall;
	private float _attackChainTime;
	private float _chainAttackTimer = 0.50f;

	private byte _timesWallJumped = 1; // Consecutive times

	private float hAxis, vAxis, timeOfJump, initialJumpVelocity;

	private Vector2 _wallCheckBoxSize = new Vector2(.3f, 3.0f);
	private Vector3 _wallCheckOffset = new Vector2(1.9f, 3.11f);
	private Vector3 _reverseWallCheckOffset = new Vector2(-1.9f, 3.11f);

	private Coroutine _ignoreHAxisCor;

	[Header("--- Player Properties ---")]
	// UI
	//[SerializeField] private Animator powerUpIndicatorAnim;
	// Self
	[SerializeField] private float moveSpeed;
	[Range(0, 10)]
	[SerializeField] private float jumpTime = 0.15f;
	[Range(0, 300)]
	[SerializeField] private float jumpVelocity = 30.0f;
	[SerializeField] private ParticleSystem attackPSystem = null;

	[Header("--- Dev Properties ---")]
	[SerializeField] private bool _readInputs = true;
	// Properties
	public bool Dead { get; private set; }
	public float MoveSpeed { get => moveSpeed; set { moveSpeed = value; } }

	public bool PushingWall { get => OnGround && OnWall && hAxis != 0; }

	private void InteractWithinRange()
	{
		InteractableInRange?.Interact(this);
	}

	public bool OnWall
	{
		get
		{
			Collider2D collider = Physics2D.OverlapBox(
				transform.position + (transform.rotation == Quaternion.identity ? _wallCheckOffset : _reverseWallCheckOffset),
				_wallCheckBoxSize,
				0,
				LayerMask.GetMask("Ground"));

			if (collider != null) return true;

			return false;
		}
	}
	public bool GroundAttackAnimationPlaying
	{
		get
		{
			return
				(_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1")) ||
				(_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2")) ||
				(_anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3")) ||
				_meleeWeapon.OnCooldown;
		}
	}
 
	public bool AirAttackAnimationPlaying
	{
		get
		{
			return
				(_anim.GetCurrentAnimatorStateInfo(0).IsName("AirAttack"));
		}
	}

	private void Awake()
	{
		InteractableInRange = null;
		Instance = this;
	}

	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();

		_anim = GetComponent<Animator>();
		_meleeWeapon = GetComponentInChildren<MeleeWeapon>();

		canMove = true;
		canGroundJump = true;
		canWallJump = true;
		_cantSlideWall = false;
		Dead = false;
		timeOfJump = -1500.0f;
		initialJumpVelocity = jumpVelocity;

		_ignoreHAxis = false;
		_ignoreHAxisCor = null;

		_groundCheckBoxSize = new Vector2(3.5f, .25f);
	}

	private void FixedUpdate()
	{
		UpdateRBVelocity();
	}

	// Update is called once per frame
	protected override void Update()
	{
		// Get inputs
		if (_readInputs && !KnockedBack)
		{
			hAxis = Input.GetAxis("Horizontal");
			jumpPressed = Input.GetKey(_jumpInput);
			crouchPressed = Input.GetKey(_downInput) && !jumpPressed && OnGround;
			interactionPressed = Input.GetKeyDown(_interactInput);
			canMove = !(crouchPressed);

			attackPressed = Input.GetKeyDown(_attackInput) && canMove;
		}

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
		if (interactionPressed)
			InteractWithinRange();

		UpdateRotation();

		if (jumpVelocity == initialJumpVelocity)
			CapMaxYVelocity();

		base.Update();

		// ANIMATIOR
		_anim.SetFloat("hAxis", hAxis);
		_anim.SetFloat("yVeloc", rb.velocity.y);
		_anim.SetBool("grounded", OnGround);
		_anim.SetBool("canMove", canMove);
		_anim.SetBool("crouched", crouchPressed);
		_anim.SetBool("wallGrab", OnWall && !OnGround && !_cantSlideWall && canWallJump && !KnockedBack);
		_anim.SetBool("pushingWall", PushingWall);

		if (hAxis >= 0.1f || hAxis <= -0.1f)
			_anim.speed = hAxis > 0 ? 1 * hAxis : 1 * -hAxis;
		else
			_anim.speed = 1;

		// TEMP DEATH PROCEDURE //
		if (HP <= 0 && !Dead)
			OnDeath();
	}

	private void UpdateRBVelocity()
	{
		Vector2 currentVelocity = rb.velocity;

		if (!GroundAttackAnimationPlaying && !KnockedBack && hAxis != 0 && !AirAttackAnimationPlaying)
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
			if (((OnGround && !OnWall) || (OnGround && OnWall)) && canGroundJump)
			{
				rb.gravityScale = 10.0f;
				currentVelocity.y = jumpVelocity;
				timeOfJump = Time.time;
				canGroundJump = false;
				canWallJump = false;
			}
			// Wall jump
			else if (!OnGround && OnWall && canWallJump && !_cantSlideWall)
			{
				rb.gravityScale = 10.0f;
				currentVelocity.y = jumpVelocity / 1.3f;
				timeOfJump = Time.time + jumpTime / 2;
				canWallJump = false;
				canGroundJump = false;

				bool turnedRight = ForceRotate();

				currentVelocity.x = turnedRight ? jumpVelocity / 2.5f : -jumpVelocity / 2.5f;

				_ignoreHAxisCor = StartCoroutine(CIgnoreHAxisFor(_IGNORE_HAXIS_WALL_JUMP_TIME));

				_timesWallJumped++;
			}
			// Rising and pressing space
			else if ((Time.time - timeOfJump) < jumpTime && rb.velocity[1] > 0)
			{
				rb.gravityScale = 20f;
			}
			// Falling space pressed
			else
			{
				if (!OnGround && currentVelocity[1] > 0)
					currentVelocity[1] -= 4;
				rb.gravityScale = 30.0f;
			}
		}
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
			if (OnWall && canWallJump && !_cantSlideWall && !KnockedBack)
			{
				currentVelocity[0] = 0;
				currentVelocity[1] = -1 * (_WALL_SLIDE_FALLOFF_FACTOR * _timesWallJumped);

				if (_timesWallJumped >= _MAX_WALL_JUMP_CONSECUTIVE_JUMPS)
				{
					currentVelocity[1] = -1 * (_WALL_SLIDE_FALLOFF_FACTOR * _timesWallJumped + 1);
					_cantSlideWall = true;
				}

				rb.gravityScale = 0.0f;
			}
			else
			{
				rb.gravityScale = 60.0f;
			}

			// MAKE METHOD ONLAND() IN THE FUTURE
			if (OnGround)
			{
				_timesWallJumped = 1;
				_cantSlideWall = false;
			}
		}

		if (OnWall && !OnGround)
		{
			currentVelocity[0] = 0;
		}

		if (!canMove)
			currentVelocity[0] = 0;

		SetVelocity(currentVelocity);
	}

	private void UpdateChainTime()
	{
		// Chain attack countdown
		if (_attackChainTime > 0)
		{
			_attackChainTime -= Time.deltaTime;
			_anim.SetBool("AttackChainEnd", false);
		}
		else
			_anim.SetBool("AttackChainEnd", true);
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

	// Returns true if new rotation is turned right
	private bool ForceRotate()
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

	public void SetVelocity(Vector2 newVelocity)
	{
		rb.velocity = newVelocity;
	}

	public void Heal(byte ammount)
	{
		if (HP < 3)
			HP += ammount;
	}

	private void Attack()
	{
		if (!GroundAttackAnimationPlaying)
		{
			Vector2 currentVelocity = rb.velocity;

			if (OnGround)
			{
				// Check attack intention
				if (!_meleeWeapon.OnCooldown)
				{
					_attackChainTime = _chainAttackTimer;

					_anim.SetTrigger("Attack");

					_meleeWeapon.GroundAttack(currentVelocity, attackPSystem);
				}

			}
			else
			{
				if (!_meleeWeapon.OnCooldown)
				{
					_anim.SetTrigger("AirAttack");

					_meleeWeapon.AirAttack(currentVelocity, attackPSystem);
				}
			}
		}
	}

	protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
	{
		if (!invulnerable)
		{
			base.OnHit(cameFromRight, knockSpeed, dmg);

			CameraActions.ActiveCamera.Shake(40 * dmg, 30 * dmg);

			if (HP <= 0 && !Dead)
			{
				OnDeath();
			}
			else
			{
				deathParticle.Emit(Random.Range(15, 25));
				SetInvunerability(true);
				StartCoroutine(CIgnoreInputFor(0.8f));
			}
		}
	}

	public override void KnockBack(bool cameFromRight, float knockSpeed)
	{
		if (invulnerable) return;

		base.KnockBack(cameFromRight, knockSpeed);
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

	private IEnumerator CIgnoreHAxisFor(float seconds)
	{
		_ignoreHAxis = true;
		yield return new WaitForSeconds(seconds);
		_ignoreHAxis = false;
	}

	public IEnumerator CIgnoreInputFor(float seconds)
	{
		SetInputReading(false);
		yield return new WaitForSeconds(seconds);
		SetInputReading(true);
	}

	protected override void OnDeath()
	{
		deathParticle.Emit(Random.Range(95, 105));
		base.OnDeath();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(transform.position, new Vector3(_groundCheckBoxSize.x, _groundCheckBoxSize.y, 1));
		Gizmos.DrawCube(transform.position, new Vector3(_groundCheckBoxSize.x, _groundCheckBoxSize.y, 1));
	}
}