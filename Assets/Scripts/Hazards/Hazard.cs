using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// When not overridden, will hit everything
public class Hazard : MonoBehaviour
{
	[SerializeField] protected byte _damage = 1;
	[SerializeField] protected float yHitDirection = 1.5f;
	[SerializeField] protected float knockSpeed = 100.0f;

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

	private void OnTriggerEnter2D(Collider2D col)
	{
		switch (col.tag)
		{
			case "Player":
			case "Enemy":
				OnEntityCollision(col.gameObject);
				break;
			case "Collectible":
				OnCollectableCollision(col.gameObject);
				break;
		}
		Debug.Log(col.name);
	}
}
