using System;
using System.Collections.Generic;

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
        private readonly List<IModifier<T>> _modifiers = new List<IModifier<T>>();

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
            var value = BaseValue;
            foreach (var modifier in _modifiers)
            {
                value = modifier.Calculate(value);
            }

            return value;
        }

        /// <summary>
        /// Adds a modifier to the attribute instance.
        /// </summary>
        /// <param name="modifier">The modifier to add.</param>
        public void AddModifier(IModifier<T> modifier)
        {
            _modifiers.Add(modifier);
        }
    }
}
