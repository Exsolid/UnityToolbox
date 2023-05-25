using System;

namespace UnityToolbox.Item.Management
{
    /// <summary>
    /// A class summarizing a field by its type, name and value. Required for all classes inheriting <see cref="ItemDefinition"/>.
    /// </summary>
    [Serializable]
    public class ItemField
    {
        private string _fieldName;
        /// <summary>
        /// The assembly accurate name of the field.
        /// </summary>
        public string FieldName
        {
            get { return _fieldName; }
        }

        private Type _fieldType;
        /// <summary>
        /// The field type.
        /// </summary>
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

        /// <summary>
        /// Returns the value of the field. The type is equal to the <see cref="FieldType"/>.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Overriden to disregard the reference and check for an equal name.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return FieldName.Equals(obj);
        }

        /// <summary>
        /// Overriden to disregard the reference and check for an equal name.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return FieldName.GetHashCode();
        }
    }
}
