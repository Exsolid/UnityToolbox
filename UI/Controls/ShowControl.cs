using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ShowControl : MonoBehaviour
{
    [SerializeField] private string _control;
    [SerializeField] private string _actionName;
    [SerializeField] private Text _displayText;
    private SettingsManager _manager;
    private bool _isEnabled;

    public void Awake()
    {
        GetComponentInParent<Menu>().OnActiveChanged += (isActive) =>
        {
            _isEnabled = isActive;
        };
    }

    void Start()
    {
        _manager = ModuleManager.GetModule<SettingsManager>();
    }

    private void Update()
    {
        if (_isEnabled)
        {
            _displayText.text = _manager.CurrentValueOfControl(_control, _actionName).Split("/").Last();
        }
    }
}
