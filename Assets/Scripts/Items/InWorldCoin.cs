using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InWorldCoin : InWorldItem
{
    [SerializeField] private InWorldCoinProperties _properties;
    protected override InWorldResourceProperties _BaseProperties {get => _properties; }

    protected override void OnPlayerCollision(Player p)
    {
        p.Inventory?.AddCoins(_properties.Amount);
        PickupEffect();
        Destroy(gameObject);
    }
}
