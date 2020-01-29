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
        [SerializeField] private bool _debugMode;

        // The calculated world scaled room size
        private Vector2 _totalRoomSize;

        private List<List<PathPosition>> _allPaths;

        // Start is called before the first frame update
        void Awake()
        {
            _allPaths = new List<List<PathPosition>>(10);

            //UnityEngine.Random.InitState();

            GrabTotalRoomSize();
            CreateDungeon();
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

        private void CreateDungeon()
        {
            PathPosition startPathPos = new PathPosition(transform.position);

            // Main path
            List<PathPosition> newPath;

            do
            {
                newPath = CreateNewPath(startPathPos, true);
            } while (newPath.Count == 0);

            AddPathToAllPaths(newPath);
            Debug.LogWarning("Main path generated");

            // Create branches
            for (int i = 0; i < _allPaths.Count; i++)
            {
                int count = _allPaths[i].Count;
                for (int j = 0; j < count - 1; j++)
                    if (_allPaths[i][j].IsBranchIntersection)
                    {
                        //Drawbranch
                        newPath = CreateNewPath(_allPaths[i][j], false, true, false);

                        if (newPath.Count != 0)
                        {
                            AddPathToAllPaths(newPath);
                            Debug.LogWarning("New branch path generated");
                        }
                    }
            }

            SetAllPathOpenings();
            Debug.LogWarning("All path openings set");

            foreach (List<PathPosition> ps in _allPaths)
            {
                //SetPathOpenings(ps);
                SpawnRoomsInPathPositions(ps);
            }
            Debug.LogWarning("Rooms spawned.");

        }


        private List<PathPosition> CreateNewPath(PathPosition startPos, bool addFirstPosToCollection, bool ignoreDungeonInfo = false, bool createNewBranches = true)
        {
            List<PathPosition> newDrawnPath = new List<PathPosition>(150);
            if (addFirstPosToCollection)
                newDrawnPath.Add(new PathPosition(startPos.Position, startPos.Index));


            int rooms = 0;
            Vector2Int pathIndex = startPos.Index;
            Vector2 pathPivot = startPos.Position;

            // 0-Left 1-Right 2-Down 3-Up
            byte[] dirs = new byte[12] { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };

            if (!ignoreDungeonInfo)
            {
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
            }

            int maxLoops = 30;
            int loops = 0;
            byte? lastDir = null;
            byte pathDir = dirs[UnityEngine.Random.Range(0, dirs.Length)];
            float rndNum;
            while (Mathf.Abs(pathIndex.x) < Mathf.Abs(_dungeonInfo.Size.x) && Mathf.Abs(pathIndex.y) < Mathf.Abs(_dungeonInfo.Size.y) && loops < maxLoops && rooms < _dungeonInfo.MaxRoomsPerPath)
            {
                rndNum = UnityEngine.Random.Range(0.0f, 1.0f);
                bool shouldChange = rndNum < _dungeonInfo.Complexity;

                if (shouldChange)
                {
                    do
                    {
                        pathDir = dirs[UnityEngine.Random.Range(0, dirs.Length)];
                    } while (lastDir != null && pathDir == lastDir);
                }

                if (pathDir == 0 && lastDir != 1 && IsNextPathAvailable(new Vector2Int(pathIndex.x - 1, pathIndex.y)))
                {
                    pathPivot.x -= _totalRoomSize.x;
                    pathIndex.x--;
                    NewRoom();
                }
                else if (pathDir == 1 && lastDir != 0 && IsNextPathAvailable(new Vector2Int(pathIndex.x + 1, pathIndex.y)))
                {
                    pathPivot.x += _totalRoomSize.x;
                    pathIndex.x++;
                    NewRoom();
                }
                else if (pathDir == 2 && lastDir != 3 && IsNextPathAvailable(new Vector2Int(pathIndex.x, pathIndex.y - 1)))
                {
                    pathPivot.y -= _totalRoomSize.y;
                    pathIndex.y--;
                    NewRoom();
                }
                else if (pathDir == 3 && lastDir != 2 && IsNextPathAvailable(new Vector2Int(pathIndex.x, pathIndex.y + 1)))
                {
                    pathPivot.y += _totalRoomSize.y;
                    pathIndex.y++;
                    NewRoom();
                }

                loops++;
            }
            GC.Collect();

            //  Recursive
            // // Creation is stuck 
            // if (loops >= maxLoops && restartIfStuck)
            // {
            //     // Restart
            //     CreateNewPath(startPos, addFirstPosToCollection, restartIfStuck);
            // }
            // else
            // {
            //     Debug.LogWarning("Path Done");


            //     AddPathToAllPaths(newDrawnPath);
            // }

            return newDrawnPath;

            void NewRoom()
            {
                loops = 0;
                rooms++;

                PathPosition newPos = new PathPosition(pathPivot, pathIndex);

                if (rndNum < _dungeonInfo.Complexity / 3 && (createNewBranches || _dungeonInfo.BranchesHaveBranches) && _dungeonInfo.GenerateBranches)
                {
                    newPos.TriggerBranch();
                }

                newDrawnPath.Add(newPos);
                lastDir = pathDir;
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

                foreach (PathPosition p in newDrawnPath)
                {
                    if (pathToCheck == p.Index)
                    {
                        used = true;
                        break;
                    }
                }

                if (used) return used;

                int size;
                foreach (List<PathPosition> c in _allPaths)
                {
                    size = c.Count;
                    for (int i = 0; i < size; i++)
                    {
                        if (c[i].Index != pathToCheck) continue;

                        used = true;
                        break;

                    }
                }

                return used;
            }
        }

        private void AddPathToAllPaths(List<PathPosition> newDrawnPath)
        {
            _allPaths.Add(newDrawnPath);
        }

        private void SetAllPathOpenings()
        {
            Vector2Int leftIndex = Vector2Int.zero;
            Vector2Int rightIndex = Vector2Int.zero;
            Vector2Int topIndex = Vector2Int.zero;
            Vector2Int bottomIndex = Vector2Int.zero;

            // Foreach individual path
            foreach (List<PathPosition> c in _allPaths)
            {
                int points = c.Count;
                // Foreach individual point 
                for (int i = 0; i < points; i++)
                {
                    leftIndex = new Vector2Int(c[i].Index.x - 1, c[i].Index.y);
                    rightIndex = new Vector2Int(c[i].Index.x + 1, c[i].Index.y);
                    topIndex = new Vector2Int(c[i].Index.x, c[i].Index.y + 1);
                    bottomIndex = new Vector2Int(c[i].Index.x, c[i].Index.y - 1);

                    bool[] openings = new bool[4];

                    // Go through every path
                    foreach (List<PathPosition> n in _allPaths)
                    {
                        int nSize = n.Count;
                        // Through every point again
                        for (int j = 0; j < nSize; j++)
                        {
                            // Connect if the collection is the same or is a intersection and checking point is the first room of collection and vice versa
                            if (!(c == n || (c[i].IsBranchIntersection && j == 0) || (i == 0 && n[j].IsBranchIntersection))) continue;

                            if (n[j].Index == leftIndex)
                                openings[0] = true;
                            else if (n[j].Index == rightIndex)
                                openings[1] = true;
                            else if (n[j].Index == topIndex)
                                openings[2] = true;
                            else if (n[j].Index == bottomIndex)
                                openings[3] = true;

                        }
                    }

                    // And finally, set openings
                    c[i].SetOpenings(openings);
                }
            }
        }

        private void SetPathOpenings(List<PathPosition> pathPosits)
        {
            int numOfPos = pathPosits.Count;
            for (int i = 0; i < numOfPos; i++)
            {
                // Left Right Top Bottom
                bool[] openings = new bool[4];

                Vector2? previousPos = null;
                Vector2? nextPos = null;
                if (i - 1 >= 0)
                    previousPos = pathPosits[i - 1].Position;
                if (i + 1 < numOfPos)
                    nextPos = pathPosits[i + 1].Position;


                if (previousPos != null)
                {
                    // Opening on the right ?
                    if (((Vector2)previousPos).x > pathPosits[i].Position.x)
                        openings[1] = true;
                    // Left
                    else if (((Vector2)previousPos).x < pathPosits[i].Position.x)
                        openings[0] = true;

                    // Opening up ?
                    else if (((Vector2)previousPos).y > pathPosits[i].Position.y)
                        openings[2] = true;
                    // Down
                    else if (((Vector2)previousPos).y < pathPosits[i].Position.y)
                        openings[3] = true;
                }
                if (nextPos != null)
                {
                    // Opening on the right ?
                    if (((Vector2)nextPos).x > pathPosits[i].Position.x)
                        openings[1] = true;
                    // Left
                    else if (((Vector2)nextPos).x < pathPosits[i].Position.x)
                        openings[0] = true;

                    // Opening up ?
                    else if (((Vector2)nextPos).y > pathPosits[i].Position.y)
                        openings[2] = true;
                    // Down
                    else if (((Vector2)nextPos).y < pathPosits[i].Position.y)
                        openings[3] = true;
                }

                pathPosits[i].SetOpenings(openings);
            }
        }

        private void SpawnRoomsInPathPositions(ICollection<PathPosition> pathPositions)
        {
            foreach (PathPosition p in pathPositions)
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
            if (_debugMode)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(transform.position, (new Vector3(_totalRoomSize.x, _totalRoomSize.y) * (Vector2)_dungeonInfo.Size * 2));

                if (_allPaths != null)
                {
                    //bool toggle = false;
                    foreach (List<PathPosition> c in _allPaths)
                    {
                        foreach (PathPosition p in c)
                        {
                            if (p.IsBranchIntersection)
                            {
                                Gizmos.color = Color.yellow;
                                Gizmos.DrawWireCube(p.Position, new Vector3(16, 16, 16));
                            }
                            if (p == c[c.Count - 1])
                            {
                                if (c == _allPaths[0])
                                    Gizmos.color = Color.magenta;
                                else
                                    Gizmos.color = Color.red;
                                Gizmos.DrawSphere(p.Position, 6.0f);
                            }
                            else if (p == c[0])
                            {
                                if (c == _allPaths[0])
                                    Gizmos.color = Color.green;
                                else
                                    Gizmos.color = Color.cyan;
                                Gizmos.DrawSphere(p.Position, 6.0f);
                            }
                            else
                            {
                                if (c == _allPaths[0])
                                    Gizmos.color = Color.white;
                                else Gizmos.color = Color.grey;

                                Gizmos.DrawWireSphere(p.Position, 4.5f);
                            }

                            // if (toggle)
                            //     Gizmos.color = Color.cyan;
                            // else 
                            //     Gizmos.color = Color.magenta;

                            // if(v.Openings[0])
                            //     Gizmos.DrawSphere(v.Position - new Vector2(_totalRoomSize.x/3, 0), 4.0f);
                            // if(v.Openings[1])
                            //     Gizmos.DrawSphere(v.Position + new Vector2(_totalRoomSize.x/3, 0), 4.0f);
                            // if(v.Openings[2])
                            //     Gizmos.DrawSphere(v.Position + new Vector2(0, _totalRoomSize.y/3), 4.0f);
                            // if(v.Openings[3])
                            //     Gizmos.DrawSphere(v.Position - new Vector2(0, _totalRoomSize.y/3), 4.0f);

                            //toggle = !toggle;
                            //Gizmos.DrawWireCube(v, _totalRoomSize);
                        }
                    }
                }
            }
        }
    }
}