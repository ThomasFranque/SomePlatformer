using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GFXBlink
{
	private float lastBlinkTime;
	private bool toggle;

	private float ElapsedTime => Time.unscaledTime - lastBlinkTime;

	public GFXBlink(bool toggle = true)
	{
		lastBlinkTime = Time.unscaledTime;
		this.toggle = toggle;
	}

	private bool ShouldBlink(float secsBB) => ElapsedTime > secsBB;

	public void DoBlink(SpriteRenderer targetRenderer, float secsBetweenBlinks)
	{
		if (ShouldBlink(secsBetweenBlinks))
		{
			lastBlinkTime = Time.unscaledTime;
			toggle = !toggle;
			targetRenderer.enabled = toggle;
		}
	}

	public void DoBlink(TextMeshProUGUI targetRenderer, float secsBetweenBlinks)
	{
		if (ShouldBlink(secsBetweenBlinks))
		{
			lastBlinkTime = Time.unscaledTime;
			toggle = !toggle;
			targetRenderer.enabled = toggle;
		}
	}
	public void DoBlink(Image targetRenderer, float secsBetweenBlinks)
	{
		if (ShouldBlink(secsBetweenBlinks))
		{
			lastBlinkTime = Time.unscaledTime;
			toggle = !toggle;
			targetRenderer.enabled = toggle;
		}
	}
	public void DoBlink(GameObject targetObj, float secsBetweenBlinks)
	{
		if (ShouldBlink(secsBetweenBlinks))
		{
			lastBlinkTime = Time.unscaledTime;
			toggle = !toggle;
			targetObj.SetActive(toggle);
		}
	}
}
