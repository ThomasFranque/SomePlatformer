using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour
{
	[SerializeField] private DialogBox _inputPopUpScript = null;
	[SerializeField] private Vector2 _interactionRange = new Vector2(24, 2);
	[SerializeField] private Vector2 _interactionOffset = new Vector2(0,2);

	protected Player Player { get; private set; }

	private Action PlayerInteraction;
	private bool _playerRangeTrigger = false;

	protected bool IsPlayerInRange
	{
		get
		{
			Collider2D col = Physics2D.OverlapBox(
				transform.position + (Vector3)_interactionOffset,
				_interactionRange,
				0,
				LayerMask.GetMask("Player"));

			return col != null;
		}
	}
	// Start is called before the first frame update
	protected virtual void Start()
    {
		PlayerInteraction += _inputPopUpScript.Exit;
		Player = null;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (IsPlayerInRange && !_playerRangeTrigger)
		{
			_playerRangeTrigger = true;
			OnPlayerEnterRange();
		}
		else if (!IsPlayerInRange && _playerRangeTrigger)
		{
			_playerRangeTrigger = false;
			OnPlayerExitRange();
		}

    }

	public void Interact(Player p)
	{
		if (Player == null)
			Player = p;

		OnPlayerInteraction();
	}

	public virtual void ExitInteraction(byte index = 0)
	{
		if (IsPlayerInRange)
			_inputPopUpScript.Display(this);
		else
			_inputPopUpScript.Exit();
	}

	protected virtual void OnPlayerEnterRange()
	{
		Player.InteractableInRange = this;
		_inputPopUpScript?.Display(this);
	}

	protected virtual void OnPlayerExitRange()
	{
		Player.InteractableInRange = null;
		_inputPopUpScript.Exit();

	}

	protected void AddActionToInteraction(Action action)
	{
		PlayerInteraction += action;
	}

	private void OnPlayerInteraction()
	{
		PlayerInteraction?.Invoke();
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position + (Vector3)_interactionOffset, new Vector3(_interactionRange.x, _interactionRange.y, 1));
	}
}
