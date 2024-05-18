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

    /// <summary>
    /// Extension methods to make modifiers easier to use.
    /// </summary>
    public static class ModifierExtensions
    {
        /// <summary>
        /// Adds a new modifier to an object.
        /// </summary>
        /// <param name="obj">The attributable object.</param>
        /// <param name="attribute">The attribute type.</param>
        /// <param name="modifierTag">The name/ID of the modifier.</param>
        /// <param name="value">The modifier value.</param>
        /// <param name="operation">The operation to perform.</param>
        public static void AddModifier<T>(
            this IAttributable obj,
            Attribute attribute,
            string modifierTag,
            T value,
            Operation operation = Operation.Add)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            if (!obj.HasAttribute(attribute))
            {
                throw new InvalidOperationException("Attribute does not exist on object");
            }

            var instance = obj.GetOrCreateAttribute<T>(attribute);
            instance.AddModifier(modifierTag, new Modifier<T>
            {
                Operation = operation,
                Value = value
            });
        }

        /// <summary>
        /// Removes a modifier from an object.
        /// </summary>
        /// <param name="obj">The attributable object.</param>
        /// <param name="attribute">The attribute type.</param>
        /// <param name="modifierTag">The name/ID of the modifier.</param>
        public static void RemoveModifier<T>(
            this IAttributable obj,
            Attribute attribute,
            string modifierTag)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            obj.GetOrCreateAttribute<T>(attribute).RemoveModifier(modifierTag);
        }
    }
}
