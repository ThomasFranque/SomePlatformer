using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable
{
	[SerializeField] private DialogBox _dialogBoxScript = null;
	[SerializeField] private string[] _firstDialog = null;
	[SerializeField] private string[] _defaultDialog = null;
	
	private Cinemachine.CinemachineVirtualCamera _interactionCam = null;

	private bool _alreadyInteracted = false;

    // Start is called before the first frame update
    protected override void Start()
    {
		base.Start();

		_interactionCam = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();

		AddActionToInteraction(OnInteraction);

		if (_defaultDialog.Length == 0)
			_defaultDialog = _firstDialog;

	}

    // Update is called once per frame
    protected override void Update()
    {
		base.Update();
    }

	private void OnInteraction()
	{
		Player.SetInputReading(false);
		_interactionCam.enabled = true;
		_dialogBoxScript.Display(this, _alreadyInteracted ? _defaultDialog : _firstDialog);

		_alreadyInteracted = true;
	}

	public override void ExitInteraction()
	{
		Player.SetInputReading(true);
		_interactionCam.enabled = false;
		_dialogBoxScript.Exit();
	}
}
