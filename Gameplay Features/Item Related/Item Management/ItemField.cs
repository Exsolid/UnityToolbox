using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ItemField
{
    private string _fieldName;
    public string FieldName
    {
        get { return _fieldName; }
    }

    private Type _fieldType;
    public Type FieldType
    {
        get { return _fieldType; }
    }

    private int _intValue;
    private float _floatValue;
    private string _stringValue;
    private bool _boolValue;

    public ItemField(string fieldName, bool boolValue)
    {
        _fieldName = fieldName;
        _fieldType = typeof(bool);
        _boolValue = boolValue;
    }

    public ItemField(string fieldName, string stringValue)
    {
        _fieldName = fieldName;
        _fieldType = typeof(string);
        _stringValue = stringValue;
    }

    public ItemField(string fieldName, int intValue)
    {
        _fieldName = fieldName;
        _fieldType = typeof(int);
        _intValue = intValue;
    }

    public ItemField(string fieldName, float floatValue)
    {
        _fieldName = fieldName;
        _fieldType = typeof(float);
        _floatValue = floatValue;
    }

    public dynamic GetValue()
    {
        if (_fieldType.Equals(typeof(int)))
        {
            return _intValue;
        }
        else if (_fieldType.Equals(typeof(float)))
        {
            return _floatValue;
        }
        else if (_fieldType.Equals(typeof(bool)))
        {
            return _boolValue;
        }

        return _stringValue;
    }

    public override bool Equals(object obj)
    {
        return FieldName.Equals(obj);
    }

    public override int GetHashCode()
    {
        return FieldName.GetHashCode();
    }
}
