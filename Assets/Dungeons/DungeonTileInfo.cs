using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeons
{
    public struct DungeonTileInfo
    {
        public readonly Vector3Int PositionOnGrid;
        private readonly Tilemap _instertedTilemap;
        public readonly bool IsGround;
        public readonly bool IsRoof;
        public readonly bool IsLeftWall;
        public readonly bool IsRightWall;

        public DungeonTileInfo(Vector3Int gridPosition, Tilemap insertedTilemap, bool isLeftWall, bool isRightWall, bool isRoof, bool isGround )
        {
            _instertedTilemap = insertedTilemap;
            PositionOnGrid = gridPosition;
            IsGround = isGround;
            IsRoof = isRoof;
            IsRightWall = isRightWall;
            IsLeftWall = isLeftWall;
        }
    }
}
