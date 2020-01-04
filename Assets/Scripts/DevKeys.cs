using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevKeys : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			Debug.Log("DEV RELOAD");
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
        else if (Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log("DEV QUIT");
			Application.Quit();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Debug.Log("DEV CAM SHAKE");
			CameraActions.ActiveCamera.Shake();
		}
    }
}
