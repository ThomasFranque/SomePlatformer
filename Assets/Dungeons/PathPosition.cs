using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dungeons
{
    public class PathPosition
    {
        private Vector2 _position;
        private Vector2Int _index;
        private bool[] _openings;

        public Vector2 Position => _position;
        public Vector2Int Index => _index;

        public bool[] Openings => _openings;

        public bool IsBranchIntersection { get; private set; }

        public PathPosition(Vector2 position)
        {
            _position = position;
            _index = Vector2Int.zero;
            _openings = new bool[4];
        }

        public PathPosition(Vector2 position, Vector2Int index)
        {
            _position = position;
            _index = index;
            _openings = new bool[4];
        }

        // Left Right Top Bottom
        public void SetOpenings(params bool[] openings)
        {
            _openings = openings;
        }

        public void TriggerBranch()
        {
            IsBranchIntersection = true;
        }
    }
}