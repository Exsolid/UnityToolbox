using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A script which is placed on a UI element and acts as a button. It will reset key controls to the last know setting.
/// It requires a <see cref="SettingsManager"/>, as well information about the ActionName and the Control (e.g. "Character Move" & "left").
/// The control can be left empty if no information is necessary. (e.g. "Character Jump" & "")
/// </summary>
public class ResetButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string _control;
    [SerializeField] private string _actionName;
    private SettingsManager manager;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
        {
            _isEnabled = isActive;
        };
    }

    private void Start()
    {
        manager = ModuleManager.GetModule<SettingsManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!_isEnabled) return;
        if (_control.Equals("") && _actionName.Equals("")) manager.ResetAllKeys();
        else manager.ResetKey(_control, _actionName);
    }
}
