using UnityEngine.SceneManagement;

public interface IOnSceneLoad 
{
	/// <summary>
	/// Has to be assign to LoadSave.AddActionToScenesLoad(OnSceneLoad);
	/// </summary>
	/// <param name="scene">Loaded Scene</param>
	/// <param name="mode">Load mode</param>
	void OnSceneLoad(Scene scene, LoadSceneMode mode);
}
