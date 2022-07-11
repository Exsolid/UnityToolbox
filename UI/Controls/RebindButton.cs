using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RebindButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string _control;
    [SerializeField] private string _actionName;
    private string _keyPress;
    private bool _isKeyboard;
    private bool _isSetting;
    private string _alternateText;

    private Text _textChild;

    private bool _otherIsSetting;

    private ControlManager _manager; 
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
        _manager = ModuleManager.GetModule<ControlManager>();
        ModuleManager.GetModule<UIEventManager>().OnBindingKey += (isSetting) => { _otherIsSetting = isSetting; };
    }
    public void OnGUI()
    {
        _textChild.text = _alternateText.Equals("") ? returnKeyCode(_manager.CurrentValueOfControl(_control, _actionName)) : _alternateText;
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
            StartCoroutine(setEvent());
            _isSetting = true;
        }
    }

    private void setKeyToControl(string key, string keyOrMouseCode)
    {
        _alternateText = "";
        string keyPath = keyOrMouseCode + key;
        Dictionary<string, string> temp = new Dictionary<string, string>();
        if (!_manager.SetKey(_control, keyPath, _actionName))
        {
            _alternateText = "Input invalid";
            StartCoroutine(resetText());
        }
    }
    IEnumerator resetText()
    {
        yield return new WaitForSeconds(3);
        _alternateText = "";
    }
    IEnumerator setEvent()
    {
        _keyPress = "";
        do
        {
            yield return null;
        } while (_keyPress == "");
        setKeyToControl(_keyPress, _isKeyboard ? "<Keyboard>/" : "<Mouse>/");
        yield return new WaitForSeconds(0.3f);
        _isSetting = false;
        ModuleManager.GetModule<UIEventManager>().BindingKey(false);
    }

    private string returnKeyCode(string path)
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
