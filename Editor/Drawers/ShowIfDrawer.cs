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
        private ShowIfAttribute _showAttribute;
        private SerializedProperty _showProperty;
        private bool _prevCanShow, _canShow;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _showAttribute = (ShowIfAttribute)attribute;
            _showProperty = property.serializedObject.FindProperty(_showAttribute.PropertyName);
            _canShow = CanShow();

            if (!_canShow && _showAttribute.hide)
                return 0f;
            return EditorGUI.GetPropertyHeight(property, label, true);
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
            if (_showProperty.type == nameof(Enum))
                return _showProperty.enumValueIndex.Equals((int)_showAttribute.ShowValue);
            return _showProperty.boxedValue.Equals(_showAttribute.ShowValue);
        }
    }
}
