using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogBox : MonoBehaviour
{
	/* WHEN CALLING ANY CUSTOM SPECIAL CHARACTER A SPACE MUST FOLLOW AFTER INSTRUCTION
	 * WHEN CALLING WAIT (\W) IT MUST ALLWAYS HAVE DECIMAL NUMBER OF 1 (EX: \W1.0 )
	 * BIGGER AND SMALLER WILL AFFECT ALL TEXT
	 * SKIP WAIT WILL REMAIN ACTIVE UNTIL NEXT DIALOGUE IF NOT CALLED AGAIN
	*/

	private const char _SPECIAL_CHAR = '\\';
	private const char _TMPRO_SPECIAL_CHAR_START = '<';
	private const char _TMPRO_SPECIAL_CHAR_END = '>';

	private const char _TIME_WAIT_CHAR = 'W';
	private const char _BIGGER_TEXT_SIZE_CHAR = 'B';
	private const char _SMALLER_TEXT_SIZE_CHAR = 'L';
	private const char _CAM_SHAKE_CHAR = 'S';
	private const char _NEXT_CAM_CHAR = 'C';
	private const char _FIRST_CAM_CHAR = 'c';
	private const char _SKIP_WAIT_CHAR = '-';
	private const char _AUTO_NEXT_CHAR = 'N';

	private const float _SLOW_WRITE_SPEED = .06f; //secs
	private const float _TEXT_SPEED_UP_AMMOUNT = 2.5f;

	private Animator _anim;
	private TextMeshPro _dialogTextPro;
	private Coroutine _dialogCor;
	private Cinemachine.CinemachineVirtualCamera[] _cams;
	private float _defaultTextSize;
	private byte _activeCamIndex;

	// Start is called before the first frame update
	private void Start()
	{
		_anim = GetComponent<Animator>();
		_dialogTextPro = GetComponentInChildren<TextMeshPro>();
		_dialogCor = null;
		_activeCamIndex = 0;
		_defaultTextSize = _dialogTextPro.fontSize;
	}

	public void Display(Interactible interactable, Cinemachine.CinemachineVirtualCamera[] cams = null, string[] dialog = null)
	{
		_cams = cams;

		if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Display"))
			_anim.SetTrigger("Display");

		if (dialog != null)
			DisplayDialog(interactable, dialog);

	}

	public void Exit()
	{
		if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Hide"))
			_anim.SetTrigger("Hide");
	}

	private void DisplayDialog(Interactible interactable, string[] dialog)
	{
		_dialogTextPro.text = "";

		_dialogCor = StartCoroutine(CDialogTxt(interactable, dialog));

	}

	private float ExtractTimeFromText(string word, uint index)
	{
		// https://stackoverflow.com/questions/1354924/how-do-i-parse-a-string-with-a-decimal-point-to-a-double
		//string invariantWord = word.ToString(CultureInfo.CurrentCulture);
		return float.Parse(word.Substring((int)index + 2, 3), CultureInfo.InvariantCulture);
		
	}

	private IEnumerator CDialogTxt(Interactible interactable, string[] dialog)
	{
		// Go through all dialog
		for (byte i = 0; i < dialog.Length; i++)
		{
			string[] words = dialog[i].Split(' ');
			bool wait = true; // used to auto skip (\-)
			bool autoNext = false; // used to automatically move to next dialog
			// Go through all words in single dialog
			for (uint j = 0; j < words.Length; j++)
			{
				bool specialAction = false;
				// Go  through all chars
				for (uint k = 0; k < words[j].Length; k++)
				{
					if (words[j][(int)k] == _SPECIAL_CHAR)
					{
						switch (words[j][(int)k + 1])
						{
							case _BIGGER_TEXT_SIZE_CHAR:
								_dialogTextPro.fontSize += 12;
								break;
							case _SMALLER_TEXT_SIZE_CHAR:
								_dialogTextPro.fontSize -= 12;
								break;
							case _TIME_WAIT_CHAR:
								yield return new WaitForSeconds(ExtractTimeFromText(words[j], k));
								break;
							case _SKIP_WAIT_CHAR:
								wait = !wait;
								break;
							case _CAM_SHAKE_CHAR:
								CameraActions.ActiveCamera?.Shake();
								Debug.Log(CameraActions.ActiveCamera.gameObject.name);
								break;
							case _NEXT_CAM_CHAR:
								NextCam();
								break;
							case _FIRST_CAM_CHAR:
								ResetCam();
								break;
							case _AUTO_NEXT_CHAR:
								autoNext = true;
								break;
							default:
								Debug.LogWarning($"UNKNOWN TEXT COMMAND: {_SPECIAL_CHAR}{words[j][(int)k + 1]}" );
								break;
						}

						// Skip next space
						specialAction = true;
					}
					else if (words[j][(int)k] == _TMPRO_SPECIAL_CHAR_START)
					{
						for (uint m = 0; m < words[j].Length && words[j][(int)(k + m)] != _TMPRO_SPECIAL_CHAR_END; k++)
						{
							_dialogTextPro.text += words[j][(int)(k + m)];
						}
						_dialogTextPro.text += _TMPRO_SPECIAL_CHAR_END;
						_dialogTextPro.text += ' ';

						// skip
						specialAction = true;
					}
					if (!specialAction)
					{
						if (wait)
							yield return new WaitForSeconds(Input.anyKey ? _SLOW_WRITE_SPEED / _TEXT_SPEED_UP_AMMOUNT : _SLOW_WRITE_SPEED);
						_dialogTextPro.text += words[j][(int)k];
					}
				}

				if (!specialAction)
					_dialogTextPro.text += ' ';
			}

			if (!autoNext)
				yield return WaitForKeyPress();

			if (i == dialog.Length - 1)
			{
				interactable.ExitInteraction(_activeCamIndex);
			}
			_dialogTextPro.fontSize = _defaultTextSize;
			_dialogTextPro.text = "";
			_activeCamIndex = 0;
			// Next dialog screen (clear previous text)
		}

		yield return null;
	}

	private void NextCam()
	{
		_cams[_activeCamIndex].enabled = false;

		_activeCamIndex++;
		if (_activeCamIndex > _cams.Length - 1)
			_activeCamIndex = 0;

		// Check if it has a target
		if (_cams[_activeCamIndex].Follow != null)
		{
			// Check if target is not active
			if (!_cams[_activeCamIndex].Follow.gameObject.activeSelf)
				NextCam();
			// Target is active
			else
				_cams[_activeCamIndex].enabled = true;
		}
		else 
			_cams[_activeCamIndex].enabled = true;
	}

	private void ResetCam()
	{
		_cams[_activeCamIndex].enabled = false;

		_activeCamIndex = 0;

		_cams[_activeCamIndex].enabled = true;
	}

	private IEnumerator WaitForKeyPress(KeyCode key = KeyCode.None)
	{
		bool done = false;
		while (!done) // essentially a "while true", but with a bool to break out naturally
		{
			if (key == KeyCode.None)
			{
				if (Input.anyKeyDown)
				{
					done = true; // breaks the loop
				}
			}
			else
				if (Input.GetKeyDown(key))
			{
				done = true; // breaks the loop
			}
			yield return null; // wait until next frame, then continue execution from here (loop continues)
		}
	}
}