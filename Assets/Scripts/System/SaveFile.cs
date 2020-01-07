using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveFile
{
	private const char _CHAR_DIVISOR = ' ';
	private const string _SAVE_DIR_NAME = "SomePlatformerDEV";
	private const string _PLAYER_FILE_NAME = "Player.sav";
	private const string _WORLD_FILE_NAME = "World.sav";
	private const string _INFO_FILE_NAME = "Info.sav";
	private static readonly string _dirPath;

	private static float _savedRuntime = 0.0f;

	static SaveFile()
	{
		// Merge all paths
		_dirPath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
			_SAVE_DIR_NAME);

		// If save directory does not exist
		if (!SaveDirExists())
			CreateSaveFiles();
	}

	public static void SaveCurrentGameState()
	{
		GameState _currentGS = ProcessCurrentGameState();

		SavePlayerState(_currentGS);
		SaveWorldState(_currentGS);
		SaveInfoState(_currentGS);

		GC.Collect();
		Debug.LogWarning("GAME SAVED");
	}

	public static GameState GetSavedGameState()
	{
		// Get full path
		string fullPlayerPath = Path.Combine(_dirPath, _PLAYER_FILE_NAME);
		string fullWorldPath = Path.Combine(_dirPath, _WORLD_FILE_NAME);
		string fullInfoPath = Path.Combine(_dirPath, _INFO_FILE_NAME);

		string[] splitLine;

		Vector2 pPos = default;
		int pHP = default;
		float runTime = default;

		ReadAndAct(fullPlayerPath, ProcessPlayerLines);
		ReadAndAct(fullWorldPath, ProcessWorldLines);
		ReadAndAct(fullInfoPath, ProcessInfoLines);

		GC.Collect();

		Debug.LogWarning("SAVED GAME READ");

		return new GameState(pPos, pHP, runTime);

		void ReadAndAct(string path, Action<string,byte> actionPerLine)
		{
			// Open Player file
			using (StreamReader ScoreSR = new StreamReader(path))
			{
				string line;
				byte index = 0;
				// Read every line
				while ((line = ScoreSR.ReadLine()) != null)
				{
					actionPerLine.Invoke(line, index);
					index++;
				}
				ScoreSR.Close();
			}
		}
		void ProcessPlayerLines(string line, byte lineIndex)
		{
			line = line.ToString(CultureInfo.InvariantCulture);
			// Get position
			if (lineIndex == 0)
			{
				// Split [1] [3]
				splitLine = line.Split(' ', ',', '(', ')');
				pPos.x = float.Parse(splitLine[1]);
				pPos.y = float.Parse(splitLine[3]);
			}
			// Get hp
			else if (lineIndex == 1)
			{
				splitLine = line.Split('\n');
				pHP = int.Parse(splitLine[0]);
			}
		}
		void ProcessWorldLines(string line, byte lineIndex)
		{
			// Nothing yeet
		}
		void ProcessInfoLines(string line, byte lineIndex)
		{
			splitLine = line.Split('\n');
			runTime = float.Parse(splitLine[0]);
		}
	}

	private static GameState ProcessCurrentGameState()
	{
		Vector2 pPos = Player.Instance.transform.position;
		int pHP = Player.Instance.HP;
		float runtime = Time.realtimeSinceStartup;

		return new GameState(pPos, pHP, runtime);
	}

	private static void SavePlayerState(GameState currentGS)
	{
		string fullPlayerPath = Path.Combine(_dirPath, _PLAYER_FILE_NAME);

		string finalTxt = "";
		finalTxt = currentGS.PlayerPos.ToString();
		finalTxt += '\n';
		finalTxt += currentGS.PlayerHP;

		File.WriteAllText(fullPlayerPath, finalTxt);
	}

	private static void SaveWorldState(GameState currentGS)
	{
		string fullWorldPath = Path.Combine(_dirPath, _WORLD_FILE_NAME);
		string finalTxt = "";
		File.WriteAllText(fullWorldPath, finalTxt);
	}

	// MISSING ADDITION
	private static void SaveInfoState(GameState currentGS)
	{
		string fullInfoPath = Path.Combine(_dirPath, _INFO_FILE_NAME);
		string finalTxt = "";
		finalTxt = (currentGS.RunTime + _savedRuntime).ToString();
		File.WriteAllText(fullInfoPath, finalTxt);
	}

	private static bool SaveDirExists() =>
		Directory.Exists(_dirPath);

	private static void CreateSaveFiles()
	{
		Directory.CreateDirectory(_dirPath);

		string fullPlayerPath = Path.Combine(_dirPath, _PLAYER_FILE_NAME);
		string fullWorldPath = Path.Combine(_dirPath, _WORLD_FILE_NAME);
		string fullInfoPath = Path.Combine(_dirPath, _INFO_FILE_NAME);
		string finalTxt = "N";

		File.WriteAllText(fullPlayerPath, finalTxt);
		File.WriteAllText(fullWorldPath, finalTxt);
		finalTxt = "0";
		File.WriteAllText(fullInfoPath, finalTxt);
	}
}

