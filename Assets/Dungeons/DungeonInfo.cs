using UnityEngine;

namespace Dungeons
{
    [CreateAssetMenu(menuName = "Dungeon/Dungeon Info")]
    public class DungeonInfo : ScriptableObject
    {
        [SerializeField] private DungeonRoomInfo _blockedRoom;
        [SerializeField] private Vector2Int _dungeonSize;

        [SerializeField] private bool _ascending, _backwards;

        [Tooltip("Create them from the asset menu \"Dungeon/Room Info\"")]
        [SerializeField] private DungeonRoomInfo[] _possibleRooms;

        public DungeonRoomInfo BlockedRoom => _blockedRoom;

        public Vector2Int Size => _dungeonSize;

        public bool Ascending => _ascending;
        public bool Backwards => _backwards;

        public DungeonRoomInfo RandomRoomInfo => _possibleRooms[Random.Range(0, _possibleRooms.Length)];
    }
}
