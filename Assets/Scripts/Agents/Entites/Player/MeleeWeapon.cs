using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
	// WARNING
	// THE COOLDOWN FLOAT CAN BE IGNORED / EXTENDED TO MATCH ANIMATION TIME EXTERNALY

	[Header("--- Melee Weapon properties ---")]
	[SerializeField] protected byte damage = 1;
	[SerializeField] protected float cooldown, delayToActivate, groundAttackXVelocity = 1, airAttackXVelocity = 10;
	[SerializeField] protected Vector2 weaponColSize = default;
	[SerializeField] protected LayerMask enemiesMask = 0;
	[SerializeField] private float _knockSpeed = 0;


	protected Collider2D weaponCol;
	protected Player player;

	public bool OnCooldown { get; set; }

	protected virtual void Start()
	{
		OnCooldown = false;
		player = GetComponentInParent<Player>();
	}

	public void GroundAttack(Vector2 currentPVelocity, ParticleSystem ps)
	{
		if (!OnCooldown)
		{
			OnCooldown = true;
			StartCoroutine(CGroundAttackDelay(currentPVelocity, ps));
		}
	}

	public void AirAttack(Vector2 currentPVelocity, ParticleSystem ps)
	{
		if (!OnCooldown)
		{
			OnCooldown = true;
			AttackSequence(currentPVelocity, ps, currentPVelocity.x);
		}
	}

	private IEnumerator CCooldown()
	{
		yield return new WaitForSeconds(cooldown);
		OnCooldown = false;
	}

	private IEnumerator CGroundAttackDelay(Vector2 currentPVelocity, ParticleSystem ps)
	{
		yield return new WaitForSeconds(delayToActivate);
		AttackSequence(currentPVelocity, ps, groundAttackXVelocity);
	}

	private void AttackSequence(Vector2 currentPVelocity, ParticleSystem ps, float xVel)
	{

		ps.Emit(10);

		StartCoroutine(CCooldown());

		if (transform.rotation == Quaternion.identity && Mathf.Sign(xVel) != 1 ||
			transform.rotation != Quaternion.identity && Mathf.Sign(xVel) != -1)
			xVel = -xVel;

		// Forward Movement
		player.SetVelocity(
			new Vector2(
				xVel,
				currentPVelocity.y / 2));

		Collider2D[] enemiesInRange =
			Physics2D.OverlapBoxAll(
				transform.position,
				weaponColSize,
				0,
				enemiesMask);


		HitAllInRange(enemiesInRange);
	}

	private void HitAllInRange(Collider2D[] enemiesInRange)
	{
		foreach (Collider2D enemyCol in enemiesInRange)
		{
			ICanBeHit e = enemyCol.GetComponent<ICanBeHit>();

			bool cameFromRight = enemyCol.transform.position.x < transform.position.x;

			e?.Hit(cameFromRight, _knockSpeed);

		}
	}

	protected void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, weaponColSize);
		//Gizmos.DrawWireCube(airAttackPos.transform.position, weaponColSize);
	}
}
