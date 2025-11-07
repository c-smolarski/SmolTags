using System;
using UnityEngine;

namespace SmolTags
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public ReadOnlyAttribute()
            : base(true)
        { }
    }
}
