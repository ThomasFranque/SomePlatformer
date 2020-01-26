using UnityEngine;

namespace Dungeons
{
    [CreateAssetMenu(menuName = "Dungeon/Dungeon Info")]
    public class DungeonInfo : ScriptableObject
    {
        [SerializeField] private DungeonRoomInfo _blockedRoom = null;
        [SerializeField] private Vector2Int _maxDungeonSize = new Vector2Int(8,8);
        [SerializeField] private int _maxMainRooms = 20;

        [SerializeField] private bool _ascending, _backwards = false;

        [Tooltip("Create them from the asset menu \"Dungeon/Room Info\"")]
        [SerializeField] private DungeonRoomInfo[] _possibleRooms = null;

        public DungeonRoomInfo BlockedRoom => _blockedRoom;

        public Vector2Int Size => _maxDungeonSize;
        public int MaxMainRooms => _maxMainRooms;

        public bool Ascending => _ascending;
        public bool Backwards => _backwards;

        public DungeonRoomInfo RandomRoomInfo => _possibleRooms[Random.Range(0, _possibleRooms.Length)];
    }
}
