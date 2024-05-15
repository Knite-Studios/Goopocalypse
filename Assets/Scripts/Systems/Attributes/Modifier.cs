namespace Systems.Attributes
{
    /// <summary>
    /// A modifier is a mutable value which influences the value of an attribute.
    /// The modifier is only applied to the **base value**, it cannot modify the final result directly.
    /// </summary>
    public class Modifier
    {
        /// <summary>
        /// This is the value of the modifier.
        /// This should not be used directly.
        /// </summary>
        public float Value { get; set; } = 0.0f;

        /// <summary>
        /// This is the mathematical operation to apply to the base value.
        /// </summary>
        public Operation Operation { get; set; } = Operation.Add;

        /// <summary>
        /// Calculates the value of the modifier.
        /// </summary>
        /// <param name="baseValue">The initial value of an attribute.</param>
        /// <returns>The modifier-influenced value.</returns>
        public float Calculate(float baseValue)
        {
            return Operation switch
            {
                Operation.Add => baseValue + Value,
                Operation.Multiply => baseValue * Value,
                _ => baseValue
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
