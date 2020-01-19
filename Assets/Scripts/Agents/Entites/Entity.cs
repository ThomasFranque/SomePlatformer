using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Entity : MonoBehaviour, ICanBeHit
{
	[Header("Entity")]
	[SerializeField] private int _hp = 3;
	[SerializeField] protected ParticleSystem deathParticle;
	[SerializeField] protected Vector2 _selfKnockBackAmmount = new Vector2(32.0f, 2.5f);
	[SerializeField] protected float _invulnerabilityTime = 0.5f;
	[SerializeField] protected float _knockBackTime = 0.5f;

	protected Animator _anim;

	public int HP { get => _hp; set { _hp = value; } }

	private float knockBackTimer;

	protected Rigidbody2D rb;
	protected SpriteRenderer sr;

	protected Collider2D selfCol;

	protected GFXBlink _blink;

	protected Vector2 _groundCheckBoxSize = new Vector2 (.15f, .25f);
	protected Vector3 _groundCheckOffset = new Vector2 (0.0f, 0.0f);

	protected Stack<string> ignoreCollisionTags;

	protected bool invulnerable;

	protected bool KnockedBack
	{
		get
		{
			if (knockBackTimer > 0.0f)
				return true;
			return false;
		}
	}

	public bool OnGround
	{
		get
		{
			Collider2D[] collider = Physics2D.OverlapBoxAll(
				transform.position + _groundCheckOffset,
				_groundCheckBoxSize,
				0,
				LayerMask.GetMask("Ground"));

			for (byte i = 0; i < collider.Length; i++)
				if (collider[i] != null) return true;

			return false;
		}
	}

	protected bool TurnedRight => transform.right.x > 0.0f;

	protected virtual void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		selfCol = GetComponent<Collider2D>();

		sr = GetComponent<SpriteRenderer>();
		_anim = GetComponent<Animator>();

		if (_anim == null)
			_anim = GetComponentInChildren<Animator>();
		if (sr == null)
			sr = GetComponentInChildren<SpriteRenderer>();
		if (deathParticle == null)
			deathParticle = GetComponentInChildren<ParticleSystem>();
		ignoreCollisionTags = new Stack<string>();

		_blink = new GFXBlink();

	}
	protected virtual void Update()
	{
		UpdateKnockback();
		UpdateInvunerabilityEffect();
	}

	protected void CapMaxYVelocity()
	{
		if (rb.velocity.y > 150)
			rb.velocity = new Vector2 (rb.velocity.x, 150);
		if (rb.velocity.y < -120)
			rb.velocity = new Vector2(rb.velocity.x, -120);
	}

	protected virtual void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
	{
		//Debug.Log($"{name} was HIT!");

		HP -= dmg;
		knockBackTimer = _knockBackTime;

		KnockBack(cameFromRight, knockSpeed);

		if (HP <= 0)
			OnDeath(dmg);
	}

	protected virtual void OnPlayerCollision(Collision2D col) { }


	protected void SetInvulnerability(bool active)
	{
		invulnerable = active;
		if (gameObject.activeSelf)
			StartCoroutine(CSetInvulnerability(!active));
	}

	protected virtual void OnDeath(byte dmg = 1)
	{
		deathParticle.transform.parent = null;
		DeathCamShake(dmg);
		gameObject.SetActive(false);
	}

	protected void DeathCamShake(byte dmg)
	{
		CameraActions.ActiveCamera.Shake(20 * dmg, 30 * dmg, 0.1f);
	}

	private IEnumerator CSetInvulnerability(bool active)
	{
		yield return new WaitForSeconds(_invulnerabilityTime);
		invulnerable = active;
	}

	private void UpdateInvunerabilityEffect()
	{
		//Blink when necessary (invunerable)
		if (invulnerable)
			_blink.DoBlink(sr, 0.04f);
		else if (!sr.enabled && !invulnerable)
			sr.enabled = true;
	}

	private void UpdateKnockback()
	{
		// Knockback
		if (knockBackTimer > 0.00f)
			knockBackTimer -= Time.deltaTime;
	}

	public void Hit(bool cameFromRight, float knockSpeed, byte damage = 1)
	{
		OnHit(cameFromRight, knockSpeed, damage);
	}

	public void InstaKill()
	{
		HP -= HP;
	}

	public virtual void KnockBack(bool cameFromRight, float knockSpeed)
	{
		if (invulnerable) return;

		Vector2 finalKnock = _selfKnockBackAmmount;
		finalKnock.x = cameFromRight ? -finalKnock.x : finalKnock.x;
		if (rb != null)
			rb.velocity = finalKnock * knockSpeed;
	}
	public virtual void AddictiveKnockBack(bool cameFromRight, float knockSpeed)
	{
		Vector2 finalKnock = _selfKnockBackAmmount;
		finalKnock.x = cameFromRight ? -finalKnock.x : finalKnock.x;

		if (rb != null)
			rb.velocity += finalKnock * knockSpeed;
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player")
			OnPlayerCollision(col);
		else if (col.gameObject.tag == "Tilemap")
			OnEnterGroundCollision();
	}

	protected virtual void OnEnterGroundCollision() { }
	protected virtual void OnExitGroundCollision() { }

	protected void SetGravityScale(float newG)
	{
		
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(transform.position + _groundCheckOffset, new Vector3(_groundCheckBoxSize.x, _groundCheckBoxSize.y, 1));
	}
}
