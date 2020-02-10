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
        private List<DungeonTileInfo> _roomTiles;
        private bool _isMainPath;
        private bool _isPathEntry;
        private bool _isPathEnd;

        private void Awake()
        {
        }

        public void Initialize(bool[] openings, Vector2Int roomIndex, Tilemap tilemap, DungeonInfo dungeonInfo, bool isMainPath, bool isPathStart, bool isPathEnd)
        {
            _roomTiles = new List<DungeonTileInfo>(_dungeonInfo.RoomSize.x * _dungeonInfo.RoomSize.y);
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
                    if (pivot.x == bottomLeft.x && !_openings[0])
                        SpawnTile(pivot, _dungeonInfo.DungeonTile, onLeftWall: true);
                    else if (pivot.x == bottomLeft.x + _dungeonInfo.RoomSize.x - 1 && !_openings[1])
                        SpawnTile(pivot, _dungeonInfo.DungeonTile, onRightWall: true);
                    else if (pivot.y == bottomLeft.y && !_openings[3])
                        SpawnTile(pivot, _dungeonInfo.DungeonTile, onBottomWall: true);
                    else if (pivot.y == bottomLeft.y + _dungeonInfo.RoomSize.y - 1 && !_openings[2])
                        SpawnTile(pivot, _dungeonInfo.DungeonTile, onTopWall: true);

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
                pivot.x++;
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

        private void SpawnTile(Vector2Int position, TileBase tile, bool onLeftWall = false, bool onRightWall = false, bool onTopWall = false, bool onBottomWall = false)
        {
            _tilemap.SetTile((Vector3Int)position, tile);
            _roomTiles.Add(new DungeonTileInfo((Vector3Int)position, _tilemap, onLeftWall, onRightWall, onTopWall, onBottomWall));
        }

        public void Populate(bool forcePopulate = false)
        {
            int rnd = Random.Range(0, 4);
            do { rnd = Random.Range(1, 4); } while (rnd == 0 && forcePopulate);

            Vector3Int roomCenter = new Vector3Int(_roomIndex.x * _dungeonInfo.RoomSize.x, _roomIndex.y * _dungeonInfo.RoomSize.y, 0);

            switch (rnd)
            {
                case 0:
                    break;
                case 1:
                    SpawnObjectInCell(roomCenter, _dungeonInfo.GetRandomGroundEnemy);
                    break;
                case 2:
                    SpawnObjectInCell(roomCenter, _dungeonInfo.GetRandomAerialEnemy);
                    break;
                case 3:
                    SpawnObjectInCell(roomCenter, _dungeonInfo.GetRandomHazard);
                    break;

            }
        }

        public GameObject SpawnObjectInCell(Vector3Int cell, GameObject objectToSpawn)
        {
            return GameObject.Instantiate(
                objectToSpawn, 
                _tilemap.layoutGrid.CellToWorld(cell), 
                Quaternion.identity);
        }
    }
}
