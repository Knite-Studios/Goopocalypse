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

        /// <summary>
        /// Checks if the object has an attribute.
        /// </summary>
        /// <param name="attribute">The attribute type.</param>
        /// <returns>True if the attribute is on the object.</returns>
        bool HasAttribute(Attribute attribute);

        /// <summary>
        /// Gets or creates an attribute instance.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="attribute">The attribute type.</param>
        /// <param name="defaultValue">The default value if the attribute doesn't exist.</param>
        /// <returns>The attribute instance.</returns>
        AttributeInstance<T> GetOrCreateAttribute<T>(Attribute attribute, T defaultValue)
            where T : struct, IComparable, IConvertible, IFormattable;

        /// <summary>
        /// Gets the value of an attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="attribute">The attribute type.</param>
        /// <returns>The calculated value of the attribute.</returns>
        T GetAttributeValue<T>(Attribute attribute) where T : struct, IComparable, IConvertible, IFormattable;
    }
}