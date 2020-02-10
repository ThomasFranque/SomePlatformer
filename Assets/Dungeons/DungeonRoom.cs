using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeons
{
    public class DungeonRoom
    {
        //lrtb
        private bool[] _openings;
        private Vector2Int _roomIndex;
        private Tilemap _tilemap;
        private DungeonInfo _dungeonInfo;

        private void Awake()
        {
        }

        public void Initialize(bool[] openings, Vector2Int roomIndex, Tilemap tilemap, DungeonInfo dungeonInfo)
        {
            _openings = openings;
            _roomIndex = roomIndex;
            _tilemap = tilemap;
            _dungeonInfo = dungeonInfo;
        }

        public void SpawnRoom()
        {
            // Get bottom left
            Vector2Int bottomLeft = new Vector2Int(_roomIndex.x * _dungeonInfo.RoomSize.x, _roomIndex.y * _dungeonInfo.RoomSize.y);
            bottomLeft.x = bottomLeft.x - _dungeonInfo.RoomSize.x / 2;
            bottomLeft.y = bottomLeft.y - _dungeonInfo.RoomSize.y / 2;

            Vector2Int pivot = bottomLeft;

            for (int y = 0; y < _dungeonInfo.RoomSize.y; y++)
            {
                for (int x = 0; x < _dungeonInfo.RoomSize.x; x++)
                {
                    if ((pivot.x == bottomLeft.x && !_openings[0]) ||
                        (pivot.x == bottomLeft.x + _dungeonInfo.RoomSize.x - 1 && !_openings[1]) ||
                        (pivot.y == bottomLeft.y && !_openings[3]) ||
                        (pivot.y == bottomLeft.y + _dungeonInfo.RoomSize.y - 1 && !_openings[2]))
                            SpawnTile(pivot, _dungeonInfo.DungeonTile);

                    pivot.x++;
                }
                pivot.y++;
                pivot.x = bottomLeft.x;
            }

            // Left
            if (_openings[0])
            {
                pivot = bottomLeft;
                pivot.y += _dungeonInfo.RoomSize.y / 2;
                SpawnTile(pivot, _dungeonInfo.DungeonTile);
                pivot.x ++;
                SpawnTile(pivot, _dungeonInfo.DungeonTile);
                pivot = bottomLeft;
            }
            //Right
            if (_openings[1])
            {
                pivot = bottomLeft;
                pivot.y += _dungeonInfo.RoomSize.y / 2;
                pivot.x += _dungeonInfo.RoomSize.x;
                SpawnTile(pivot, _dungeonInfo.DungeonTile);
                pivot.x--;
                SpawnTile(pivot, _dungeonInfo.DungeonTile);
                pivot = bottomLeft;                
            }
            // top
            if (_openings[2])
            {
                pivot = bottomLeft;
                pivot.y += _dungeonInfo.RoomSize.y;
                pivot.x += _dungeonInfo.RoomSize.x / 2;
                SpawnTile(pivot, _dungeonInfo.DungeonTile);
                pivot.y--;
                SpawnTile(pivot, _dungeonInfo.DungeonTile);
                pivot = bottomLeft;                
            }
            // bottom
            if (_openings[3])
            {
                pivot = bottomLeft;
                pivot.x += _dungeonInfo.RoomSize.x / 2;
                SpawnTile(pivot, _dungeonInfo.DungeonTile);
                pivot.y++;
                SpawnTile(pivot, _dungeonInfo.DungeonTile);
                pivot = bottomLeft;
            }
        }

        private void SpawnTile(Vector2Int position, TileBase tile)
        {
            _tilemap.SetTile((Vector3Int)position, tile);
            //_tilemap.GetTile()
        }
    }
}
