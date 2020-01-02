using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
	public static Player Instance = null;

	public static Interactable InteractableInRange { get; set; }

	private const float _IGNORE_HAXIS_WALL_JUMP_TIME = 1f;

	[Header("Player")]
	[SerializeField]
	private GameObject stillVersionPrefab;
	public GameObject StillVersionPrefab { get => stillVersionPrefab; }

	[Header("Inputs")]
	[SerializeField] private KeyCode _rightInput;
	[SerializeField] private KeyCode _leftInput;
	[SerializeField] private KeyCode _upInput;
	[SerializeField] private KeyCode _downInput;
	[SerializeField] private KeyCode _jumpInput;
	[SerializeField] private KeyCode _interactInput;
	[SerializeField] private KeyCode _attackInput;

	// variables

	private Animator anim;

	private MeleeWeapon _meleeWeapon;

	private bool jumpPressed, crouchPressed, attackPressed, interactionPressed;
	private bool canMove, canGroundJump, canWallJump, invinciblePowUp, _ignoreHAxis;
	bool _cantSlideWall;
	float _fallSlideWallMultiplyer, _attackChainTime;
	float _chainAttackTimer = 0.50f;

	private byte collectables;
	private float hAxis, vAxis, timeOfJump,
		initialMoveSpeed, initialJumpVelocity;

	private Vector3 spawnpoint;

	private Vector2 _wallCheckBoxSize = new Vector2(.3f, 3.0f);
	private Vector3 _wallCheckOffset = new Vector2(2.85f, 2.0f);
	private Vector3 _reverseWallCheckOffset = new Vector2(-2.85f, 2.0f);
	private Vector2 _interactionRange = new Vector2(12, 2);
	private Vector3 _offset = new Vector2(6, 3);

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
	public float HAxis { get => hAxis; }
	public float MoveSpeed { get => moveSpeed; set { moveSpeed = value; } }
	public float InitialMoveSpeed { get => initialMoveSpeed; }
	public float InitialJumpVelocity { get => initialJumpVelocity; }

	public bool PushingWall { get => OnGround && OnWall && HAxis != 0; }

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
	public bool AttackAnimationPlaying
	{
		get
		{
			return
				(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1")) ||
				(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2")) ||
				(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3")) ||
				_meleeWeapon.OnCooldown;
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
		spawnpoint = transform.position;


		base.Start();

		anim = GetComponent<Animator>();
		_meleeWeapon = GetComponentInChildren<MeleeWeapon>();

		canMove = true;
		canGroundJump = true;
		canWallJump = true;
		invinciblePowUp = false;
		Dead = false;
		timeOfJump = -1500.0f;
		initialMoveSpeed = moveSpeed;
		initialJumpVelocity = jumpVelocity;
		_cantSlideWall = false;
		_fallSlideWallMultiplyer = 1;

		_ignoreHAxis = false;
		_ignoreHAxisCor = null;

		// Initial invincibility frames
		SetInvunerability(true);
	}

	private void FixedUpdate()
	{
		UpdateRBVelocity();
	}

	// Update is called once per frame
	protected override void Update()
	{
		// Get inputs
		if (_readInputs)
		{
			hAxis = Input.GetAxis("Horizontal");
			jumpPressed = Input.GetKey(_jumpInput);
			crouchPressed = Input.GetKey(_downInput) && !jumpPressed && OnGround;
			interactionPressed = Input.GetKeyDown(_interactInput);
			canMove = !(crouchPressed);

			attackPressed = Input.GetKeyDown(_attackInput) && canMove && OnGround;
		}

		// Attack
		if (attackPressed)
			Attack();

		UpdateChainTime();

		// Prevent continuous ground jumping
		if (!jumpPressed && !canGroundJump && OnGround)
		{
			canGroundJump = true;
		}
		if (!jumpPressed && !canWallJump && (OnWall || OnGround))
			canWallJump = true;

		if (interactionPressed)
			InteractWithinRange();

		UpdateRotation();

		//Individual Updates
		if (jumpVelocity == InitialJumpVelocity)
			CapMaxYVelocity();
		//UpdateUI();

		base.Update();

		// ANIMATIOR
		anim.SetFloat("hAxis", hAxis);
		anim.SetFloat("yVeloc", rb.velocity.y);
		anim.SetBool("grounded", OnGround);
		anim.SetBool("canMove", canMove);
		anim.SetBool("crouched", crouchPressed);
		anim.SetBool("wallGrab", OnWall && !OnGround && canWallJump && !_cantSlideWall);
		anim.SetBool("pushingWall", PushingWall);

		if (hAxis >= 0.1f || hAxis <= -0.1f)
			anim.speed = hAxis > 0 ? 1 * hAxis : 1 * -hAxis;
		else
			anim.speed = 1;

		// TEMP DEATH PROCEDURE //
		if (HP <= 0 && !Dead)
			OnDeath();
	}

	private void UpdateRBVelocity()
	{
		Vector2 currentVelocity = rb.velocity;

		if (!AttackAnimationPlaying && !KnockedBack)
		{
			if (!_ignoreHAxis)
				currentVelocity = new Vector2(hAxis * moveSpeed, currentVelocity.y);
			else
			{
				float newXMoveSpeed = currentVelocity.x + HAxis * moveSpeed / 5;
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

				currentVelocity.x = turnedRight ? jumpVelocity / 1.8f : -jumpVelocity / 1.8f;

				_ignoreHAxisCor = StartCoroutine(CIgnoreHAxisFor(_IGNORE_HAXIS_WALL_JUMP_TIME));

				_fallSlideWallMultiplyer *= 2.5f;
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
			if (OnWall && canWallJump && !_cantSlideWall)
			{
				currentVelocity[0] = 0;
				currentVelocity[1] = -1 * _fallSlideWallMultiplyer;

				if (currentVelocity[1] < -60)
				{
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
				_fallSlideWallMultiplyer = 1;
				_cantSlideWall = false;
			}
		}

		if (OnWall && !OnGround)
		{
			currentVelocity[0] = 0;
		}

		if (!canMove)
			currentVelocity[0] = 0;

		rb.velocity = currentVelocity;
	}

	private void UpdateChainTime()
	{
		// Chain attack countdown
		if (_attackChainTime > 0)
		{
			_attackChainTime -= Time.deltaTime;
			anim.SetBool("AttackChainEnd", false);
		}
		else
			anim.SetBool("AttackChainEnd", true);
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

	// Power Up
	public void CollectablePickup(string powUp)
	{
		collectables++;
		//LevelMngr.Instance.CollectablePickup((byte)playerNumber, powUp);
	}

	public void Heal(byte ammount)
	{
		if (HP < 3)
			HP += ammount;
	}

	private void Attack()
	{
		if (!AttackAnimationPlaying)
		{
			Vector2 currentVelocity = rb.velocity;

			if (OnGround)
			{
				// Check attack intention
				if (!_meleeWeapon.OnCooldown)
				{
					_attackChainTime = _chainAttackTimer;

					anim.SetTrigger("Attack");

					_meleeWeapon.GroundAttack(currentVelocity, attackPSystem);
				}

			}
		}
	}

	protected override void OnHit(Vector2 hitDirection, float knockSpeed)
	{
		if (!invulnerable && !invinciblePowUp)
		{
			base.OnHit(hitDirection, knockSpeed);

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

	//private void OnDeath()
	//{
	//	Dead = true;
	//	deathParticle.Emit(Random.Range(95, 105));
	//	transform.DetachChildren();
	//	gameObject.SetActive(false);
	//}

	public override void KnockBack(Vector2 hitDirection, float knockSpeed, bool ignoreInvulnerability = false)
	{
		if (invulnerable && !ignoreInvulnerability) return;

		base.KnockBack(hitDirection, knockSpeed);
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

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.yellow;
	//	Gizmos.DrawCube(transform.position + _wallCheckOffset, new Vector3(_wallCheckBoxSize.x, _wallCheckBoxSize.y, 1));
	//}
}