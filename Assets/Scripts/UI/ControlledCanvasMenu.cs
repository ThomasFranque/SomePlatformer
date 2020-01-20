using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ControlledCanvasMenu : MonoBehaviour
{
	[SerializeField]
	protected AudioClip _buttonPressedSound;
	
	private EventSystem _eventSystem;
	protected SoundPlayer _sp;

	protected virtual void Awake()
	{
		_sp = new SoundPlayer();
	}

	protected virtual void Start()
	{
		_eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();	
	}

	public void PlaySound(AudioClip clip)
	{
		_sp.PlayOneShotGeneral(clip);
	}
}
