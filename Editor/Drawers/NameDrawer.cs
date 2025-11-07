using UnityEditor;
using UnityEngine;

namespace SmolTags.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(NameAttribute))]
    public class NameDrawer : PropertyDrawer
    {
        private NameAttribute _nameAttribute;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _nameAttribute = (NameAttribute)attribute;
            label.text = _nameAttribute.DisplayedName;
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = _nameAttribute.DisplayedName;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
