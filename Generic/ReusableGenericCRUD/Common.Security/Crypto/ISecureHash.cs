namespace Common.Security.Crypto
{
    /// <summary>
    /// Interface for hashing capabilities
    /// </summary>
    public interface ISecureHash
    {
        /// <summary>
        /// Perform a predictable hash on a value.
        /// Useful for something that must be encrypted, but also searched for duplicates (like ssn)
        /// </summary>
        /// <param name="value">data (like ssn)</param>
        string PredictableHash(string value);

        /// <summary>
        /// Securly Hash a password into a one-way hashed value
        /// </summary>
        /// <param name="password">password from user</param>
        string HashPassword(string password);

        /// <summary>
        /// Verify a password against the hash
        /// </summary>
        /// <param name="password">password from user</param>
        /// <param name="hash">hash from storage</param>
        /// <returns>true if password matches, false otherwise</returns>
        bool VerifyPasswordHash(string password, string hash);
    }
}
