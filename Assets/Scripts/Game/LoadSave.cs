using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSave : MonoBehaviour
{
	public static LoadSave Instance { get; private set; }
	private static GameState _currentSavedGS;
	private static bool _loadSave;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			SceneManager.sceneLoaded += OnLevelFinishedLoading;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);
	}

	public void Load()
	{
		_loadSave = true;
		_currentSavedGS = SaveFile.GetSavedGameState();

		// EVENTUALLY LOAD SAVED SCENE // Will call OnLevelFinishedLoading()
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void Save()
	{
		SaveFile.SaveCurrentGameState();
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("Level Loaded: " + scene.name + " - " + mode);

		if (_loadSave)
			LoadSavedInfo();
	}

	private void LoadSavedInfo()
	{
		_loadSave = false;
		Player.Instance.HP = _currentSavedGS.PlayerHP > 0 ? _currentSavedGS.PlayerHP : 3;
		Player.Instance.transform.position = _currentSavedGS.PlayerPos;
	}
}
