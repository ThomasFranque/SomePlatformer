using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashedShipInteraction : InteractionListener
{
	protected override void InteractionFinishedAction()
	{
		GameObject.Find("CANVAS").GetComponent<MainCanvas>().FadeIn();
		LoadSave.Instance.LoadIn(4);
	}

	protected override void InteractionStartedAction()
	{

	}
}
