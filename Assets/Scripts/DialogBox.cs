using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogBox : MonoBehaviour
{
	private const char _SPECIAL_CHAR = '\\';
	private const char _TIME_WAIT_CHAR = 'W';
	private const char _BIGGER_TEXT_SIZE_CHAR = 'B';
	private const char _SMALLER_TEXT_SIZE_CHAR = 'L';
	private const float _SLOW_WRITE_SPEED = .06f; //secs

	private const float _TEXT_SPEED_UP_AMMOUNT = 2.5f;

	private Animator _anim;
	private TextMeshPro _dialogTextPro;
	private Coroutine _dialogCor;
	private float _defaultTextSize;

	// Start is called before the first frame update
	private void Start()
	{
		_anim = GetComponent<Animator>();
		_dialogTextPro = GetComponentInChildren<TextMeshPro>();
		_dialogCor = null;
		_defaultTextSize = _dialogTextPro.fontSize;
	}

	public void Display(Interactable interactable, string[] dialog = null)
	{
		_anim.SetTrigger("Display");

		if (dialog != null)
			DisplayDialog(interactable, dialog);
	}

	public void Exit()
	{
		_anim.SetTrigger("Hide");
	}

	private void DisplayDialog(Interactable interactable, string[] dialog)
	{
		_dialogTextPro.text = "";

		_dialogCor = StartCoroutine(CDialogTxt(interactable, dialog));

	}

	private float ExtractTimeFromText(string word)
	{
		return float.Parse(word.Substring(2));
	}

	private IEnumerator CDialogTxt(Interactable interactable, string[] dialog)
	{
		// Go through all text
		for (byte i = 0; i < dialog.Length; i++)
		{
			string[] words = dialog[i].Split(' ');
			// Go through all words
			for (uint j = 0; j < words.Length; j++)
			{
				bool skipSpace = false;
				// Check for special stuff time
				if (words[j].StartsWith(_SPECIAL_CHAR.ToString()))
				{
					if (words[j][1] == _BIGGER_TEXT_SIZE_CHAR)
						_dialogTextPro.fontSize += 12;
					else if (words[j][1] == _SMALLER_TEXT_SIZE_CHAR)
						_dialogTextPro.fontSize -= 12;
					else if (words[j][1] == _TIME_WAIT_CHAR)
						yield return new WaitForSeconds(Input.anyKey ? ExtractTimeFromText(words[j]) / _TEXT_SPEED_UP_AMMOUNT : ExtractTimeFromText(words[j]));
					// Skip next char
					skipSpace = true;
				}
				else
					// Go  through all chars
					for (uint k = 0; k < words[j].Length; k++)
					{
						yield return new WaitForSeconds(Input.anyKey ? _SLOW_WRITE_SPEED/ _TEXT_SPEED_UP_AMMOUNT : _SLOW_WRITE_SPEED);
						_dialogTextPro.text += words[j][(int)k];
					}

				if (!skipSpace)
					_dialogTextPro.text += ' ';
			}
			yield return WaitForKeyPress();

			if (i == dialog.Length - 1)
				interactable.ExitInteraction();
			_dialogTextPro.fontSize = _defaultTextSize;
			_dialogTextPro.text = "";
			// Next dialog screen (clear previous text)
		}

		yield return null;
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
