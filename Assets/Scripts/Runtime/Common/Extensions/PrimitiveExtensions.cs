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

        /// <summary>
        /// Converts a string to a Steam ID.
        /// </summary>
        /// <param name="value">The string ID value.</param>
        /// <returns>The Steam ID object.</returns>
        public static CSteamID ToSteamId(this string value)
            => value == "localhost" ?
                SteamUser.GetSteamID() :
                new CSteamID(ulong.Parse(value));
    }
}
