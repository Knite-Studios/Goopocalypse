using System;
namespace Systems.Attributes
{
    /// <summary>
    /// A modifier is a mutable value which influences the value of an attribute.
    /// The modifier is only applied to the **base value**, it cannot modify the final result directly.
    /// </summary>
    public class Modifier<T> : IModifier<T> where T : struct, IComparable, IConvertible, IFormattable
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
            switch (Operation)
            {
                case Operation.Add:
                    return Add(baseValue, Value);
                case Operation.Multiply:
                    return Multiply(baseValue, Value);
                default:
                    throw new InvalidOperationException("Unsupported operation");
            }
        }

        private static T Add(T a, T b)
        {
            return Type.GetTypeCode(typeof(T)) switch
            {
                TypeCode.Int32 => (T)(object)((int)(object)a + (int)(object)b),
                TypeCode.UInt32 => (T)(object)((uint)(object)a + (uint)(object)b),
                TypeCode.Single => (T)(object)((float)(object)a + (float)(object)b),
                TypeCode.Double => (T)(object)((double)(object)a + (double)(object)b),
                _ => throw new InvalidOperationException("Unsupported type for addition")
            };
        }

        private static T Multiply(T a, T b)
        {
            return Type.GetTypeCode(typeof(T)) switch
            {
                TypeCode.Int32 => (T)(object)((int)(object)a * (int)(object)b),
                TypeCode.UInt32 => (T)(object)((uint)(object)a * (uint)(object)b),
                TypeCode.Single => (T)(object)((float)(object)a * (float)(object)b),
                TypeCode.Double => (T)(object)((double)(object)a * (double)(object)b),
                _ => throw new InvalidOperationException("Unsupported type for multiplication")
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
