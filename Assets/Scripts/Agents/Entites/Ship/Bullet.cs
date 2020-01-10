using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	private const float _LIFE_TIME = 2.0f;

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
		yield return new WaitForSeconds(_LIFE_TIME);
		Destroy(gameObject);
	}
}
