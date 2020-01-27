using UnityEngine;

namespace Dungeons
{
    [CreateAssetMenu(menuName = "Dungeon/Dungeon Info")]
    public class DungeonInfo : ScriptableObject
    {
        [SerializeField] private GameObject _dungeonRoom = null;
        [SerializeField] private Vector2Int _maxDungeonSize = new Vector2Int(8,8);
        [SerializeField] private int _maxMainRooms = 15;
        [Range(0.0f,1.0f)] [SerializeField] private float _complexity = 0.8f;

        [SerializeField] private bool _ascending, _backwards = false;

        public Vector2Int Size => _maxDungeonSize;
        public int MaxMainRooms => _maxMainRooms;
        public float Complexity => _complexity;

        public bool Ascending => _ascending;
        public bool Backwards => _backwards;

        public GameObject DungeonRoom => _dungeonRoom;
    }
}
