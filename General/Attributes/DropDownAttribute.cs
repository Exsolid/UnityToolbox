using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines a [DropDown(..)] attribute, which creates a drop down menu to select from.
/// </summary>
public class DropDownAttribute : PropertyAttribute
{
    public string VariableNameForList;
    public bool UseParentNestedForList;

    /// <summary>
    /// Creates a dropdown menu from a <see cref="List{T>}"/> of type <see cref="string"/> found by <paramref name="variableNameForList"/>.
    /// Use <paramref name="useParentNestedForList"/> if the object which should be displaying a dropdown is nested within a collection. See <see cref="MenuList"/> for an implementation.
    /// </summary>
    /// <param name="variableNameForList">The name of the <see cref="List{T>}"/> of type <see cref="string"/>.</param>
    /// <param name="useParentNestedForList">Whether the object is nested within a collection.</param>
    public DropDownAttribute(string variableNameForList, bool useParentNestedForList)
    {
        VariableNameForList = variableNameForList;
        UseParentNestedForList = useParentNestedForList;
    }
    /// <summary>
    /// Creates a dropdown menu from a <see cref="List{T>}"/> of type <see cref="string"/> found by <paramref name="variableNameForList"/>.
    /// </summary>
    /// <param name="variableNameForList">The name of the <see cref="List{T>}"/> of type <see cref="string"/>.</param>
    public DropDownAttribute(string variableNameForList)
    {
        VariableNameForList = variableNameForList;
    }
}
