using UnityEngine;

public class DropDownAttribute : PropertyAttribute
{
    public string VariableNameForList;
    public bool UseParentNestedForList;

    public DropDownAttribute(string variableNameForList, bool useParentNestedForList)
    {
        VariableNameForList = variableNameForList;
        UseParentNestedForList = useParentNestedForList;
    }

    public DropDownAttribute(string variableNameForList)
    {
        VariableNameForList = variableNameForList;
    }
}
