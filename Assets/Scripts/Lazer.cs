using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : Hazard
{
	[SerializeField] Sprite image1 = null;
	[SerializeField] Sprite image2 = null;

	private SpriteRenderer sr = null;
	protected override void Start()
	{
		base.Start();

		sr = GetComponent<SpriteRenderer>();

		if (Random.Range(0.0f, 1.0f) > 0.5f)
			sr.sprite = image1;
		else
			sr.sprite = image2;
	}
}
