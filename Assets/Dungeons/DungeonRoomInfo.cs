using UnityEngine;

namespace Dungeons
{
    [CreateAssetMenu(menuName = "Dungeon/Room Info")]
    public class DungeonRoomInfo : ScriptableObject
    {
        private const string _ROOM_TOOLTIP =
        "A premade room on a grid (beware of max cell count per room declared on DungeonGenerator.cs)";

        [Tooltip(_ROOM_TOOLTIP)]
        [SerializeField] private GameObject _roomPrefab;

        [Header("Openings")]
        [SerializeField] private bool _left;
        [SerializeField] private bool _right;
        [SerializeField] private bool _top;
        [SerializeField] private bool _bottom;

        public GameObject RoomPrefab => _roomPrefab;

        public bool[] Openings => new bool[4] { _left, _right, _top, _bottom };

        public static bool CanBePlaced(DungeonRoomInfo roomToCheck, params DungeonRoomInfo[] neighbors)
        {
            //! indexes follow the same order as openings ([0] is room on the left, etc... )
            bool canBePlaced = true;

            for(int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] != null)
                {
                    switch(i)
                    {
                        // Left n
                        case 0:
                            canBePlaced = roomToCheck.Openings[0] && neighbors[i].Openings[1] || !roomToCheck.Openings[0] && !neighbors[i].Openings[1];
                            break;
                        // Right n
                        case 1:
                            canBePlaced = roomToCheck.Openings[1] && neighbors[i].Openings[0] || !roomToCheck.Openings[1] && !neighbors[i].Openings[0];
                            break;
                        // Top n
                        case 2:
                            canBePlaced = roomToCheck.Openings[2] && neighbors[i].Openings[3] || !roomToCheck.Openings[2] && !neighbors[i].Openings[3];
                            break;
                        // Bottom n
                        case 3:
                            canBePlaced = roomToCheck.Openings[3] && neighbors[i].Openings[2] || !roomToCheck.Openings[3] && !neighbors[i].Openings[2];
                            break;
                    }
                    
                    if (!canBePlaced) break;
                }
            }

            return canBePlaced;
        }
    }
}
