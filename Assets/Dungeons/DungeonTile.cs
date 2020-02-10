﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonTile : TileBase
{
    public Sprite Sprite{ get; set; }
 
    // Docs: https://docs.unity3d.com/ScriptReference/Tilemaps.TileBase.GetTileData.html
 
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = Sprite;
        tileData.colliderType = Tile.ColliderType.Grid;
    }
}
