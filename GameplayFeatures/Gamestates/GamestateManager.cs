using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityToolbox.GameplayFeatures.SaveGame;
using UnityToolbox.General.Management;
using UnityToolbox.General.Preferences;

namespace UnityToolbox.GameplayFeatures.Gamestates
{
    /// <summary>
    /// This module manages all gamestates.
    /// The <see cref="SaveGameManager"/> is required for it to keep track of the players progress.
    /// </summary>
    public class GamestateManager : Module
    {
        public const string FILENAME = "GamestateData.txt";

        private Dictionary<GamestateNodeData, List<GamestateNodeData>> _stateNodeConnections;
        private Dictionary<string, GamestateNodeData> _stateNodes;
        private HashSet<GamestateNodeData> _activeNodes;

        private List<GamestateNodeData> _startNodes;

        /// <summary>
        /// An event which is triggered once the gamestate with the given string ID is completed;
        /// </summary>
        public event Action<string> OnGamestateCompleted;

        public void Start()
        {
            if (ModuleManager.ModuleRegistered<SaveGameManager>())
            {
                _activeNodes = ModuleManager.GetModule<SaveGameManager>().ActiveGamestates;
            }
            else
            {
                _activeNodes = new HashSet<GamestateNodeData>();
            }
            _stateNodeConnections = new Dictionary<GamestateNodeData, List<GamestateNodeData>>();
            _stateNodes = new Dictionary<string, GamestateNodeData>();
            _startNodes = new List<GamestateNodeData>();

            List<GamestateNodeData> data = ResourcesUtil.GetFileData<List<GamestateNodeData>>(ProjectPrefKeys.GAMESTATEDATASAVEPATH, FILENAME);
            if (data != null)
            { 
                foreach (GamestateNodeData node in data)
                {
                    _stateNodeConnections.Add(node, new List<GamestateNodeData>());
                    _stateNodes.Add(node.Name, node);
                    if(node.InputIDs.Count == 0)
                    {
                        _startNodes.Add(node);
                    }
                    node.IsActive = _activeNodes.Contains(node);
                }

                foreach (GamestateNodeData node in data)
                {
                    foreach (int id in node.OutputIDs)
                    {
                        GamestateNodeData nextNode = data.Where(n => n.ID.Equals(id)).FirstOrDefault();
                        _stateNodeConnections[node].Add(nextNode);
                    }
                }

                if(_activeNodes.Count == 0)
                {
                    foreach(GamestateNodeData startNode in _startNodes)
                    {
                        startNode.IsActive = true;
                        _activeNodes.Add(startNode);
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether a given state name is active or not.
        /// </summary>
        /// <param name="stateName">The unique name of the state.</param>
        /// <returns>Whether the gamestate is active.</returns>
        public bool IsStateActive(string stateName)
        {
            CheckNodeValid(stateName);
            return _activeNodes.Where(node => node.Name.Equals(stateName) && node.IsActive).Any();
        }

        /// <summary>
        /// Proceeds to the next state if possible. Sets the current state to inactive.
        /// </summary>
        /// <param name="currentState">The current gamestate.</param>
        /// <param name="option">Which path the gamestate should take within the tree. Input 0 if it is linear.</param>
        /// <exception cref="ArgumentException"></exception>
        public void GoToNextState(string currentState, int option)
        {
            CheckNodeValid(currentState);
            if (!_activeNodes.Where(node => node.Name.Equals(currentState) && node.IsActive).Any())
            {
                Debug.LogWarning("The given state \"" + currentState + "\" is not active and therefore cannot proceed further.");
            }
            GamestateNodeData node = _stateNodes[currentState];


            if (option >= _stateNodes[currentState].OutputIDs.Count)
            {
                throw new ArgumentException("There is no option " + option + " for state \"" + currentState + "\".");
            }

            node.IsActive = false;
            _stateNodeConnections[node][option].IsActive = true;
            _activeNodes.Remove(node);
            _activeNodes.Add(_stateNodeConnections[node][option]);

            Debug.Log("Completed gamestate \"" + node.Name + "\" and proceeded to gamestate \"" + _stateNodeConnections[node][option].Name + "\"");
            OnGamestateCompleted?.Invoke(node.Name);

            if (ModuleManager.ModuleRegistered<SaveGameManager>())
            {
                ModuleManager.GetModule<SaveGameManager>().ActiveGamestates = _activeNodes;
            }
        }

        private void CheckNodeValid(string stateName)
        {
            if (!_stateNodes.ContainsKey(stateName))
            {
                throw new ArgumentException("The given state \"" + stateName + "\" is not recognized. Is it typed correctly?");
            }
        }
    }
}
