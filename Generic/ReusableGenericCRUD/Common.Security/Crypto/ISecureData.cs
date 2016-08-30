namespace Common.Security.Crypto
{
    /// <summary>
    /// Securly Encrypt and Decrypt Data
    /// </summary>
    public interface ISecureData
    {
        /// <summary>
        /// Encrypt bytes to bytes
        /// </summary>
        byte[] Encrypt(byte[] data);

        /// <summary>
        /// Encrypt string to bytes
        /// </summary>
        byte[] Encrypt(string data);

        /// <summary>
        /// Encrypt bytes to Url-Safe Base64 Encoded string
        /// </summary>
        string EncryptToBase64(byte[] data);

        /// <summary>
        /// Encrypt string to Url-Safe Base64 Encoded string
        /// </summary>
        string EncryptToBase64(string data);

        /// <summary>
        /// Encrypt bytes to a Hex String  [0x03, 0xa4] => "03a4"
        /// </summary>
        string EncryptToHex(byte[] data);

        /// <summary>
        /// Encrypt bytes to a Hex String  [0x03, 0xa4] => "03a4"
        /// </summary>
        string EncryptToHex(string data);

        /// <summary>
        /// Decrypt bytes to bytes
        /// </summary>
        byte[] Decrypt(byte[] encrypted);

        /// <summary>
        /// Decrypt Url-Safe Base64 Encoded string to bytes
        /// </summary>
        byte[] DecryptBase64(string encrypted);

        /// <summary>
        /// Decrypt Hex string to bytes
        /// </summary>
        byte[] DecryptHex(string encrypted);

        /// <summary>
        /// Decrypt bytes to string
        /// </summary>
        string DecryptToString(byte[] encrypted);

        /// <summary>
        /// Decrypt Url-Safe Base64 Encoded string to string
        /// </summary>
        string DecryptBase64ToString(string encrypted);

        /// <summary>
        /// Decrypt Hex string to string
        /// </summary>
        string DecryptHexToString(string encrypted);
    }
}