using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// from
// http://guidohenkel.com/2018/05/endless_starfield_unity/
public class Starfield : MonoBehaviour
{
	[SerializeField] private int _maxStars = 100;
	[SerializeField] private float _starSize = 0.1f;
	[SerializeField] private float _starSizeRange = 0.5f;
	[SerializeField] private bool _colorize = false;
	[SerializeField] private Vector2 _fieldSize = new Vector2(20.0f, 25.0f);

	float xOffset;
	float yOffset;

	ParticleSystem _particles;
	ParticleSystem.Particle[] _stars;

	private void Awake()
	{
		_stars = new ParticleSystem.Particle[_maxStars];
		_particles = GetComponent<ParticleSystem>();

		Assert.IsNotNull(_particles, "Particle system missing from object!");

		xOffset = _fieldSize.x * 0.5f;                                                                                                        // Offset the coordinates to distribute the spread
		yOffset = _fieldSize.y * 0.5f;                                                                                                       // around the object's center

		for (int i = 0; i < _maxStars; i++)
		{
			float randSize = Random.Range(_starSizeRange, _starSizeRange + 1f);                       // Randomize star size within parameters
			

			_stars[i].position = GetRandomInRectangle(_fieldSize.x, _fieldSize.y) + transform.position;
			_stars[i].startSize = _starSize * randSize;
			//_stars[i].startColor = new Color(1f, scaledColor, scaledColor, 1f);
			_stars[i].startColor = _colorize ? Random.ColorHSV() : Color.white;
		}
		_particles.SetParticles(_stars, _stars.Length);                                                                // Write data to the particle system
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < _maxStars; i++)
		{
			Vector3 pos = _stars[i].position + transform.position;

			if (pos.x < (CameraActions.ActiveCamera.transform.position.x - xOffset))
			{
				pos.x += _fieldSize.x;
			}
			else if (pos.x > (CameraActions.ActiveCamera.transform.position.x + xOffset))
			{
				pos.x -= _fieldSize.x;
			}

			if (pos.y < (CameraActions.ActiveCamera.transform.position.y - yOffset))
			{
				pos.y += _fieldSize.y;
			}
			else if (pos.y > (CameraActions.ActiveCamera.transform.position.y + yOffset))
			{
				pos.y -= _fieldSize.y;
			}

			_stars[i].position = pos - transform.position;
		}
		_particles.SetParticles(_stars, _stars.Length);
	}

	// Get a random value within a certain rectangle area
	Vector3 GetRandomInRectangle(float width, float height)
	{
		float x = Random.Range(0, width);
		float y = Random.Range(0, height);
		return new Vector3(x - xOffset, y - yOffset, 0);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.grey;
		Gizmos.DrawWireCube(transform.position, _fieldSize);
	}
}
