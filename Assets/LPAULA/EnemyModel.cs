using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LP2/EnemyModel")]
public class EnemyModel : ScriptableObject
{
	[SerializeField]
	private Sprite _sprite;
	[SerializeField]
	private Color _color;

	public Sprite Sprite => _sprite;
	public Color Color => _color;
	public Color RandomColor 
	{ 
		get
		{
			Color newColor = Random.ColorHSV();
			newColor.a = 250;
			return newColor;
		}

	}

}
