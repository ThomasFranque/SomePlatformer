using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeons
{
    public class DungeonRoom
    {
        private bool[] _openings;
        private Vector2Int _roomIndex;
        private Tilemap _tilemap;
        private DungeonInfo _dungeonInfo;

        private bool _isMainPath;
        private bool _isPathEntry;
        private bool _isPathEnd;

        private void Awake()
        {
        }

        public void Initialize(bool[] openings, Vector2Int roomIndex, Tilemap tilemap, DungeonInfo dungeonInfo, bool isMainPath, bool isPathStart, bool isPathEnd)
        {
            _openings = openings;
            _roomIndex = roomIndex;
            _tilemap = tilemap;
            _dungeonInfo = dungeonInfo;
            _isMainPath = isMainPath;
            _isPathEntry = isPathStart;
            _isPathEnd = isPathEnd;
        }

        public void SpawnRoom()
        {
            SpawnRoomSurrounds();
        }

        private void SpawnRoomSurrounds()
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
        }

        public void Populate(bool forcePopulate = false)
        {
            int rnd = Random.Range(0,4);
            Vector3Int roomCenter = new Vector3Int(_roomIndex.x * _dungeonInfo.RoomSize.x, _roomIndex.y * _dungeonInfo.RoomSize.y, 0);

            switch(rnd)
            {
                case 0:
                    break;
                case 1:
                    GameObject.Instantiate(_dungeonInfo.GetRandomGroundEnemy, _tilemap.layoutGrid.CellToWorld(roomCenter), Quaternion.identity);
                    break;
                case 2:
                    GameObject.Instantiate(_dungeonInfo.GetRandomAerialEnemy, _tilemap.layoutGrid.CellToWorld(roomCenter), Quaternion.identity);
                    break;
                case 3:
                    GameObject.Instantiate(_dungeonInfo.GetRandomHazard, _tilemap.layoutGrid.CellToWorld(roomCenter), Quaternion.identity);
                    break;

            }
        }
    }
}
