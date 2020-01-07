using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSave : MonoBehaviour
{
	public static LoadSave Instance { get; private set; }
	private static GameState _currentSavedGS;
	private static bool _loadSave;

	private int CurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;

	private void Awake()
	{
		if (Instance == null)
		{
			InitializeSingleton();
		}
		else
			Destroy(gameObject);

	}

	private void InitializeSingleton()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);
		AddActionToScenesLoad(OnLevelFinishedLoading);
	}

	public void Load()
	{
		_loadSave = true;
		_currentSavedGS = SaveFile.GetSavedGameState();

		// EVENTUALLY LOAD SAVED SCENE // Will call OnLevelFinishedLoading()
		LoadScene(CurrentSceneIndex);
	}

	public void Save()
	{
		SaveFile.SaveCurrentGameState();
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("Level Loaded: " + scene.name + "\nLoad Mode: " + mode);

		if (_loadSave)
			LoadSavedInfo();
	}

	private void LoadSavedInfo()
	{
		_loadSave = false;
		Player.Instance.HP = _currentSavedGS.PlayerHP > 0 ? _currentSavedGS.PlayerHP : 3;
		Player.Instance.transform.position = _currentSavedGS.PlayerPos;
	}

	private void LoadScene(int index)
	{
		SceneManager.LoadScene(index);
	}

	public static void AddActionToScenesLoad(UnityEngine.Events.UnityAction<Scene, LoadSceneMode> action)
	{
		SceneManager.sceneLoaded += action;



	}
	public static void RemoveActionFromScenesLoad(UnityEngine.Events.UnityAction<Scene, LoadSceneMode> action)
	{
		SceneManager.sceneLoaded -= action;
	}
}
