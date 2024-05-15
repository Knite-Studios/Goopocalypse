using System;

namespace Systems.Attributes
{
    /// <summary>
    /// Interface for an attribute modifier.
    /// </summary>
    public interface IModifier<T>
        where T : struct, IComparable, IConvertible, IFormattable
    {
        T Calculate(T baseValue);
    }
}
