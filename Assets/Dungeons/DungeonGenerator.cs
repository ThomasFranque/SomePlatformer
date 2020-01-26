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

        // Local world pivot position
        private Vector2 _roomPivot;

        // The calculated world scaled room size
        private Vector2 _totalRoomSize;

        private Vector2 _pivotStartPos;

        private List<PathPosition> _mainPath;

        // Start is called before the first frame update
        void Awake()
        {
            _roomPivot = _pivotStartPos = transform.position;

            //UnityEngine.Random.InitState();

            GrabTotalRoomSize();

            StartCoroutine(DrawMainPath());
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

        private IEnumerator DrawMainPath()
        {
            _mainPath = new List<PathPosition>(150);
            _mainPath.Add(new PathPosition(_pivotStartPos));

            int mainRooms = 0;
            Vector2Int pathIndex = Vector2Int.zero;
            List<Vector2Int> passedIndexes = new List<Vector2Int>(150);
            passedIndexes.Add(pathIndex);

            Vector2 pathPivot = _pivotStartPos;

            // 0-Left 1-Right 2-Down 3-Up
            byte[] dirs = new byte[12] { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };

            if (_dungeonInfo.Ascending)
            {
                //dirs[7] = 3;
                dirs[8] = 3;
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

                if (pathDir == 0 && lastDir != 1 && IsNextPathAvailable(new Vector2Int(pathIndex.x - 1, pathIndex.y)))
                {
                    loops = 0;
                    pathPivot.x -= _totalRoomSize.x;
                    pathIndex.x--;
                    mainRooms++;

                    passedIndexes.Add(pathIndex);
                    _mainPath.Add(new PathPosition(pathPivot));
                    lastDir = pathDir;
                }
                else if (pathDir == 1 && lastDir != 0 && IsNextPathAvailable(new Vector2Int(pathIndex.x + 1, pathIndex.y)))
                {
                    loops = 0;
                    pathPivot.x += _totalRoomSize.x;
                    pathIndex.x++;
                    mainRooms++;

                    passedIndexes.Add(pathIndex);
                    _mainPath.Add(new PathPosition(pathPivot));
                    lastDir = pathDir;
                }
                else if (pathDir == 2 && lastDir != 3 && IsNextPathAvailable(new Vector2Int(pathIndex.x, pathIndex.y - 1)))
                {
                    loops = 0;
                    pathPivot.y -= _totalRoomSize.y;
                    pathIndex.y--;
                    mainRooms++;

                    passedIndexes.Add(pathIndex);
                    _mainPath.Add(new PathPosition(pathPivot));
                    lastDir = pathDir;
                }
                else if (pathDir == 3 && lastDir != 2 && IsNextPathAvailable(new Vector2Int(pathIndex.x, pathIndex.y + 1)))
                {
                    loops = 0;
                    pathPivot.y += _totalRoomSize.y;
                    pathIndex.y++;
                    mainRooms++;

                    passedIndexes.Add(pathIndex);
                    _mainPath.Add(new PathPosition(pathPivot));
                    lastDir = pathDir;
                }

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
                SetPathOpenings();
                SpawnRoomsInPathPositions(_mainPath);
                //Debug.Log(_mainPath.Count);
                //TODO: Get path openings
                //SpawnRoomsInMainLine(); 
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

        private void SetPathOpenings()
        {
            int numOfPos = _mainPath.Count;
            for (int i = 0; i < numOfPos; i++)
            {
                // Left Right Top Bottom
                bool[] openings = new bool[4];

                Vector2? previousPos = null;
                Vector2? nextPos = null;
                if (i - 1 >= 0)
                    previousPos = _mainPath[i - 1].Position;
                if (i + 1 < numOfPos)
                    nextPos = _mainPath[i + 1].Position;

                //! BUG ON LEFT AND RIGHT

                if (previousPos != null)
                {
                    // Opening on the right ?
                    if (((Vector2)previousPos).x > _mainPath[i].Position.x)
                        openings[1] = true;
                    // Left
                    else if (((Vector2)previousPos).x < _mainPath[i].Position.x)
                        openings[0] = true;

                    // Opening up ?
                    else if (((Vector2)previousPos).y > _mainPath[i].Position.y)
                        openings[2] = true;
                    // Down
                    else if (((Vector2)previousPos).y < _mainPath[i].Position.y)
                        openings[3] = true;
                }
                if (nextPos != null)
                {
                    // Opening on the right ?
                    if (((Vector2)nextPos).x > _mainPath[i].Position.x)
                        openings[1] = true;
                    // Left
                    else if (((Vector2)nextPos).x < _mainPath[i].Position.x)
                        openings[0] = true;

                    // Opening up ?
                    else if (((Vector2)nextPos).y > _mainPath[i].Position.y)
                        openings[2] = true;
                    // Down
                    else if (((Vector2)nextPos).y < _mainPath[i].Position.y)
                        openings[3] = true;
                }

                _mainPath[i].SetOpenings(openings);
            }
        }

        private void SpawnRoomsInPathPositions(ICollection<PathPosition> pathPositions)
        {
            foreach(PathPosition p in pathPositions)
            {

               // DungeonRoomInfo newRoomInfo = _dungeonInfo.GetRoomWithOpenings(p.Openings);

                DungeonRoom newRoom =
                Instantiate(
                _dungeonInfo.DungeonRoom,
                p.Position,
                Quaternion.identity,
                _roomsTransform).GetComponent<DungeonRoom>();

                newRoom.SetOpenings(p.Openings);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(_roomPivot, 1.0f);

            Gizmos.color = Color.green;
            if (_mainPath != null)
            {
                bool toggle = false;
                Vector2 lastPoint = _pivotStartPos;
                foreach (PathPosition v in _mainPath)
                {
                    Gizmos.color = Color.green;

                    Gizmos.DrawLine(lastPoint, v.Position);

                    if (toggle)
                        Gizmos.color = Color.cyan;
                    else 
                        Gizmos.color = Color.magenta;

                    if(v.Openings[0])
                        Gizmos.DrawSphere(v.Position - new Vector2(_totalRoomSize.x/3, 0), 4.0f);
                    if(v.Openings[1])
                        Gizmos.DrawSphere(v.Position + new Vector2(_totalRoomSize.x/3, 0), 4.0f);
                    if(v.Openings[2])
                        Gizmos.DrawSphere(v.Position + new Vector2(0, _totalRoomSize.y/3), 4.0f);
                    if(v.Openings[3])
                        Gizmos.DrawSphere(v.Position - new Vector2(0, _totalRoomSize.y/3), 4.0f);

                    lastPoint = v.Position;
                    toggle = !toggle;
                    //Gizmos.DrawWireCube(v, _totalRoomSize);
                }
            }
        }
    }
}