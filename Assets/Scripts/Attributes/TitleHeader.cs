using System;
using UnityEngine;

namespace Attributes
{
    /// <summary>
    /// Stylized header attribute for serialized fields in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TitleHeaderAttribute : PropertyAttribute
    {
        public readonly string Header;
    
        public TitleHeaderAttribute(string header) => Header = header;
    }
}