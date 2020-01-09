using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ControlledCanvasMenu : MonoBehaviour
{
	private EventSystem _eventSystem;

	protected virtual void Start()
	{
		_eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();	
	}
}
