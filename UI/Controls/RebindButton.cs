using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A script which is placed on a UI element and acts as a button. It will rebind key controls.
/// It requires the <see cref="UIEventManager"/> and the <see cref="SettingsManager"/>, as well information about the ActionName and the Control (e.g. "Character Move" & "left").
/// The control can be left empty if no information is necessary. (e.g. "Character Jump" & "")
/// </summary>
public class RebindButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioMixer _clickSounds;
    [SerializeField] private AudioMixer _errorSounds;
    [SerializeField] private string _control;
    [SerializeField] private string _actionName;
    private string _keyPress;
    private bool _isKeyboard;
    private bool _isSetting;
    private string _alternateText;

    private Text _textChild;

    private bool _otherIsSetting;

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
        _alternateText = "";
        _textChild = (Text)gameObject.GetComponentInChildren(typeof(Text));
        _manager = ModuleManager.GetModule<SettingsManager>();
        ModuleManager.GetModule<UIEventManager>().OnBindingKey += (isSetting) => { _otherIsSetting = isSetting; };
    }

    public void OnGUI()
    {
        _textChild.text = _alternateText.Equals("") ? ReturnKeyCode(_manager.CurrentValueOfControl(_control, _actionName)) : _alternateText;
        Event e = Event.current;
        if (e != null && e.type.Equals(EventType.KeyDown) && e.keyCode != KeyCode.None)
            _keyPress = e.keyCode.ToString(); _isKeyboard = true;
        if (e != null && e.isMouse)
        {
            _isKeyboard = false;
            switch (e.button)
            {
                case 0:
                    _keyPress = "leftButton";
                    break;
                case 1:
                    _keyPress = "rightButton";
                    break;
                case 2:
                    _keyPress = "middleButton";
                    break;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isSetting && _isEnabled && !_otherIsSetting)
        {
            ModuleManager.GetModule<UIEventManager>().BindingKey(true);
            StartCoroutine(SetEvent());
            _isSetting = true;

            if (_clickSounds != null)
            {
                _clickSounds.PlayRandomSource();
            }
        }
    }

    private void SetKeyToControl(string key, string keyOrMouseCode)
    {
        _alternateText = "";
        string keyPath = keyOrMouseCode + key;
        Dictionary<string, string> temp = new Dictionary<string, string>();
        if (!_manager.SetKey(_control, keyPath, _actionName))
        {
            _alternateText = "Input invalid";

            if (_errorSounds != null)
            {
                _errorSounds.PlayRandomSource();
            }
            StartCoroutine(ResetText());
        }
    }
    IEnumerator ResetText()
    {
        yield return new WaitForSeconds(2);
        _alternateText = "";
    }
    IEnumerator SetEvent()
    {
        _keyPress = "";
        do
        {
            yield return null;
        } while (_keyPress == "");
        SetKeyToControl(_keyPress, _isKeyboard ? "<Keyboard>/" : "<Mouse>/");
        yield return new WaitForSeconds(0.3f);
        _isSetting = false;
        ModuleManager.GetModule<UIEventManager>().BindingKey(false);
    }

    private string ReturnKeyCode(string path)
    {
        if (path.Equals("")) return path;
        char[] chars = path.ToCharArray();
        bool canStart = false;
        path = "";
        bool nextToUpper = true;
        foreach (char c in chars)
        {
            if (canStart)
            {
                char newC = c;
                if (nextToUpper)
                {
                    newC = char.ToUpper(newC);
                    nextToUpper = false;
                }
                else if (char.IsUpper(newC)) path = path + " ";
                path = path + newC;

            }
            if (c.Equals('/')) canStart = true;
        }
        return path;
    }
}
