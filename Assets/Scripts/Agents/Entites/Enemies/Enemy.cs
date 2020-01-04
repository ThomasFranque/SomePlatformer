using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
	[SerializeField] private bool _useColliderAsTrigger = false;
	[SerializeField] private float _knockIntensity = 50.0f;

	protected override void OnHit(bool cameFromRight, float knockSpeed, byte dmg)
	{
		if (invulnerable) return;

		base.OnHit(cameFromRight, knockSpeed, dmg);
		SetInvunerability(true);
	}

	protected override void OnDeath()
	{
		deathParticle.Emit(Random.Range(35, 55));
		base.OnDeath();

	}

	protected override void OnPlayerCollision(Collider2D col)
	{
		if (!_useColliderAsTrigger)
			HitPlayer(col);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (_useColliderAsTrigger && col.tag == "Player")
			HitPlayer(col);
	}

	private void HitPlayer(Collider2D col)
	{
		base.OnPlayerCollision(col);
		Player p = col.GetComponent<Player>();
		bool cameFromRight = p.transform.position.x < transform.position.x;


		p.Hit(cameFromRight, _knockIntensity);
	}
}
