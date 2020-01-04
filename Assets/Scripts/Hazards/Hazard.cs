using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When not overridden, will hit everything
public class Hazard : MonoBehaviour
{
	[SerializeField] protected byte _damage = 1;
	[SerializeField] protected float yHitDirection = 1.5f;
	[SerializeField] protected float knockSpeed = 100.0f;
	[SerializeField] protected bool _hitEnemies = true;
	[SerializeField] private LayerMask _groundLayer;

	protected virtual void OnEntityCollision(GameObject target)
	{
		Entity e = target.GetComponent<Entity>();

		bool cameFromRight = e.transform.position.x < transform.position.x;

		//Vector3 hitDirection =
		//	(e.transform.position - transform.position).normalized;
		//hitDirection.y = yHitDirection;

		e.Hit(cameFromRight, knockSpeed, _damage);
	}


	protected virtual void OnCollectableCollision(GameObject target)
	{ Destroy(target); }

	protected void SelfDestructIn(float seconds)
	{
		Invoke("DestroySelf", seconds);
	}

	// CalledInSelfDestructIn
	private void DestroySelf()
	{
		Destroy(gameObject);
	}

	private void TryInteractWith(GameObject hitGO)
	{
		switch (hitGO.tag)
		{
			case "Player":
					OnEntityCollision(hitGO);
				break;
			case "Enemy":
				if (_hitEnemies) OnEntityCollision(hitGO);
				break;
			case "Collectible":
				OnCollectableCollision(hitGO);
				break;
			default:
				break;
		}
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		TryInteractWith(col.gameObject);
	}
}
