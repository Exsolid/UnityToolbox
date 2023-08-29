using UnityEngine;
using UnityToolbox.General.Attributes;

namespace UnityToolbox.GameplayFeatures.SaveGame
{
    /// <summary>
    /// A script used to identify parents for the <see cref="SaveGameManager"/>.
    /// </summary>
    public class ParentIdentifier : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private string id = "";
        public string ID
        {
            get { return id; }
        }

        private void OnValidate()
        {
            if (id.Equals(""))
            {
                id = IDManager.GetUniqueID();
            }
        }
    }
}
