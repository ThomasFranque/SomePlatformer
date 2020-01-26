using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCheckpoint : Interactible
{
    private void Awake()
    {
        AddActionToInteraction(SaveCheckpoint);
    }

    private void SaveCheckpoint()
    {
        Debug.Log("Bruh");
        LoadSave.Instance.Save();
        Destroy(this);
    }
}
