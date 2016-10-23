// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System.Security.Cryptography.X509Certificates;

namespace EsiaNET
{
    /// <summary>
    /// Specifies methods for signing and verifying data
    /// </summary>
    public interface ISignProvider
    {
        /// <summary>
        /// Returns client system certificate.
        /// </summary>
        /// <returns>X509 certificate</returns>
        X509Certificate2 GetCertificate();

        /// <summary>
        /// Returns ESIA certificate.
        /// </summary>
        /// <returns>X509 certificate</returns>
        X509Certificate2 GetEsiaCertificate();

        /// <summary>
        /// Signs message by X509 certificate
        /// </summary>
        /// <param name="message">Message to sign</param>
        /// <param name="certificate">Certificate</param>
        /// <returns>Array of signed bytes</returns>
        byte[] SignMessage(byte[] message, X509Certificate2 certificate);

        /// <summary>
        /// Verifies the signed message using the provided certificate with public key
        /// </summary>
        /// <param name="alg">The name of the hash algorithm used to create the hash value of the data</param>
        /// <param name="message">The message that was signed</param>
        /// <param name="signature">The signature data to be verified</param>
        /// <param name="certificate">The provided certificate with public key</param>
        /// <returns>true if the signature is valid; otherwise, false</returns>
        bool VerifyMessage(string alg, byte[] message, byte[] signature, X509Certificate2 certificate);
    }
}