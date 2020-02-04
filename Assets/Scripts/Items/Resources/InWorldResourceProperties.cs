using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InWorldResourceProperties : ScriptableObject
{
    [SerializeField] private Vector2 _initialBurstSpeedXRange = Vector2.zero;
    [SerializeField] private Vector2 _initialBurstSpeedYRange = Vector2.zero;
    [SerializeField] private bool _lockRotation = false;
    [SerializeField] private float _initialRotationAmount = 45.0f;
    [SerializeField] private float _lifetime = 5.0f;
    [SerializeField] private Color _spriteColor = Color.yellow;
    [SerializeField] private Sprite _sprite = null;

    public Vector2 InitialBurstSpeedXRange => _initialBurstSpeedXRange;
    public Vector2 InitialBurstSpeedYRange => _initialBurstSpeedYRange;
    public bool LockRotation => _lockRotation;
    public float InitialRotationAmount => _initialRotationAmount;
    public float Lifetime => _lifetime;
    public Color SpriteColor => _spriteColor;
    public Sprite Sprite => _sprite;
}
