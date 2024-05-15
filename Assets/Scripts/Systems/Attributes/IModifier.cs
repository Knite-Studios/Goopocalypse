using System;

namespace Systems.Attributes
{
    /// <summary>
    /// Interface for attribute modifier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModifier<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        T Calculate(T baseValue);
    }
}