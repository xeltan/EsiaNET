namespace EsiaNET.AspNetCore.Authentication
{
    /// <summary>
    ///     Specifies methods for signing and verifying data
    /// </summary>
    public interface ISignProvider
    {
        /// <summary>
        ///     Signs message by X509 certificate
        /// </summary>
        /// <param name="message">Message to sign</param>
        /// <returns>Array of signed bytes</returns>
        byte[] SignMessage(byte[] message);

        /// <summary>
        ///     Verifies the signed message using the provided certificate with public key
        /// </summary>
        /// <param name="alg">The name of the hash algorithm used to create the hash value of the data</param>
        /// <param name="message">The message that was signed</param>
        /// <param name="signature">The signature data to be verified</param>
        /// <returns>true if the signature is valid; otherwise, false</returns>
        bool VerifyMessage(string alg, byte[] message, byte[] signature);
    }
}