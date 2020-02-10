using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeons
{
    [CreateAssetMenu(menuName = "Dungeon/Dungeon Info")]
    public class DungeonInfo : ScriptableObject
    {
        [SerializeField] private Sprite[] _possibleTileSprites = null;
        //[SerializeField] private Vector2Int _maxDungeonSize = new Vector2Int(5,5);
        [Tooltip("Odd numbers work best.")]
        [SerializeField] private Vector2Int _dungeonRoomSize = new Vector2Int(25,12);
        [SerializeField] private int _maxRoomsPerPath = 15;
        [Range(0.0f,1.0f)] [SerializeField] private float _complexity = 0.6f;
        [SerializeField] private bool _generateBranches = true;
        [Tooltip("Exponentially increases generation time!")]
        [SerializeField] private bool _branchesHaveBranches = false;

        [Tooltip("It is not guaranteed, will only increase the odds.")]
        [SerializeField] private bool _ascending, _backwards = false;
        [SerializeField] private bool _isDebug = false;

        private DungeonTile[] _tiles;

        //public Vector2Int Size => _maxDungeonSize;
        public Vector2Int RoomSize => _dungeonRoomSize;
        public int MaxRoomsPerPath => _maxRoomsPerPath;
        public float Complexity => _complexity;

        public bool GenerateBranches => _generateBranches;
        public bool BranchesHaveBranches => _branchesHaveBranches;

        public bool Ascending => _ascending;
        public bool Backwards => _backwards;
        public DungeonTile DungeonTile => _tiles[Random.Range(0, _tiles.Length)];

        public bool IsDebug => _isDebug;

        public void CreateTiles()
        {
            //_tile = (DungeonTile)ScriptableObject.CreateInstance(typeof(DungeonTile));    
            _tiles = new DungeonTile[_possibleTileSprites.Length];    

            for (int i = 0; i < _possibleTileSprites.Length; i++)
            {
                _tiles[i] = (DungeonTile)ScriptableObject.CreateInstance(typeof(DungeonTile));
                _tiles[i].Sprite = _possibleTileSprites[i];
            }
        }
    }
}
