using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GamestateManager : MonoBehaviour
{
    [SerializeField] private List<string> _gamestates;
    public List<string> Gamestates
    {
        get { return _gamestates.ToList(); }
    }

    private int _currentState;
    public int CurrentState
    {
        get { return _currentState; }
    }

    public void NextState()
    {
        if(_currentState < _gamestates.Count)
        {
            _currentState++;
        }
    }

    public int GetIndexOfValue(string value)
    {
        return _gamestates.IndexOf(value);
    }
}
