using System;
using System.Collections.Generic;
using System.Linq;

namespace Systems.Attributes
{
    /// <summary>
    /// An instance of an attribute.
    /// </summary>
    public class AttributeInstance<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        /// <summary>
        /// This is the value of the attribute.
        /// </summary>
        public T BaseValue { get; set; }

        /// <summary>
        /// All value modifiers for this instance.
        /// </summary>
        private readonly Dictionary<string, IModifier<T>> _modifiers = new();

        /// <summary>
        /// Getter for the calculated value of the attribute.
        /// </summary>
        public T Value => Calculate();

        /// <summary>
        /// Calculates the value of the attribute.
        /// </summary>
        /// <returns>The value of the attribute, plus modifiers.</returns>
        private T Calculate()
        {
            return _modifiers.Values.Aggregate(BaseValue,
                (current, modifier) => modifier.Calculate(current));
        }

        /// <summary>
        /// Adds a modifier to the attribute instance.
        /// </summary>
        /// <param name="tag">The modifier's unique identifier.</param>
        /// <param name="modifier">A modifier instance to add.</param>
        public void AddModifier(string tag, IModifier<T> modifier) => _modifiers.Add(tag, modifier);

        /// <summary>
        /// Removes the modifier from the attribute instance.
        /// </summary>
        /// <param name="modifier">The modifier instance to remove.</param>
        public void RemoveModifier(IModifier<T> modifier)
        {
            var tag = _modifiers.FirstOrDefault(
                x => x.Value == modifier).Key;
            _modifiers.Remove(tag);
        }

        /// <summary>
        /// Removes the modifier from the attribute instance.
        /// </summary>
        /// <param name="tag">The modifier's unique identifier.</param>
        public void RemoveModifier(string tag) => _modifiers.Remove(tag);
    }
}
