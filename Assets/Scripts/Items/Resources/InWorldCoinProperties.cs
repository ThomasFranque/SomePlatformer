using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "In World Item/Coin")]
public class InWorldCoinProperties : InWorldResourceProperties
{
    [SerializeField] private int _amount = 5;

    public int Amount => _amount;
}
