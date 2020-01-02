using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When not overriden, will hit everything
/// </summary>
public class Hazard : MonoBehaviour
{
	protected float yHitDirection;
	protected float knockSpeed;

	protected virtual void Start()
	{
		yHitDirection = 1.5f;
		knockSpeed = 20.0f;
	}

	protected virtual void OnPlayerCollision(GameObject target)
	{
		Entity e = target.GetComponent<Entity>();

		Vector3 hitDirection =
			(e.transform.position - transform.position).normalized;
		hitDirection.y = yHitDirection;

		e.Hit(hitDirection, knockSpeed);
	}

	protected virtual void OnCometCollision(GameObject target)
	{ Destroy(target); }
	protected virtual void OnCollectableCollision(GameObject target)
	{ Destroy(target); }
	protected virtual void OnPlatformCollision(GameObject target) { }

	protected virtual void OnMovingPlatCollision(GameObject target) { }

	protected void SelfDestructIn(float seconds)
	{
		Invoke("DestroySelf", seconds);
	}

	protected void SetKnockSpeedAndY(float yHitDirection = 1.5f, float knockSpeed = 20.0f)
	{
		this.yHitDirection = yHitDirection;
		this.knockSpeed = knockSpeed;
	}

	private void DestroySelf()
	{
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player")
			OnPlayerCollision(col.gameObject);

		else if (col.tag == "Collectable")
			OnCollectableCollision(col.gameObject);

		else if (col.tag == "Platform")
			OnPlatformCollision(col.gameObject);

		else if (col.tag == "Moving Platform")
			OnMovingPlatCollision(col.gameObject);
	}

}
