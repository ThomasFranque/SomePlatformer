using UnityEngine;

public class StarfieldScroll : MonoBehaviour
{
	[SerializeField] private float _parallaxFactor = 0f;

	private void Update()
	{
		Vector3 newPos = CameraActions.ActiveCamera.transform.position * _parallaxFactor; // Calculate the position of the object
		newPos.z = 0; // Force Z-axis to zero, since we're in 2D
		transform.position = newPos;
	}
	
}