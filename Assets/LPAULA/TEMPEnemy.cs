using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMPEnemy : MonoBehaviour
{
	private Renderer _sRenderer = null;

	[SerializeField]
	private EnemyModel _model = null;

	private void Awake()
	{
		_sRenderer = GetComponent<SpriteRenderer>();
		(_sRenderer as SpriteRenderer).sprite= _model.Sprite;
		(_sRenderer as SpriteRenderer).color = _model.RandomColor;
	}

	private void Update()
	{
		(_sRenderer as SpriteRenderer).color = _model.RandomColor;
	}
}
