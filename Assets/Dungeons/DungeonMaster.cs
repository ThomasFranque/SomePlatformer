using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dungeons;

public class DungeonMaster : MonoBehaviour
{
    DungeonInfo _dungeonInfo;

    public void Initialize(DungeonInfo dungeonInfo)
    {
        _dungeonInfo = dungeonInfo;
    }
}
