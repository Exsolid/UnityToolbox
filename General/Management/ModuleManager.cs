using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityToolbox.General.Management.Logging;
using Logger = UnityToolbox.General.Management.Logging.Logger;

namespace UnityToolbox.General.Management
{
    /// <summary>
    /// The module manager (de)registeres implementations of the <see cref="Module"/>. All modules can be called from here, without searching the scene. 
    /// </summary>
    [Serializable]
    public class ModuleManager : MonoBehaviour
    {
        [SerializeField] private static ModuleManager _instance;
        [SerializeField] private List<Module> _moduls;
        public List<Module> Moduls 
        { 
            get { return _moduls; } 
        }

        private void Awake()
        {
            if (!ModuleManager.IsLoaded())
            {
                DontDestroyOnLoad(this.gameObject);
            }
            _instance = this;
            _moduls = new List<Module>();
        }

        /// <summary>
        /// Registeres a new <see cref="Module"/> of type <typeparamref name="T"/>. Only one <see cref="Module"/> of its type can be registered.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Module"/> to register.</typeparam>
        /// <param name="module">The <see cref="Module"/> to register.</param>
        /// <returns></returns>
        public static bool RegisterModul<T>(T module) where T : Module
        {
            bool isTypeAlreadyRegistered = _instance.Moduls.Where(mod => mod.GetType().IsEquivalentTo(module.GetType())).Any();
            if (isTypeAlreadyRegistered)
            {
                Logger.Log(LogLevel.WAR, typeof(ModuleManager), "Module of type [<color=" + Logger.CONTEXTCOLOR + ">" + module.GetType().Name + "</color>] already registered.");
                return false;
            }
            _instance.Moduls.Add(module);
            Logger.Log(LogLevel.INF, typeof(ModuleManager), "Registered module [<color=" + Logger.CONTEXTCOLOR + ">" + module.GetType().Name + "</color>]");
            return true;
        }

        /// <summary>
        /// Deregisters a modul.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Module"/> to deregister.</typeparam>
        /// <param name="module">The <see cref="Module"/> to deregister.</param>
        public static void DeregisterModul<T>(T module) where T : Module
        {
            _instance.Moduls.Remove(module);
            Logger.Log(LogLevel.INF, typeof(ModuleManager), "Deregistered module [<color=" + Logger.CONTEXTCOLOR + ">" + module.GetType().Name + "</color>]");
        }

        /// <summary>
        /// Check whether a <see cref="Module"/> is registered.
        /// </summary>
        /// <typeparam name="T">The type of module to search for.</typeparam>
        /// <returns>True if a <see cref="Module"/> of this type is found.</returns>
        public static bool ModuleRegistered<T>() where T : Module
        {
            return _instance.Moduls.Where(mod => mod.GetType().IsEquivalentTo(typeof(T))).Any();
        }

        /// <summary>
        /// Tries to find a <see cref="Module"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Module"/> to search for.</typeparam>
        /// <returns>The <see cref="Module"/> if it is found.</returns>
        /// <exception cref="Exception"></exception>
        public static T GetModule<T>() where T : Module
        {
            var toReturn = _instance.Moduls.Where(modul => modul.GetType() == typeof(T));
            if (!toReturn.Any())
            {
                throw new Exception("Module of type " + typeof(T).Name + " not registered!");
            }
            Logger.Log(LogLevel.INF, typeof(ModuleManager), "Module of type [<color=" + Logger.CONTEXTCOLOR + ">" + typeof(T).Name + "</color>] has been accessed.");
            return (T)toReturn.First();
        }

        /// <summary>
        /// Check whether the <see cref="ModuleManager"/> is loaded.
        /// </summary>
        /// <returns>True of the <see cref="ModuleManager"/> is instantiated.</returns>
        public static bool IsLoaded()
        {
            return _instance != null;
        }
    }
}
