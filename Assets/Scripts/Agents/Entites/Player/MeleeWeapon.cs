using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
	// WARNING
	// THE COOLDOWN FLOAT CAN BE IGNORED / EXTENDED TO MATCH ANIMATION TIME EXTERNALY

	[Header("--- Melee Weapon properties ---")]
	[SerializeField] protected byte damage;
	[SerializeField] protected float cooldown, delayToActivate, groundAttackXVelocity, airAttackYVelocity;
	[SerializeField] protected Vector2 weaponColSize;
	[SerializeField] protected LayerMask enemiesMask;
	[SerializeField] private Transform airAttackPos;
	[SerializeField] private float _knockSpeed;


	protected Collider2D weaponCol;
	protected Player player;

	public bool OnCooldown { get; set; }

	protected virtual void Start()
	{
		OnCooldown = false;
		player = GetComponentInParent<Player>();
	}

	protected virtual void Update()
	{
		if (transform.rotation == Quaternion.identity && Mathf.Sign(groundAttackXVelocity) != 1 ||
			transform.rotation != Quaternion.identity && Mathf.Sign(groundAttackXVelocity) != -1)
		{
			groundAttackXVelocity = -groundAttackXVelocity;
		}
	}
	public void GroundAttack(Vector2 currentPVelocity, ParticleSystem ps)
	{
		if (!OnCooldown)
		{
			OnCooldown = true;
			StartCoroutine(CGroundAttackDelay(currentPVelocity, ps));
		}
	}

	public void AirAttack(Vector2 currentPVelocity)
	{
		if (!OnCooldown)
		{
			OnCooldown = true;
			StartCoroutine(CAirAttackDelay(currentPVelocity));
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
		ps.Emit(Random.Range(15, 30));
		StartCoroutine(CCooldown());

		// Forward Movement
		player.SetVelocity(
			new Vector2(
				groundAttackXVelocity,
				currentPVelocity.y));

		Collider2D[] enemiesInRange =
			Physics2D.OverlapBoxAll(
				transform.position,
				weaponColSize,
				0,
				enemiesMask);


		HitAllInRange(enemiesInRange);
	}

	private IEnumerator CAirAttackDelay(Vector2 currentPVelocity)
	{
		yield return new WaitForSeconds(delayToActivate / 2);
		StartCoroutine(CCooldown());

		Collider2D[] enemiesInRange =
			Physics2D.OverlapBoxAll(
				airAttackPos.transform.position,
				weaponColSize,
				0,
				enemiesMask);

		// Boost up if enemies hit
		if (enemiesInRange.Length > 0)
			player.SetVelocity(new Vector2(currentPVelocity.x, airAttackYVelocity));


		HitAllInRange(enemiesInRange);
	}

	private void HitAllInRange(Collider2D[] enemiesInRange)
	{
		foreach (Collider2D enemyCol in enemiesInRange)
		{
			Vector3 hitDirection =
				(enemyCol.transform.position - transform.position).normalized;
			hitDirection.y = 1.5f;

			enemyCol.GetComponent<Entity>().Hit(hitDirection, _knockSpeed);

		}
	}

	protected void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, weaponColSize);
		//Gizmos.DrawWireCube(airAttackPos.transform.position, weaponColSize);
	}
}
