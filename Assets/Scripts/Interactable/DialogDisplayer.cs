using System.Collections;
using UnityEngine;

public class DialogDisplayer : Interactible
{
	[SerializeField] private DialogBox _dialogBoxScript = null;
	[SerializeField] private float _delayOnTextDisplay = 0.0f;
	[SerializeField] private bool _cameraChangeOnInteraction = true;
	[SerializeField] private bool _hidePlayerUI = true;
	[TextArea]
	[SerializeField] private string[] _firstDialog = null;
	[TextArea]
	[SerializeField] private string[] _defaultDialog = null;

	[SerializeField]
	private Cinemachine.CinemachineVirtualCamera[] _interactionCams = null;

	[Header("Optional Fields")]
	[SerializeField]
	private InteractionListener _interactionListener;
	[SerializeField] private SoundClips _sounds;

	private MainCanvas _mainCanvas;
	private bool _alreadyInteracted = false;

    // Start is called before the first frame update
    protected override void Start()
    {
		base.Start();
		AddActionToInteraction(OnInteraction);

		if (_defaultDialog.Length == 0)
			_defaultDialog = _firstDialog;

		_mainCanvas = GameObject.Find("CANVAS").GetComponent<MainCanvas>();
	}	

    // Update is called once per frame
    protected override void Update()
    {
		base.Update();
    }

	private void OnInteraction()
	{
		Player.SetInteractionInputReading(false);

		if (_cameraChangeOnInteraction)
			_interactionCams[0].enabled = true;

		if(_hidePlayerUI)
			_mainCanvas.PlayerUIScript.HideUI();

		_interactionListener?.InteractionStarted();

		StartCoroutine(CTextDelay());
		
	}

	private IEnumerator CTextDelay()
	{
		yield return new WaitForSeconds(_delayOnTextDisplay);

		if (!_cameraChangeOnInteraction)
			_interactionCams[0].enabled = true;

		DisplayText();
	}

	private void DisplayText()
	{
		_dialogBoxScript.Display(this, _interactionCams, _alreadyInteracted ? _defaultDialog : _firstDialog, sounds);
		_alreadyInteracted = true;
	}

	public override void ExitInteraction(byte camIndex = 0)
	{
		base.ExitInteraction();

		Player.SetInteractionInputReading(true);

		if (_hidePlayerUI)
			_mainCanvas.PlayerUIScript.UnHideUI();

		foreach(Cinemachine.CinemachineVirtualCamera c in _interactionCams)
			c.enabled = false;

		//_interactionCams[camIndex].enabled = false;
		_dialogBoxScript.Exit();

		_interactionListener?.InteractionFinished();
	}
}
