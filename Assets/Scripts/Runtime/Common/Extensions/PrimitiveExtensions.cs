using Steamworks;

namespace Common.Extensions
{
    public static class PrimitiveExtensions
    {
        /// <summary>
        /// Converts a long to a Steam ID.
        /// </summary>
        /// <param name="value">The long ID value.</param>
        /// <returns>The Steam ID object.</returns>
        public static CSteamID ToSteamId(this ulong value) => new(value);
    }
}
