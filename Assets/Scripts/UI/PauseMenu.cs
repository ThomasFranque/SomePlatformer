using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : ControlledCanvasMenu
{
	public static bool Paused { get; private set; } = false;
	private static bool _pauseAllowed				= true;

	[Header("Pause Menu Options")]
	[SerializeField] private KeyCode _pauseKey		= KeyCode.Escape;
	[SerializeField] private bool _gameCanBePaused	= true;

	[Header("Pause Menu References")]
	[SerializeField] private TextMeshProUGUI _pauseHeaderPro	= null;
	[SerializeField] private GameObject _continueButton			= null;
	[SerializeField] private GameObject _optionsButton			= null;
	[SerializeField] private GameObject _exitButton				= null;
	[SerializeField] private GameObject _loadButton				= null;
	[SerializeField] private GameObject _saveButton				= null;
	[SerializeField] private AudioClip _menuOpenedSound 		= null;
	private GameObject _pauseMenuHolder;
	private LoadSave SaveMngr;
	private GFXBlink blinkButtonFX;
	private GFXBlink blinkHeaderFX;

	private Action PauseMenuOpen;
	private Action PauseMenuClose;

	private bool _continueBlink;
	private bool _optionsBlink;
	private bool _quitBlink;
	private bool _loadBlink;
	private bool _saveBlink;

	private bool _loadOnExit;
	private bool _saved;

	private bool ListenForPauseKey => 
		_pauseAllowed || Paused;

	private static void SetPauseALlowed(bool allowed)
	{
		_pauseAllowed = allowed;
	}

	// Start is called before the first frame update
	protected override void Start()
    {
		base.Start();

		SaveMngr = LoadSave.Instance;

		blinkButtonFX = new GFXBlink();
		blinkHeaderFX = new GFXBlink(false);

		SetPauseALlowed(_gameCanBePaused);
		_pauseMenuHolder = transform.GetChild(0).gameObject;

		PauseMenuOpen += StopGameSpeed;
		PauseMenuOpen += AssignListeners;
		PauseMenuOpen += ToggleMenu;
		PauseMenuOpen += PlayOpenSound;
		PauseMenuClose += ToggleMenu;
		PauseMenuClose += PlayOpenSound;
		PauseMenuClose += ResetBlinking;
		PauseMenuClose += RestartGameSpeed;
    }

	private void Update()
	{
		if (ListenForPauseKey)
			Listen();

		if (Paused)
			WhilePaused();
	}

	private void AssignListeners()
	{
		_continueButton.GetComponentInChildren<Button>().onClick.AddListener(ContinueClick);
		_optionsButton.GetComponentInChildren<Button>().onClick.AddListener(OptionsClick);
		_exitButton.GetComponentInChildren<Button>().onClick.AddListener(ExitClick);
		_saveButton.GetComponentInChildren<Button>().onClick.AddListener(SaveClick);
		_loadButton.GetComponentInChildren<Button>().onClick.AddListener(LoadClick);
	}

	private void Listen()
	{
		if (Input.GetKeyDown(_pauseKey))
		{
			if (Paused)
				ContinueClick();
			else
				OnPauseMenuOpen();
		}
	}
	private void WhilePaused()
	{
		MakeHeaderBlink();

		if (_continueBlink)
			MakeButtonBlink(_continueButton);
		else if (_optionsBlink)
			MakeButtonBlink(_optionsButton);
		else if (_quitBlink)
			MakeButtonBlink(_exitButton);
		else if (_loadBlink)
			MakeButtonBlink(_loadButton);
		else if (_saveBlink)
			MakeButtonBlink(_saveButton);
	}

	// Button Presses
	private void OnPauseMenuOpen()
	{
		GC.Collect();
		Paused = true;
		PauseMenuOpen?.Invoke();

	}
	private void OnPauseMenuClose()
	{
		Paused = false;
		if (_loadOnExit)
		{
			SaveMngr.Load();
			_loadOnExit = false;
		}
		_saved = false;

		PauseMenuClose?.Invoke();
	}

	private void ContinueClick()
	{
		_continueBlink = true;
		StartCoroutine(CButtonBlinkBeforeAction(OnPauseMenuClose));
	}

	private void OptionsClick()
	{
		_optionsBlink = true;
	}

	private void ExitClick()
	{
		_quitBlink = true;
		StartCoroutine(CButtonBlinkBeforeAction(ExitGame));
	}
	private void SaveClick()
	{
		if (!_saved)
		{
			_saved = true;
			SaveMngr.Save();
			StartCoroutine(CSaveClicked());
		}

	}
	private void LoadClick()
	{
		if (!_loadOnExit)
		{
			_loadOnExit = true;
			StartCoroutine(CLoadClicked());
		}
	}
	//

	// Scene management
	private void StopGameSpeed()
	{
		SetGameSpeed(0);
	}

	private void RestartGameSpeed()
	{
		SetGameSpeed(1);
	}

	private void SetGameSpeed(float speed)
	{
		Time.timeScale = speed;
	}

	private void ExitGame()
	{
		Application.Quit();
	}
	//

	// FX
	private void ToggleMenu()
	{
		_pauseMenuHolder.SetActive(Paused);
	}

	private void MakeButtonBlink(GameObject targetButton)
	{
		//targetButton.GetComponentInParent<BlinkingEffect>().StartBlink();
		blinkButtonFX.DoBlink(targetButton, 0.1f);
	}
	private void MakeHeaderBlink()
	{
		//targetButton.GetComponentInParent<BlinkingEffect>().StartBlink();
		blinkHeaderFX.DoBlink(_pauseHeaderPro, 1.4f);
	}

	private void ResetBlinking()
	{
		_continueBlink = false;
		_optionsBlink = false;
		_quitBlink = false;

		blinkButtonFX = new GFXBlink();
		blinkHeaderFX = new GFXBlink(false);

		_pauseHeaderPro.enabled = true;
		_continueButton.SetActive(true);
		_optionsButton.SetActive(true);
		_exitButton.SetActive(true);
		_saveButton.SetActive(true);
		_loadButton.SetActive(true);
	}

	private void PlayOpenSound()
	{
		PlaySound(_menuOpenedSound);
	}

	private IEnumerator CButtonBlinkBeforeAction(Action action)
	{
		_sp.PlayOneShotGeneral(_buttonPressedSound);
		yield return new WaitForSecondsRealtime(1.0f);
		action.Invoke();
	}
	private IEnumerator CSaveClicked()
	{
		_sp.PlayOneShotGeneral(_buttonPressedSound);
		_saveBlink = true;
		yield return new WaitForSecondsRealtime(0.3f);
		_saveBlink = false;
		_saveButton.SetActive(false);
	}
	private IEnumerator CLoadClicked()
	{
		_sp.PlayOneShotGeneral(_buttonPressedSound);
		_loadBlink = true;
		yield return new WaitForSecondsRealtime(1.0f);
		_loadBlink = false;
		_loadButton.SetActive(false);
		OnPauseMenuClose();
	}
	//
}
