using System;
using UnityEngine;

namespace SmolTags
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NameAttribute : PropertyAttribute
    {
        public string DisplayedName => _displayedName;
        private string _displayedName;
        public NameAttribute(string displayedName)
            : base(true)
        {
            _displayedName = displayedName;
        }
    }
}
