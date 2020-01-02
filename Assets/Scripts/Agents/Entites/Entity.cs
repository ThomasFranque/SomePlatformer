using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	[SerializeField] private int _hp = 3;
	[SerializeField] protected ParticleSystem deathParticle;


	public int HP { get => _hp; protected set { _hp = value; } }

	private float knockBackTimer;

	protected Rigidbody2D rb;
	protected SpriteRenderer sr;

	protected Collider2D selfCol;

	private GFXBlink _blink;

	protected Vector2 groundCheckBoxSize = new Vector2 (5.4f, .25f);

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
				transform.position,
				groundCheckBoxSize,
				0,
				LayerMask.GetMask("Ground"));

			for (byte i = 0; i < collider.Length; i++)
				if (collider[i] != null && rb.velocity.y >= -0.01f) return true;

			return false;
		}
	}

	protected virtual void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		selfCol = GetComponent<Collider2D>();
		sr = GetComponent<SpriteRenderer>();

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

	protected virtual void OnHit(Vector2 hitDirection, float knockSpeed)
	{
		//Debug.Log($"{name} was HIT!");

		HP -= 1;
		knockBackTimer = 0.5f;

		rb.velocity = knockSpeed * hitDirection;

		if (HP <= 0)
			OnDeath();
	}

	protected virtual void OnPlayerCollision(Collider2D col) { }


	protected void SetInvunerability(bool active)
	{
		invulnerable = active;
		if (gameObject.activeSelf)
			StartCoroutine(CSetInvulnerability(!active));
	}

	protected virtual void OnDeath()
	{
		deathParticle.Emit(Random.Range(95, 105));
		transform.DetachChildren();
		gameObject.SetActive(false);
	}

	private IEnumerator CSetInvulnerability(bool active)
	{
		yield return new WaitForSeconds(2);
		invulnerable = active;
	}

	private void UpdateInvunerabilityEffect()
	{
		//Blink when necessary (invunerable)
		if (invulnerable)
			_blink.DoBlink(sr, 3);
		else if (!sr.enabled && !invulnerable)
			sr.enabled = true;
	}

	private void UpdateKnockback()
	{
		// Knockback
		if (knockBackTimer > 0.00f)
			knockBackTimer -= Time.deltaTime;
	}

	public void Hit(Vector2 hitDirection, float knockSpeed)
	{
		OnHit(hitDirection, knockSpeed);
	}

	public void InstaKill()
	{
		HP -= HP;
	}

	public virtual void KnockBack(Vector2 hitDirection, float knockSpeed, bool ignoreInvulnerability = false)
	{
		knockBackTimer = 0.5f;

		rb.velocity = knockSpeed * hitDirection;
	}
	public virtual void AddictiveKnockBack(Vector2 hitDirection, float knockSpeed, bool ignoreInvulnerability = false)
	{
		if (ignoreInvulnerability)
		knockBackTimer = 0.5f;

		rb.velocity += knockSpeed * hitDirection;
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player")
			OnPlayerCollision(col.collider);
	}

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.yellow;
	//	Gizmos.DrawCube(transform.position,new Vector3(	groundCheckBoxSize.x, groundCheckBoxSize.y, 1));
	//}
}
