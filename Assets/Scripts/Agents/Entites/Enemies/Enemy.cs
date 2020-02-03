using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
	private const float _STOMP_CENTER_Y_OFFSET = 0.0f;
	private const float _TIME_BEFORE_FINAL_DEATH = .15f;

	[Header("Enemy")]
	[SerializeField] protected EnemyProperties _selfProperties;

	[SerializeField] protected bool _hurtsPlayer = true;
	[SerializeField] protected bool _canBeStomped = true;
	[SerializeField] protected bool _useColliderAsTrigger = false;
	[SerializeField] private float _stompYSpeed = 250.0f;
	[SerializeField] private float _knockIntensity = 50.0f;

	protected bool _colliderTouchingSolidGround;

	protected override void Start()
	{
		base.Start();
		GetEnemyProperties();
		_anim = GetComponentInChildren<Animator>();
	}

	protected void GetEnemyProperties()
	{
		if (_selfProperties != null)
		{
			_hurtsPlayer = _selfProperties.HurtsPlayer;
			_canBeStomped = _selfProperties.CanBeStomped;
			_useColliderAsTrigger = _selfProperties.UseColliderAsTrigger;
			_stompYSpeed = _selfProperties.StompYSpeed;
			_knockIntensity = _selfProperties.KnockBackIntesity;
		}
		else
			Debug.LogWarning($"Enemy Properties on {name.ToUpper()} not assigned. Using default values.\n Please create one from the asset menu.");
	}

	protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
	{
		if (invulnerable) return;
		CameraActions.ActiveCamera.Shake(10 * dmg, 20 * dmg, 0.06f);
		deathParticle?.Emit(Random.Range(3 * dmg, 5 * dmg));

		base.OnHit(cameFromRight, knockSpeed, dmg);
		SetInvulnerability(true);
	}

	protected override void OnDeath(byte dmg = 1)
	{
			StartCoroutine(CDeathSequence(dmg));
	}

	protected override void OnPlayerCollision(Collision2D col)
	{
		base.OnPlayerCollision(col);
		if (!_useColliderAsTrigger && _canBeStomped)
		{
			Vector3 contactPoint = col.GetContact(0).point;
			Vector3 center = selfCol.bounds.center;

			bool right = contactPoint.x < center.x;
			bool top = Player.Instance.transform.position.y > center.y + _STOMP_CENTER_Y_OFFSET;

			if (top)
			{
				OnPlayerStomp(col.gameObject.GetComponent<Player>());
				return;
			}
		}

		HitPlayer(col.collider);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (_useColliderAsTrigger && col.tag == "Player")
		{
			Vector3 center = selfCol.bounds.center;
			bool top = Player.Instance.transform.position.y > center.y + _STOMP_CENTER_Y_OFFSET;

			if (top && _canBeStomped)
			{
				OnPlayerStomp(col.gameObject.GetComponent<Player>());
				return;
			}

			if (_hurtsPlayer) HitPlayer(col);

		}
		else if (col.gameObject.tag == "Tilemap") 
			OnTriggerExitGroundCollision();
	}
	private void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.tag == "Tilemap")
			OnTriggerEnterGroundCollision();
	}

	private void HitPlayer(Collider2D col)
	{
		if (_hurtsPlayer)
		{
			Player p = col.GetComponent<Player>();
			bool cameFromRight = p.transform.position.x < transform.position.x;
			OnHitPlayer();

			p.Hit(cameFromRight, _knockIntensity);
		}
	}

	protected virtual void OnHitPlayer()
	{

	}

	protected virtual void OnPlayerStomp(Player p)
	{
		p.DoStomp(_stompYSpeed);
		Hit(true, 0.0f);
	}

	protected virtual void OnTriggerEnterGroundCollision()
	{
		_colliderTouchingSolidGround = true;
	}
	protected virtual void OnTriggerExitGroundCollision()
	{
		_colliderTouchingSolidGround = false;
	}

	protected override void OnEnterGroundCollision()
	{
		base.OnEnterGroundCollision();
		_colliderTouchingSolidGround = true;
	}
	protected override void OnExitGroundCollision()
	{
		base.OnExitGroundCollision();
		_colliderTouchingSolidGround = false;
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.transform.CompareTag("Player"))
			OnPlayerCollision(collision);
	}

	private IEnumerator CDeathSequence(byte deathDmg)
	{
		_hurtsPlayer = false;
		sr.color = Color.white;
		yield return new WaitForSeconds(_TIME_BEFORE_FINAL_DEATH);
		deathParticle?.Emit(Random.Range(35, 55));
		base.OnDeath(deathDmg);
	}
}
