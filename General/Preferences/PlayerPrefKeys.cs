using UnityEngine;
using UnityToolbox.General.Management;

namespace UnityToolbox.General.Preferences
{
    /// <summary>
    /// A collection of keys that can be used across projects. A keyword should be defined, which is added to the default key. See <see cref="GetPrefereceKey"/>.
    /// </summary>
    public class PlayerPrefKeys : Module
    {
        [SerializeField] private string _keyword;
        public string Keyword
        {
            get
            {
                return _keyword + "_";
            }
        }

        public static string JSON_CONTROLS = "JSON_CONTROLS";
        public static string MOUSE_SENSITIVITY = "MOUSE_SENSITIVITY";
        public static string EFFECTS_VOLUME = "EFFECTS_VOLUME";
        public static string MUSIC_VOLUME = "MUSIC_VOLUME";
        public static string AMBIENCE_VOLUME = "AMBIENCE_VOLUME";
        public static string LANGUAGE = "LANGUAGE";
        public static string PLAYER_NAME = "PLAYER_NAME";

        public static string GAMEPLAY_SAVEGAME = "GAMEPLAY_SAVEGAME";
        public static string GAMEPLAY_STATE = "GAMEPLAY_STATE";
        public static string IDS = "IDS";

        public static string DEBUG_ORIGINAL_SCENE = "DEBUG_ORIGINAL_SCNENE";

        public override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Combines a given player pref key with the project keyword set.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The combined player pref key, that can be used for the <see cref="PlayerPrefs"/>.</returns>
        public string GetPrefereceKey(string id)
        {
            return _keyword + "_" + this.GetType().GetField(id).GetValue(this);
        }
    }
}
