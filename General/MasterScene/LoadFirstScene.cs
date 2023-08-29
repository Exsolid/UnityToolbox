using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbox.General.Management;

namespace UnityToolbox.General.MasterScene
{
    /// <summary>
    /// A script which loads the first scene. Should be placed within the master scene, as this is the initial scene to set up all <see cref="Module"/>s.
    /// Does not work in editor.
    /// </summary>
    public class LoadFirstScene : MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        // Start is called before the first frame update
        void Start()
        {
            if(!Application.isEditor)SceneManager.LoadScene(_sceneName);
        }
    }
}
