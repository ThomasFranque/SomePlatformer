using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InWorldCoin : InWorldItem
{
    [SerializeField] private InWorldCoinProperties _properties;
    protected override InWorldResourceProperties _BaseProperties {get => _properties; }
}
