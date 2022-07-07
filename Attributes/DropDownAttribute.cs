using UnityEngine;

public class DropDownAttribute : PropertyAttribute
{
    public string VariableNameForList;

    public DropDownAttribute(string variableNameForList)
    {
        VariableNameForList = variableNameForList;
    }
}
