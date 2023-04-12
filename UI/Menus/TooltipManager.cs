using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A manager which is used to display tooltips.
/// </summary>
public class TooltipManager : Module
{
    [SerializeField] private Text _tooltipText;
    private Object _callContext;
    // Start is called before the first frame update
    void Start()
    {
        if(_tooltipText == null)
        {
            _tooltipText = GetComponent<Text>();
        } 
        _tooltipText.text = "";
    }

    /// <summary>
    /// Updates the current tooltip. A new <paramref name="text"/> can only be set if the old has been updated to empty by the same <paramref name="callContext"/>.
    /// </summary>
    /// <param name="keyToDisplay">A key to display.)</param>
    /// <param name="text">The text to display.</param>
    /// <param name="callContext">The caller which sets and is then allowed to reset the tooltip.</param>
    public void UpdateTooltip(string keyToDisplay, string text, Object callContext)
    {
        if (_tooltipText != null && text != "")
        {
            _callContext = callContext;
            if(keyToDisplay != null && !keyToDisplay.Equals(""))
            {
                _tooltipText.text = "[" + keyToDisplay + "] " + text;
            }
            else
            {
                _tooltipText.text = text;
            }
        }
        else if (callContext.Equals(_callContext))
        {
            _callContext = null;
            _tooltipText.text = "";
        }
    }
}
