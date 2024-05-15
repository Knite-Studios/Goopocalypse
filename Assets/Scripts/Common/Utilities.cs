namespace Common
{
    public static class Utilities
    {
        /// <summary>
        /// Attempts to reflectively add two values.
        /// </summary>
        public static T Add<T>(T a, T b)
        {
            dynamic _a = a, _b = b;
            return _a + _b;
        }

        /// <summary>
        /// Attempts to reflectively multiply two values.
        /// </summary>
        public static T Multiply<T>(T a, T b)
        {
            dynamic _a = a, _b = b;
            return _a * _b;
        }
    }
}
