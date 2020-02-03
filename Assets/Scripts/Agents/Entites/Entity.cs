using System;
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
	[SerializeField] protected Vector2 _selfKnockBackAmount = new Vector2(8.0f, 2.5f);
	[SerializeField] protected float _invulnerabilityTime = 0.5f;
	[SerializeField] protected float _knockBackTime = 0.5f;

	[SerializeField] protected SoundClips _soundClips;

	protected Animator _anim;
	protected Color _srBaseColor;

	public int HP { get => _hp; set { _hp = value; } }
	public bool Dead => HP <= 0;

	private float _knockBackTimer;

	protected Rigidbody2D _rb;
	protected SpriteRenderer _sr;

	protected Collider2D selfCol;

	protected GFXBlink _blink;

	protected SoundPlayer _soundPlayer;

	protected Vector2 _groundCheckBoxSize = new Vector2 (.15f, .25f);
	protected Vector3 _groundCheckOffset = new Vector2 (0.0f, 0.0f);

	protected bool _invulnerable;
	public bool FacingRight => transform.right.x > 0.0f;

	protected bool KnockedBack
	{
		get
		{
			if (_knockBackTimer > 0.0f)
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

	protected virtual void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
		selfCol = GetComponent<Collider2D>();

		_sr = GetComponent<SpriteRenderer>();
		_anim = GetComponent<Animator>();

		if (_anim == null)
			_anim = GetComponentInChildren<Animator>();
		if (_sr == null)
			_sr = GetComponentInChildren<SpriteRenderer>();
		if (deathParticle == null)
			deathParticle = GetComponentInChildren<ParticleSystem>();

		_srBaseColor = _sr.color;
		_blink = new GFXBlink();
		_soundPlayer = new SoundPlayer();
	}
	protected virtual void Update()
	{
		UpdateKnockback();
		UpdateInvulnerabilityEffect();
	}

	protected void CapMaxYVelocity()
	{
		if (_rb.velocity.y > 150)
			_rb.velocity = new Vector2 (_rb.velocity.x, 150);
		if (_rb.velocity.y < -120)
			_rb.velocity = new Vector2(_rb.velocity.x, -120);
	}

	protected virtual void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
	{
		//Debug.Log($"{name} was HIT!");

		HP -= dmg;
		_knockBackTimer = _knockBackTime;

		KnockBack(cameFromRight, knockSpeed);

		if (HP <= 0)
			OnDeath(dmg);
	}

	protected virtual void OnPlayerCollision(Collision2D col) { }


	protected void SetInvulnerability(bool active)
	{
		_invulnerable = active;
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
		_invulnerable = active;
	}

	protected virtual void UpdateInvulnerabilityEffect()
	{
		//Blink when necessary (invulnerable)
		if (_invulnerable)
			_blink.DoBlink(_sr, 0.04f);
		else if (!_sr.enabled && !_invulnerable)
			_sr.enabled = true;
	}

	private void UpdateKnockback()
	{
		// Knockback
		if (_knockBackTimer > 0.00f)
			_knockBackTimer -= Time.deltaTime;
	}

	public void Hit(bool cameFromRight, float knockSpeed, byte damage = 1)
	{
		OnHit(cameFromRight, knockSpeed, damage);
	}

	public void InstaKill()
	{
		Hit(true, 0, (byte)HP);
	}

	public virtual void KnockBack(bool cameFromRight, float knockSpeed)
	{
		if (_invulnerable) return;

		Vector2 finalKnock = _selfKnockBackAmount;
		finalKnock.x = cameFromRight ? -finalKnock.x : finalKnock.x;
		if (_rb != null)
			_rb.velocity = finalKnock * knockSpeed;
	}
	public virtual void AddictiveKnockBack(bool cameFromRight, float knockSpeed)
	{
		Vector2 finalKnock = _selfKnockBackAmount;
		finalKnock.x = cameFromRight ? -finalKnock.x : finalKnock.x;

		if (_rb != null)
			_rb.velocity += finalKnock * knockSpeed;
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player")
			OnPlayerCollision(col);
	}

	protected void SetGravityScale(float newG)
	{
		_rb.gravityScale = newG;
	}
	
	protected void SetSpriteColor(Color newColor)
	{
		_sr.color = newColor;
	}

	protected Color StoreCurrentSRColor()
	{
		_srBaseColor = _sr.color;
		return _srBaseColor;
	}

	protected void WaitBeforeAction(Action action, float seconds)
	{
		StartCoroutine(CWaitBeforeAction(action, seconds));
	}

	private IEnumerator CWaitBeforeAction(Action action, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		action?.Invoke();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(transform.position + _groundCheckOffset, new Vector3(_groundCheckBoxSize.x, _groundCheckBoxSize.y, 1));
	}
}
