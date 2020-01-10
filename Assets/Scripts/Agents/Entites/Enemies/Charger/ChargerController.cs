using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerController : Enemy, IBehaviourController
{
	[SerializeField] private Transform _sightEndTransform;
	[SerializeField] private MonoBehaviour[] _behaviours;
	[SerializeField] private float _flipWaitDuration = 3.0f;
	[SerializeField] private float _chargeSpeed = 15.0f;
	[SerializeField] private float _chargeDuration = 2.0f;
	[SerializeField] private float _chargeCooldown = 5.0f;

	private float _timeOfPlayerSight;
	private float _timeOfChargeEnd;
	private float _timeOflastFlip;

	private Action CurrentBehaviour;

	public bool PlayerInRange
	{
		get
		{
			return Physics2D.Linecast(
				transform.position + new Vector3(0,4,0),
				_sightEndTransform.position,
				LayerMask.GetMask("Player"));
		}
	}

	protected override void Start()
	{
		base.Start();
		_timeOfChargeEnd = -900;
		CurrentBehaviour = IdleBehaviour;
	}
	protected override void Update()
	{
		base.Update();
		CurrentBehaviour?.Invoke();
	}

	public void ActivateBehaviour(MonoBehaviour sender, MonoBehaviour nextBehavior)
	{

	}

	private void Flip()
	{
		_timeOflastFlip = Time.time;

		if (transform.rotation == Quaternion.identity)
			transform.rotation = Quaternion.Euler(0, 180.0f, 0);
		else
			transform.rotation = Quaternion.identity;
	}

	protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
	{
		if (CurrentBehaviour != ChargeBehaviour)
			base.OnHit(cameFromRight, knockSpeed, dmg);
	}

	#region Idle
	private void IdleBehaviour()
	{
		if (Time.time - _timeOfChargeEnd > _chargeCooldown)
		{
			// While not on cooldown
			if (Time.time - _timeOflastFlip > _flipWaitDuration)
				Flip();
			else if(Time.time - _timeOflastFlip < _flipWaitDuration - _flipWaitDuration / 3)
			{
				Patrol();
				_anim.SetBool("IdleWalking", true); ;
			}
			else
				_anim.SetBool("IdleWalking", false); ;

			if (PlayerInRange)
				OnPlayerSighted();
			_hurtsPlayer = true;

			_anim.SetBool("OnCooldown", false);

		}
		else
		{
			// While on cooldown
			_timeOflastFlip = Time.time;
			_hurtsPlayer = false;
			_anim.SetBool("IdleWalking", false);
			_anim.SetBool("OnCooldown", true);
		}
	}

	private void OnPlayerSighted()
	{
		_timeOfPlayerSight = Time.time;
		CurrentBehaviour = ChargeBehaviour;
	}

	private void Patrol()
	{
		if (transform.rotation == Quaternion.identity)
			SetVelocity(15, rb.velocity.y);
		else
			SetVelocity(-15, rb.velocity.y);
	}
	#endregion

	#region Charge
	private void ChargeBehaviour()
	{
		if (Time.time - _timeOfPlayerSight < _chargeDuration)
		{
			if (transform.rotation == Quaternion.identity)
				SetVelocity(_chargeSpeed, 0);
			else
				SetVelocity(-_chargeSpeed, 0);
			_anim.SetBool("Charging", true);
		}
		else
		{
			// On Charge End
			_timeOfChargeEnd = Time.time;
			SetVelocity(rb.velocity / 2);
			CurrentBehaviour = IdleBehaviour;
			_anim.SetBool("Charging", false);
		}
	}

	protected override void OnHitPlayer()
	{
		base.OnHitPlayer();
		if (CurrentBehaviour == ChargeBehaviour)
			SetVelocity(rb.velocity / 3);
		else
			Flip();
	}

	protected override void OnPlayerStomp(Player p)
	{
		if (CurrentBehaviour == ChargeBehaviour)
			SetVelocity(rb.velocity / 3);
		else if (!invulnerable)
			Flip();
		base.OnPlayerStomp(p);

	}
	#endregion

	protected override void OnDeath(byte dmg = 1)
	{
		base.OnDeath(dmg);
		Destroy(gameObject);
	}

	public void SetVelocity(float newX, float newY)
	{
		SetVelocity(new Vector2(newX, newY));
	}
	public void SetVelocity(Vector2 newVelocity)
	{
		rb.velocity = newVelocity;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(transform.position + new Vector3(0, 4, 0), _sightEndTransform.position);
	}
}
