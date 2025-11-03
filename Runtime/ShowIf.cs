using System;
using UnityEngine;

namespace SmolTags
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string PropertyName => _propertyName;
        public object ShowValue => _showValue;

        private readonly string _propertyName;
        private readonly object _showValue;

        /// <summary>
        /// If true, property will be hidden if disabled. Defaults to true.
        /// </summary>
        public bool hide = true;

        /// <summary>
        /// If true, property will automatically be set to its default value when disabled. Defaults to true.
        /// </summary>
        public bool resetIfDisabled = true;

        /// <summary>
        /// Overrides property's default value when value is reset.
        /// </summary>
        public object resetValue = default;

        public ShowIfAttribute(string propertyName, object showValue)
            : base(true)
        {
            _propertyName = propertyName;
            _showValue = showValue;
        }
    }
}
