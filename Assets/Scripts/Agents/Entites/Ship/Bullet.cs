using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private BulletProperties _bulletProperties;

	private void Start()
	{
		GetComponent<SpriteRenderer>().sprite = _bulletProperties.RandomSprite;
		StartCoroutine(CDestroyTimer());
	}

	private void Update()
	{
		transform.position += transform.up * _bulletProperties.Speed * Time.deltaTime;
	}

	private IEnumerator CDestroyTimer()
	{
		yield return new WaitForSeconds(_bulletProperties.Lifetime);
		Destroy(gameObject);
	}
}
