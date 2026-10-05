namespace Service.Helpers;

internal static class HashHelper
{
    /// <summary>
    /// Generates a hashed string using BCrypt with a specified work factor.
    /// </summary>
    /// <param name="strToHash">The plain text string to hash.</param>
    /// <returns>A hashed version of the string.</returns>
    internal static string GenerateHash(string strToHash)
    {
        return BCrypt.Net.BCrypt.HashPassword(strToHash, workFactor: 12);
    }
}
