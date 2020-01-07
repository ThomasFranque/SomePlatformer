﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
	private const float _STOMP_CENTER_Y_OFFSET = 0.0f;

	[SerializeField] private bool _hurtsPlayer = true;
	[SerializeField] private bool _canBeStomped = true;
	[Tooltip("Using as trigger will disable stomp.")]
	[SerializeField] protected bool _useColliderAsTrigger = false;
	[SerializeField] private float _stompYSpeed = 250.0f;
	[SerializeField] private float _knockIntensity = 50.0f;

	protected bool _colliderTouchingSolidGround;

	protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
	{
		if (invulnerable) return;
		CameraActions.ActiveCamera.Shake(10 * dmg, 20 * dmg, 0.1f);
		deathParticle?.Emit(Random.Range(3 * dmg, 5 * dmg));

		base.OnHit(cameFromRight, knockSpeed, dmg);
		SetInvunerability(true);
	}

	protected override void OnDeath(byte dmg = 1)
	{
		deathParticle?.Emit(Random.Range(35, 55));
		base.OnDeath();
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
		if (_useColliderAsTrigger && col.tag == "Player" && _hurtsPlayer)
			HitPlayer(col);
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


			p.Hit(cameFromRight, _knockIntensity);
		}
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
}
