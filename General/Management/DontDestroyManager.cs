using System;
using System.Collections.Generic;

namespace UnityToolbox.General.Management
{
    public class DontDestroyManager : Module
    {
        Dictionary<string, DontDestroyObject> _dontDestroyObjects;

        private new void Awake()
        {
            base.Awake();
            _dontDestroyObjects = new Dictionary<string, DontDestroyObject>();
        }

        public void RegisterObject(DontDestroyObject gameObject)
        {
            if (_dontDestroyObjects.ContainsKey(gameObject.Identifier))
            {
                throw new ArgumentException("The given key \"" + gameObject.Identifier + "\" is already registered.");
            }

            _dontDestroyObjects.Add(gameObject.Identifier, gameObject);
        }

        public void DeregisterObject(DontDestroyObject gameObject)
        {
            _dontDestroyObjects.Remove(gameObject.Identifier);
            Destroy(gameObject.gameObject);
        }

        public void DeregisterObject(string key)
        {
            DontDestroyObject gameObject = _dontDestroyObjects[key];
            _dontDestroyObjects.Remove(key);
            Destroy(gameObject.gameObject);
        }

        public DontDestroyObject GetObject(string key)
        {
            if (!_dontDestroyObjects.ContainsKey(key))
            {
                return null;
            }
            return _dontDestroyObjects[key];
        }
    }
}
