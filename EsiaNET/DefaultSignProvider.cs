// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace EsiaNET
{
    /// <summary>
    /// Default sign provider implementation
    /// </summary>
    public class DefaultSignProvider : ISignProvider
    {
        private readonly Func<X509Certificate2> _getCertificateAction;
        private readonly Func<X509Certificate2> _getEsiaCertificateAction;

        /// <summary>
        /// Creates default sign provider with custom certificate handlers
        /// </summary>
        /// <param name="action">Handler that returns client system certificate registered with ESIA</param>
        /// <param name="esiaAction">Handler that returns ESIA certificate. Used only with token verification</param>
        public DefaultSignProvider(Func<X509Certificate2> action, Func<X509Certificate2> esiaAction = null)
        {
            _getCertificateAction = action;
            _getEsiaCertificateAction = esiaAction;
        }

        /// <summary>
        /// Returns client system certificate. In default implementation simply call custom handler
        /// </summary>
        /// <returns>X509 certificate</returns>
        public X509Certificate2 GetCertificate()
        {
            return _getCertificateAction();
        }

        /// <summary>
        /// Returns ESIA certificate. In default implementation simply call custom handler
        /// </summary>
        /// <returns>X509 certificate</returns>
        public X509Certificate2 GetEsiaCertificate()
        {
            return _getEsiaCertificateAction == null ? null : _getEsiaCertificateAction();
        }

        /// <summary>
        /// Signs message by X509 certificate
        /// </summary>
        /// <param name="message">Message to sign</param>
        /// <param name="certificate">Certificate</param>
        /// <returns>Array of signed bytes</returns>
        public byte[] SignMessage(byte[] message, X509Certificate2 certificate)
        {
            var contentInfo = new ContentInfo(message);
            var signedCms = new SignedCms(contentInfo, true);
            var cmsSigner = new CmsSigner(certificate);

            signedCms.ComputeSignature(cmsSigner);

            return signedCms.Encode();
        }

        /// <summary>
        /// Verifies the signed message using the provided certificate with public key
        /// </summary>
        /// <param name="alg">The name of the hash algorithm used to create the hash value of the data</param>
        /// <param name="message">The message that was signed</param>
        /// <param name="signature">The signature data to be verified</param>
        /// <param name="certificate">The provided certificate with public key</param>
        /// <returns>true if the signature is valid; otherwise, false</returns>
        public bool VerifyMessage(string alg, byte[] message, byte[] signature, X509Certificate2 certificate)
        {
            if ( alg.ToUpperInvariant() == "RS256" )
            {
                var csp = (RSACryptoServiceProvider)certificate.PublicKey.Key;

                return csp.VerifyData(message, CryptoConfig.MapNameToOID("SHA256"), signature);
            }

            return false;
        }
    }
}