
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string _control;
    [SerializeField] private string _actionName;
    private ControlManager manager;
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
        manager = ModuleManager.GetModule<ControlManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!_isEnabled) return;
        if (_control.Equals("") && _actionName.Equals("")) manager.ResetAllKeys();
        else manager.ResetKey(_control, _actionName);
    }
}
