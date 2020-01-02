using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlinkingEffect : MonoBehaviour
{
	private SpriteRenderer spriteRenderer = null;
	private TextMeshProUGUI tmPro = null;
	private Image img = null;

	[SerializeField]
	private bool startBlinking = false;

	[Tooltip("In Seconds")]
	[SerializeField]
	private float blinkSpeed = 1.0f;

	public bool IsBlinking { get; private set; }

	private Coroutine blinkCoroutine;

	// Start is called before the first frame update
	void Start()
    {
		spriteRenderer = GetComponent<SpriteRenderer>();
		tmPro = GetComponent<TextMeshProUGUI>();
		img = GetComponent<Image>();

		if (startBlinking)
			StartBlink();
    }

    // Update is called once per frame
    void Update()
    {
		if (IsBlinking && blinkCoroutine == null)
			blinkCoroutine = StartCoroutine(CBlink());			
    }

	public void StartBlink()
	{
		IsBlinking = true;
	}

	public void StopBlink()
	{
		IsBlinking = false;
		if (blinkCoroutine != null)
		{
			StopCoroutine(blinkCoroutine);
			blinkCoroutine = null;
		}
		ToggleSprite();
	}

	public void SetBlinkSpeed(float seconds = 1.0f)
	{
		blinkSpeed = seconds;
		if (blinkCoroutine != null)
		{
			StopCoroutine(blinkCoroutine);
			blinkCoroutine = null;
		}
		ToggleSprite();
	}

	private void ToggleSprite(bool active = true)
	{
		if (spriteRenderer != null)
			spriteRenderer.enabled = active;
		else if (tmPro != null)
			tmPro.enabled = active;
		else if (img != null)
			img.enabled = active;
	}

	private IEnumerator CBlink()
	{
		yield return new WaitForSeconds(blinkSpeed);

		if (spriteRenderer != null)
			spriteRenderer.enabled = !spriteRenderer.enabled;
		else if (tmPro != null)
			tmPro.enabled = !tmPro.enabled;
		else if (img != null)
			img.enabled = !img.enabled;

		blinkCoroutine = null;
	}

	public void BlinkForSeconds(float seconds)
	{
		IsBlinking = true;
		StartCoroutine(CStopBlinkIn(seconds));
	}

	private IEnumerator CStopBlinkIn(float secs)
	{
		yield return new WaitForSeconds(secs);
		StopBlink();
	}

	private void OnEnable()
	{
		if (startBlinking)
			blinkCoroutine = StartCoroutine(CBlink());

	}
}
