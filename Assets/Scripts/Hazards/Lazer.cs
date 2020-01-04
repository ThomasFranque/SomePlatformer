using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : Hazard
{
	[SerializeField] Sprite image1 = null;
	[SerializeField] Sprite image2 = null;

	private float fillSpeed = 8.0f;
	private SpriteRenderer sr = null;
	
	private void Start()
	{
		sr = GetComponent<SpriteRenderer>();

		if (Random.Range(0.0f, 1.0f) > 0.5f)
			sr.sprite = image1;
		else
			sr.sprite = image2;

		SetFill(0);
	}

	private void Update()
	{
		SetFill(transform.localScale.y + (Time.deltaTime * fillSpeed));
	}

	public void SetFill(float fillAmount)
	{
		Vector3 newScale = transform.localScale;
		newScale.y = fillAmount;
		transform.localScale = newScale;
	}
}
