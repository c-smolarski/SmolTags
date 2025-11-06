using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmolTags.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        private const char PROPERTY_PATH_SPLITTER = '.';
        private const string ARRAY_PATH = "Array";
        private const string ARRAY_DATA_PATH = "data[";

        private ShowIfAttribute _showAttribute;
        private SerializedProperty _showProperty;
        private bool _prevCanShow, _canShow;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _showAttribute = (ShowIfAttribute)attribute;
            _showProperty = GetProperty(property);

            _canShow = CanShow();

            if (!_canShow && _showAttribute.hide)
                return 0f;
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private SerializedProperty GetProperty(SerializedProperty property)
        {
            SerializedProperty newProp = property.serializedObject.FindProperty(_showAttribute.PropertyName);
            if (newProp != null)
                return newProp;

            string[] paths = property.propertyPath.Split(PROPERTY_PATH_SPLITTER);
            newProp = property.serializedObject.FindProperty(paths[0]);
            for (int i = 1; i < paths.Length - 1; i++)
            {
                if (paths[i] == ARRAY_PATH)
                {
                    newProp = newProp.GetArrayElementAtIndex(int.Parse(paths[i + 1][ARRAY_DATA_PATH.Length..^1]));
                    i += 1;
                }
                else
                    newProp = newProp.FindPropertyRelative(paths[i]);
            }
            return newProp.FindPropertyRelative(_showAttribute.PropertyName);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_canShow)
                EditorGUI.PropertyField(position, property, label, true);
            else
            {
                if (!_showAttribute.hide)
                {
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property, label, true);
                    GUI.enabled = true;
                }

                if (_prevCanShow == true && _showAttribute.resetIfDisabled)
                    ResetProperty(property, _showAttribute.resetValue);
            }

            if (_prevCanShow != _canShow)
                _prevCanShow = _canShow;
        }

        private void ResetProperty(SerializedProperty property, object resetValue)
        {
            object defaultValue;
            if (property.isArray)
            {
                IList valueList;

                try
                {
                    valueList = (IList)resetValue;
                    property.arraySize = valueList.Count;
                }
                catch
                {
                    valueList = new List<object>();
                    for (int i = 0; i < property.arraySize; i++)
                        valueList.Add(GetDefaultValue(property.GetArrayElementAtIndex(i).boxedValue));
                }

                for (int i = 0; i < property.arraySize; i++)
                    ResetProperty(property.GetArrayElementAtIndex(i), valueList[i]);
                return;
            }

            if (!property.boxedValue.Equals(resetValue))
            {
                defaultValue = GetDefaultValue(property.boxedValue);
                property.boxedValue = resetValue != default ? resetValue : defaultValue;
            }
        }

        private object GetDefaultValue(object obj)
        {
            Type t = obj.GetType();
            if (t.IsValueType || t.IsGenericType)
                return Activator.CreateInstance(t);
            return null;
        }

        private bool CanShow()
        {
            if (_showProperty == null)
                throw new Exception("Invalid property");

            if (_showProperty.type == nameof(Enum))
                return _showProperty.enumValueIndex.Equals((int)_showAttribute.ShowValue);
            return _showProperty.boxedValue.Equals(_showAttribute.ShowValue);
        }
    }
}