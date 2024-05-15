using System;
using System.Collections.Generic;

namespace Systems.Attributes
{
    /// <summary>
    /// Base class for entities that have attributes.
    /// </summary>
    public class Attributable : IAttributable
    {
        /// <summary>
        /// The holder for all attributes.
        /// </summary>
        public Dictionary<Attribute, object> Attributes { get; private set; } = new Dictionary<Attribute, object>();

        /// <summary>
        /// Checks if the object has an attribute.
        /// </summary>
        /// <param name="attribute">The attribute type.</param>
        /// <returns>True if the attribute is on the object.</returns>
        public bool HasAttribute(Attribute attribute)
        {
            return Attributes.ContainsKey(attribute);
        }

        /// <summary>
        /// Gets or creates an attribute instance.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="attribute">The attribute type.</param>
        /// <param name="defaultValue">The default value if the attribute doesn't exist.</param>
        /// <returns>The attribute instance.</returns>
        public AttributeInstance<T> GetOrCreateAttribute<T>(
            Attribute attribute, 
            T defaultValue) where T : struct, IComparable, IConvertible, IFormattable
        {
            if (Attributes.TryGetValue(attribute, out var instance))
            {
                return (AttributeInstance<T>)instance;
            }

            var newInstance = new AttributeInstance<T> { BaseValue = defaultValue };
            Attributes[attribute] = newInstance;
            
            return newInstance;
        }

        /// <summary>
        /// Gets the value of an attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="attribute">The attribute type.</param>
        /// <returns>The calculated value of the attribute.</returns>
        public T GetAttributeValue<T>(Attribute attribute) where T : struct, IComparable, IConvertible, IFormattable
        {
            return Attributes.TryGetValue(attribute, out var instance) ? ((AttributeInstance<T>)instance).Value : default;
        }
    }
}
