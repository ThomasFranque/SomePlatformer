using System;
using System.Collections;
using System.Collections.Generic;
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

        private List<Vector2> _mainPath;

        // Start is called before the first frame update
        void Awake()
        {
            _roomPivot = _pivotStartPos = transform.position;
            _rooms = new DungeonRoomInfo[_dungeonInfo.Size.x, _dungeonInfo.Size.y];

            //UnityEngine.Random.InitState();

            AssignActions();
            GrabTotalRoomSize();
            //SpawnRooms();
            //TODO: Re-checkRooms();

            //! debugging
            StartCoroutine(DrawMainPath());
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


        //! IT IS A CORROUTINE FOR DEBUG PURPOSES
        private IEnumerator DrawMainPath()
        {
            _mainPath = new List<Vector2>(150);
            _mainPath.Add(_pivotStartPos);

            int mainRooms = 0;
            Vector2Int pathIndex = Vector2Int.zero;
            List<Vector2Int> passedIndexes = new List<Vector2Int>(150);
            passedIndexes.Add(pathIndex);

            Vector2 pathPivot = _pivotStartPos;

            // 0-Left 1-Right 2-Down 3-Up
            byte[] dirs = new byte[12] { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3};
            
            if (_dungeonInfo.Ascending)
            {
                //dirs[7] = 3;
                dirs [8] = 3;
            }
            else 
            {
                //dirs[9] = 2;
                dirs[10] = 2;
            }

            if (_dungeonInfo.Backwards)
            {
                //dirs[3] = 0;
                dirs[4] = 0;
            }
            else 
            {
                //dirs[1] = 1;
                dirs[2] = 1;
            }

            byte lastDir = 0;

            int maxLoops = 30;
            int loops = 0;

            while (Mathf.Abs(pathIndex.x) < Mathf.Abs(_dungeonInfo.Size.x) && Mathf.Abs(pathIndex.y) < Mathf.Abs(_dungeonInfo.Size.y) && loops < maxLoops && mainRooms < _dungeonInfo.MaxMainRooms)
            {

                byte pathDir = dirs[UnityEngine.Random.Range(0, dirs.Length)];

                if (pathDir == 0 && lastDir != 0 && IsNextPathAvailable(new Vector2Int(pathIndex.x - 1, pathIndex.y)))
                {
                    loops = 0;
                    pathPivot.x -= _totalRoomSize.x;
                    pathIndex.x--;
                    mainRooms++;
                }
                else if (pathDir == 1 && lastDir != 1 && IsNextPathAvailable(new Vector2Int(pathIndex.x + 1, pathIndex.y)))
                {
                    loops = 0;
                    pathPivot.x += _totalRoomSize.x;
                    pathIndex.x++;
                    mainRooms++;
                }
                else if (pathDir == 2 && lastDir != 2 && IsNextPathAvailable(new Vector2Int(pathIndex.x, pathIndex.y - 1)))
                {
                    loops = 0;
                    pathPivot.y -= _totalRoomSize.y;
                    pathIndex.y--;
                    mainRooms++;
                }
                else if (pathDir == 3 && lastDir != 3 && IsNextPathAvailable(new Vector2Int(pathIndex.x, pathIndex.y + 1)))
                {
                    loops = 0;
                    pathPivot.y += _totalRoomSize.y;
                    pathIndex.y++;
                    mainRooms++;
                }

                passedIndexes.Add(pathIndex);
                _mainPath.Add(pathPivot);
                lastDir = pathDir;
                loops++;
                yield return new WaitForSeconds(0.0000f);
            }
            GC.Collect();

            // Creation is stuck 
            if (loops >= maxLoops)
            {
                // Restart
                StartCoroutine(DrawMainPath());
            }
            else 
            {
                Debug.LogWarning("Main Path Done");
            }

            // Checks if the has already been used and neighbors ones are availble
            bool IsNextPathAvailable(Vector2Int nexIndex)
            {
                bool available = true;

                available =
                !WasPathUsed(nexIndex) && (
                !WasPathUsed(new Vector2Int(nexIndex.x + 1, nexIndex.y)) ||
                !WasPathUsed(new Vector2Int(nexIndex.x - 1, nexIndex.y)) ||
                !WasPathUsed(new Vector2Int(nexIndex.x, nexIndex.y + 1)) ||
                !WasPathUsed(new Vector2Int(nexIndex.x, nexIndex.y - 1)));

                return available;
            }

            bool WasPathUsed(Vector2Int pathToCheck)
            {
                bool used = false;

                foreach (Vector2Int v in passedIndexes)
                {
                    if (pathToCheck == v)
                    {
                        used = true;
                        break;
                    }
                }

                return used;
            }
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

            Gizmos.color = Color.green;
            if (_mainPath != null)
            {
                Vector2 lastPoint = _pivotStartPos;
                foreach (Vector2 v in _mainPath)
                {
                    Gizmos.DrawLine(lastPoint, v);
                    lastPoint = v;
                    //Gizmos.DrawWireCube(v, _totalRoomSize);
                }
            }
        }

        private Action NewRoom;
        private Action NewRoomRow;
        private Action NewRoomColumn;
    }
}