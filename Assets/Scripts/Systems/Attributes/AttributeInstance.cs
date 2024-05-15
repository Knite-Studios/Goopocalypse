using System.Collections.Generic;

namespace Systems.Attributes
{
    /// <summary>
    /// An instance of an attribute.
    /// </summary>
    public class AttributeInstance
    {
        /// <summary>
        /// This is the value of the attribute.
        /// </summary>
        public float BaseValue { get; set; } = 0.0f;

        /// <summary>
        /// All value modifiers for this instance.
        /// </summary>
        private readonly Dictionary<string, Modifier> _modifiers = new();

        /// <summary>
        /// Getter for the calculated value of the attribute.
        /// </summary>
        public float Value => Calculate();

        /// <summary>
        /// Calculates the value of the attribute.
        /// Formula: BaseValue + Sum(Modifiers)
        /// </summary>
        /// <param name="collapse">
        /// If true, the modifiers will be collapsed into the base value.
        /// If false, the modifiers will be added to the base value.
        /// </param>
        /// <returns>The value of the attribute, plus modifiers.</returns>
        private float Calculate(bool collapse = false)
        {
            var value = BaseValue;
            foreach (var modifier in _modifiers.Values)
            {
                if (collapse)
                {
                    value = modifier.Calculate(value);
                }
                else
                {
                    value += modifier.Calculate(value);
                }
            }

            return value;
        }
    }
}
