namespace Interfaces
{
    public interface IPoolObject
    {
        /// <summary>
        /// Invoked when the object is re-used.
        /// </summary>
        void Reset();
    }
}
