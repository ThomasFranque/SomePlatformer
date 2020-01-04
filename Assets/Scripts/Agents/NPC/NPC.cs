using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactible
{
	[SerializeField] private DialogBox _dialogBoxScript = null;
	[TextArea]
	[SerializeField] private string[] _firstDialog = null;
	[TextArea]
	[SerializeField] private string[] _defaultDialog = null;

	[SerializeField]
	private Cinemachine.CinemachineVirtualCamera[] _interactionCams = null;

	private bool _alreadyInteracted = false;

    // Start is called before the first frame update
    protected override void Start()
    {
		base.Start();

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
		_interactionCams[0].enabled = true;
		_dialogBoxScript.Display(this, _interactionCams, _alreadyInteracted ? _defaultDialog : _firstDialog);

		_alreadyInteracted = true;
	}

	public override void ExitInteraction(byte camIndex = 0)
	{
		base.ExitInteraction();

		Player.SetInputReading(true);
		_interactionCams[camIndex].enabled = false;
		_dialogBoxScript.Exit();
	}
}
