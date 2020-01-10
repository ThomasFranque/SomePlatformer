using System;
using UnityEngine;

public abstract class InteractionListener : MonoBehaviour, IInteractionListener
{
	private Action InteractionStart;
	private Action InteractionEnd;

	private void Start()
	{
		InteractionStart = InteractionStartedAction;
		InteractionEnd = InteractionFinishedAction;
	}

	public void InteractionStarted()
	{
		InteractionStart?.Invoke();
	}

	public void InteractionFinished()
	{
		InteractionEnd?.Invoke();
	}

	protected abstract void InteractionStartedAction();
	protected abstract void InteractionFinishedAction();
}
