using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dungeons
{
    public class PathPosition
    {
        private Vector2 _position;
        private bool[] _openings;

        public Vector2 Position => _position;
        public bool[] Openings => _openings;

        public PathPosition(Vector2 position)
        {
            _position = position;
            _openings = new bool[4];
        }

        // Left Right Top Bottom
        public void SetOpenings(params bool[] openings)
        {
            _openings = openings;
        } 
        
    }
}