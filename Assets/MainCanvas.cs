using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
	public PlayerUI PlayerUIScript { get; private set; }
	public PauseMenu PauseMenu { get; private set; }

	[SerializeField] private Animator _fadeAnim;

	// Start is called before the first frame update
	void Awake()
    {
		PlayerUIScript = GetComponentInChildren<PlayerUI>();
		PauseMenu = GetComponentInChildren<PauseMenu>();
	}

	public void FadeIn()
	{
		_fadeAnim.SetTrigger("FadeIn");
	}

	public void FadeOut()
	{
		_fadeAnim.SetTrigger("FadeOut");
	}
}
