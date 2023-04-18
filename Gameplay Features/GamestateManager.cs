using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A module which can manage liniear gamestates.
/// </summary>
public class GamestateManager : MonoBehaviour
{
    [SerializeField] private List<string> _gamestates;
    /// <summary>
    /// All gamestates.
    /// </summary>
    public List<string> Gamestates
    {
        get { return _gamestates.ToList(); }
    }

    private int _currentState;
    /// <summary>
    /// The current gamestate.
    /// </summary>
    public int CurrentState
    {
        get { return _currentState; }
    }

    /// <summary>
    /// Proceeds to the next gamestate.
    /// </summary>
    public void NextState()
    {
        if(_currentState < _gamestates.Count)
        {
            _currentState++;
        }
    }

    /// <summary>
    /// Returns the index value of a given gamestate.
    /// </summary>
    /// <param name="value">The gamestate.</param>
    /// <returns>The index of the gamestate.</returns>
    public int GetIndexOfValue(string value)
    {
        return _gamestates.IndexOf(value);
    }
}
