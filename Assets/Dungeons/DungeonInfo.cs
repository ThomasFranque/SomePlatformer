using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeons
{
    [CreateAssetMenu(menuName = "Dungeon/Dungeon Info")]
    public class DungeonInfo : ScriptableObject
    {
        [SerializeField] private Color _dungeonColor = Color.white;
        [SerializeField] private Sprite[] _possibleTileSprites = null;
        [SerializeField] private GameObject[] _possibleAerialEnemies = null;
        [SerializeField] private GameObject[] _possibleGroundEnemies = null;
        [SerializeField] private GameObject[] _possibleHazards = null;

        //[SerializeField] private Vector2Int _maxDungeonSize = new Vector2Int(5,5);
        [Tooltip("Odd numbers work best.")]
        [SerializeField] private Vector2Int _dungeonRoomSize = new Vector2Int(15,11);
        [Tooltip("Starting around 60, generation time will be a little more and the results may not turn out the best. (May even freeze unity)")]
        [SerializeField] private int _maxRoomsPerPath = 15;
        [Range(0.0f,1.0f)] [SerializeField] private float _complexity = 0.6f;
        [SerializeField] private bool _generateBranches = true;
        [Tooltip("Exponentially increases generation time! (May even freeze unity depending on the max rooms per path)")]
        [SerializeField] private bool _branchesHaveBranches = false;

        [Tooltip("It is not guaranteed, will only increase the odds.")]
        [SerializeField] private bool _ascending, _backwards = false;
        [SerializeField] private bool _isDebug = false;

        private DungeonTile[] _tiles;

#region Getters
        //public Vector2Int Size => _maxDungeonSize;
        public Color Color => _dungeonColor;
        public Vector2Int RoomSize => _dungeonRoomSize;
        public int MaxRoomsPerPath => _maxRoomsPerPath;
        public float Complexity => _complexity;

        public bool GenerateBranches => _generateBranches;
        public bool BranchesHaveBranches => _branchesHaveBranches;

        public bool Ascending => _ascending;
        public bool Backwards => _backwards;
        public DungeonTile DungeonTile => _tiles[Random.Range(0, _tiles.Length)];
        public GameObject GetRandomAerialEnemy => _possibleAerialEnemies[Random.Range(0,_possibleAerialEnemies.Length)];
        public GameObject GetRandomGroundEnemy => _possibleGroundEnemies[Random.Range(0,_possibleGroundEnemies.Length)];
        public GameObject GetRandomHazard => _possibleHazards[Random.Range(0,_possibleHazards.Length)];

        public bool IsDebug => _isDebug;
#endregion
        
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
