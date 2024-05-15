using System.Collections.Generic;
using JetBrains.Annotations;

namespace Systems.Attributes
{
    /// <summary>
    /// This should be implemented on any "living" entity which has game-oriented statistics.
    /// </summary>
    public interface IAttributable
    {
        /// <summary>
        /// The holder for all attributes.
        /// </summary>
        Dictionary<Attribute, AttributeInstance> Attributes { get; }

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
        /// Returns an attribute instance.
        /// </summary>
        /// <param name="attribute">The attribute type.</param>
        /// <returns>The attribute instance, or null.</returns>
        [CanBeNull]
        public AttributeInstance GetAttribute(Attribute attribute)
        {
            return HasAttribute(attribute) ? Attributes[attribute] : null;
        }

        /// <summary>
        /// Reads the value of an attribute.
        /// </summary>
        /// <param name="attribute">The attribute type.</param>
        /// <param name="fallback">The default value to use if the attribute is new.</param>
        /// <returns>The calculated attribute value.</returns>
        public float AttributeValue(
            Attribute attribute,
            float fallback = 0.0f)
        {
            return ReadAttribute(attribute, fallback).Value;
        }

        /// <summary>
        /// Gets an instance of an attribute.
        /// Creates the attribute if it doesn't exist.
        /// </summary>
        /// <param name="attribute">The attribute type.</param>
        /// <param name="fallback">The default base value to use if the attribute doesn't exist.</param>
        /// <returns>The attribute instance.</returns>
        public AttributeInstance ReadAttribute(
            Attribute attribute,
            float fallback = 0.0f)
        {
            if (HasAttribute(attribute))
            {
                return GetAttribute(attribute);
            }

            // Create the attribute instance.
            var instance = new AttributeInstance
            {
                BaseValue = fallback
            };
            Attributes[attribute] = instance;

            return GetAttribute(attribute);
        }
    }
}
