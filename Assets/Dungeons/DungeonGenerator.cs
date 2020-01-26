using System;
using UnityEngine;

namespace Dungeons
{

    public class DungeonGenerator : MonoBehaviour
    {
        private const int _CELLS_PER_ROOM_H = 30;
        private const int _CELLS_PER_ROOM_V = 16;
        [SerializeField] private DungeonInfo _dungeonInfo;
        [SerializeField] private Transform _roomsTransform;

        private DungeonRoomInfo[,] _rooms;

        // Local world pivot position
        private Vector2 _roomPivot;

        // The calculated world scaled room size
        private Vector2 _totalRoomSize;

        private Vector2 _pivotStartPos;

        private Vector2Int _roomIndex;

        // Start is called before the first frame update
        void Awake()
        {
            _roomPivot = _pivotStartPos = transform.position;
            _rooms = new DungeonRoomInfo[_dungeonInfo.Size.x,_dungeonInfo.Size.y];

            AssignActions();
            GrabTotalRoomSize();
            SpawnRooms();
            //Re-checkRooms();
        }

        private void AssignActions()
        {
            // What happens when a room is supposed to spawn
            NewRoom += RandomRoomInPivotPosition;

            // Keeps track of Y pivot
            if (_dungeonInfo.Ascending) NewRoomRow += IncrementPivotY;
            else NewRoomRow += DecrementPivotY;
            NewRoomRow += ResetPivotX;

            // Spawns room and keeps track of X pivot
            NewRoomColumn += OnNewRoom;
            if (_dungeonInfo.Backwards) NewRoomColumn += DecrementPivotX;
            else NewRoomColumn += IncrementPivotX;
        }

        private void GrabTotalRoomSize()
        {
            Grid grid = GetComponentInChildren<Grid>();

            Vector2 gridCellGap = grid.cellGap;
            Vector2 gridCellSize = grid.cellSize;

            _totalRoomSize = new Vector2(
                (gridCellSize.x * _CELLS_PER_ROOM_H) + gridCellGap.x,
                (gridCellSize.y * _CELLS_PER_ROOM_V) + gridCellGap.y);
        }

        private void SpawnRooms()
        {
            for (int y = 0; y < _dungeonInfo.Size.y; y++)
            {
                _roomIndex.y = y;

                for (int x = 0; x < _dungeonInfo.Size.x; x++)
                {
                    _roomIndex.x = x;
                    // New Column in row
                    OnNewRoomColumn();
                }

                // New row
                OnNewRoomRow();
            }
        }

        private void RandomRoomInPivotPosition()
        {
            DungeonRoomInfo[] neighbors = GetNeighbors();
            DungeonRoomInfo newRoomInfo;

            int maxLoops = _dungeonInfo.Size.x * _dungeonInfo.Size.y * 2;
            int loops = 0;

            do
            {
                newRoomInfo = _dungeonInfo.RandomRoomInfo;
                loops++;
            } while (!DungeonRoomInfo.CanBePlaced(newRoomInfo, neighbors) && loops < maxLoops);

            if (loops >= maxLoops) newRoomInfo = _dungeonInfo.BlockedRoom;
            _rooms[_roomIndex.x, _roomIndex.y] = newRoomInfo;

            Instantiate(
                newRoomInfo.RoomPrefab,
                _roomPivot, Quaternion.identity,
                _roomsTransform);
        }

        private DungeonRoomInfo[] GetNeighbors()
        {
            DungeonRoomInfo[] neighbors = new DungeonRoomInfo[4];

            neighbors[0] = _roomIndex.x - 1 >= 0 ? _rooms[_roomIndex.x - 1, _roomIndex.y] : null;
            neighbors[1] = _roomIndex.x + 1 < _dungeonInfo.Size.x ? _rooms[_roomIndex.x + 1, _roomIndex.y] : null;
            neighbors[2] = _roomIndex.y + 1 < _dungeonInfo.Size.y ? _rooms[_roomIndex.x, _roomIndex.y + 1] : null;
            neighbors[3] = _roomIndex.y - 1 >= 0 ? _rooms[_roomIndex.x, _roomIndex.y - 1] : null;

            return neighbors;
        }

        private void IncrementPivotX()
        {
            _roomPivot.x += _totalRoomSize.x;
        }

        private void DecrementPivotX()
        {
            _roomPivot.x -= _totalRoomSize.x;
        }

        private void DecrementPivotY()
        {
            _roomPivot.y -= _totalRoomSize.y;
        }

        private void IncrementPivotY()
        {
            _roomPivot.y += _totalRoomSize.y;
        }

        private void ResetPivotX()
        {
            _roomPivot.x = _pivotStartPos.x;
        }


        private void OnNewRoomRow()
        {
            NewRoomRow?.Invoke();
        }
        private void OnNewRoomColumn()
        {
            NewRoomColumn?.Invoke();
        }

        private void OnNewRoom()
        {
            NewRoom?.Invoke();
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(_roomPivot, 1.0f);
        }

        private Action NewRoom;
        private Action NewRoomRow;
        private Action NewRoomColumn;
    }
}