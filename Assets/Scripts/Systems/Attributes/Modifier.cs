using System;
using Common;

namespace Systems.Attributes
{
    /// <summary>
    /// A modifier is a mutable value which influences the value of an attribute.
    /// The modifier is only applied to the **base value**, it cannot modify the final result directly.
    /// </summary>
    public class Modifier<T> : IModifier<T>
        where T : struct, IComparable, IConvertible, IFormattable
    {
        /// <summary>
        /// This is the value of the modifier.
        /// This should not be used directly.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// This is the mathematical operation to apply to the base value.
        /// </summary>
        public Operation Operation { get; set; } = Operation.Add;

        /// <summary>
        /// Calculates the value of the modifier.
        /// </summary>
        /// <param name="baseValue">The initial value of an attribute.</param>
        /// <returns>The modifier-influenced value.</returns>
        public T Calculate(T baseValue)
        {
            return Operation switch
            {
                Operation.Add => Utilities.Add(baseValue, Value),
                Operation.Multiply => Utilities.Multiply(baseValue, Value),
                _ => throw new InvalidOperationException("Unsupported operation")
            };
        }
    }

    /// <summary>
    /// The applicable operations for a modifier.
    /// </summary>
    public enum Operation
    {
        Add,
        Multiply
    }
}
