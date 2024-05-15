using System;
using System.Collections.Generic;

namespace Systems.Attributes
{
    /// <summary>
    /// Interface for entities that have attributes.
    /// </summary>
    public interface IAttributable
    {
        /// <summary>
        /// Dictionary to hold attributes with different types.
        /// </summary>
        Dictionary<Attribute, object> Attributes { get; }
    }

    /// <summary>
    /// Methods for the IAttributable interface.
    /// </summary>
    public static class AttributableMethods
    {
        /// <summary>
        /// Gets the value of an attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="obj">The attributable object.</param>
        /// <param name="attribute">The attribute type.</param>
        /// <returns>The calculated value of the attribute.</returns>
        public static T GetAttributeValue<T>(this IAttributable obj, Attribute attribute)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            return obj.Attributes.TryGetValue(attribute, out var instance) ? ((AttributeInstance<T>)instance).Value : default;
        }

        /// <summary>
        /// Checks if the object has an attribute.
        /// </summary>
        /// <param name="obj">The attributable object.</param>
        /// <param name="attribute">The attribute type.</param>
        /// <returns>True if the attribute is on the object.</returns>
        public static bool HasAttribute(this IAttributable obj, Attribute attribute)
        {
            return obj.Attributes.ContainsKey(attribute);
        }

        /// <summary>
        /// Gets or creates an attribute instance.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="obj">The attributable object.</param>
        /// <param name="attribute">The attribute type.</param>
        /// <param name="defaultValue">The default value if the attribute doesn't exist.</param>
        /// <returns>The attribute instance.</returns>
        public static AttributeInstance<T> GetOrCreateAttribute<T>(this IAttributable obj, Attribute attribute, T defaultValue)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            if (obj.Attributes.TryGetValue(attribute, out var instance))
            {
                return instance as AttributeInstance<T>;
            }

            var newInstance = new AttributeInstance<T> { BaseValue = defaultValue };
            obj.Attributes[attribute] = newInstance;

            return newInstance;
        }
    }
}
