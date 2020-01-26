using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    private GameObject _leftWall;
    private GameObject _rightWall;
    private GameObject _topWall;
    private GameObject _bottomWall;

    private void Awake() 
    {
        GameObject _wallsHolder = transform.GetChild(0).gameObject;
        _leftWall = _wallsHolder.transform.GetChild(0).gameObject;    
        _rightWall = _wallsHolder.transform.GetChild(1).gameObject;    
        _topWall = _wallsHolder.transform.GetChild(2).gameObject;    
        _bottomWall = _wallsHolder.transform.GetChild(3).gameObject;
    }

    public void SetOpenings(bool[] openings)
    {
        if (openings[0]) Destroy(_leftWall);
        if (openings[1]) Destroy(_rightWall);
        if (openings[2]) Destroy(_topWall);
        if (openings[3]) Destroy(_bottomWall);
    }
}
