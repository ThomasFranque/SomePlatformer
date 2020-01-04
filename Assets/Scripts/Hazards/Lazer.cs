using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : Hazard
{
	[SerializeField] Sprite image1 = null;
	[SerializeField] Sprite image2 = null;

	private float yFillSpeed = 8.0f;
	private float xFillSpeed = 16.0f;
	private SpriteRenderer sr = null;

	private bool xMaxReached;

	private void Start()
	{
		sr = GetComponent<SpriteRenderer>();

		if (Random.Range(0.0f, 1.0f) > 0.5f)
			sr.sprite = image1;
		else
			sr.sprite = image2;

		SetFill(0, 0);
	}

	private void Update()
	{
		CheckIfXMaxReached();
		SetFill(transform.localScale.y + (Time.deltaTime * yFillSpeed), transform.localScale.x + (Time.deltaTime * (xMaxReached ? -xFillSpeed : xFillSpeed)));
	}

	public void SetFill(float yFillAmount, float xFillAmount)
	{
		Vector3 newScale = transform.localScale;
		newScale.y = yFillAmount;
		newScale.x = Mathf.Clamp(xFillAmount, 0 ,1);
		transform.localScale = newScale;
	}

	private void CheckIfXMaxReached()
	{
		if (transform.localScale.x >= 0.99f)
			xMaxReached = true;
	}
}
