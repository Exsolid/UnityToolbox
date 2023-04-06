using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : Module
{
    private Text _tooltipText;
    private Object _callContext;
    // Start is called before the first frame update
    void Start()
    {
        _tooltipText = GetComponent<Text>();
        _tooltipText.text = "";
    }

    public void UpdateTooltip(string control, string text, Object callContext)
    {
        if (_tooltipText != null && text != "")
        {
            _callContext = callContext;
            _tooltipText.text = "[" + control + "] " + text;
        }
        else if (callContext.Equals(_callContext))
        {
            _callContext = null;
            _tooltipText.text = "";
        }
    }
}
