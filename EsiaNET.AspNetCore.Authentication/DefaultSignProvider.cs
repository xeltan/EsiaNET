using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace EsiaNET.AspNetCore.Authentication
{
    /// <summary>
    ///     Default sign provider implementation
    /// </summary>
    public class DefaultSignProvider : ISignProvider
    {
        private readonly Func<X509Certificate2> _getCertificateAction;
        private readonly Func<X509Certificate2> _getEsiaCertificateAction;

        /// <summary>
        ///     Creates default sign provider with custom certificate handlers
        /// </summary>
        /// <param name="action">Handler that returns client system certificate registered with ESIA</param>
        /// <param name="esiaAction">Handler that returns ESIA certificate. Used only with token verification</param>
        public DefaultSignProvider(Func<X509Certificate2> action, Func<X509Certificate2> esiaAction = null)
        {
            _getCertificateAction = action;
            _getEsiaCertificateAction = esiaAction;
        }

        /// <summary>
        ///     Signs message by X509 certificate
        /// </summary>
        /// <param name="message">Message to sign</param>
        /// <returns>Array of signed bytes</returns>
        public byte[] SignMessage(byte[] message)
        {
            var contentInfo = new ContentInfo(message);
            var signedCms = new SignedCms(contentInfo, true);
            var cmsSigner = new CmsSigner(GetCertificate());

            signedCms.ComputeSignature(cmsSigner);

            return signedCms.Encode();
        }

        /// <summary>
        ///     Verifies the signed message using the provided certificate with public key
        /// </summary>
        /// <param name="alg">The name of the hash algorithm used to create the hash value of the data</param>
        /// <param name="message">The message that was signed</param>
        /// <param name="signature">The signature data to be verified</param>
        /// <returns>true if the signature is valid; otherwise, false</returns>
        public bool VerifyMessage(string alg, byte[] message, byte[] signature)
        {
            if ( alg.ToUpperInvariant() == "RS256" )
            {
                var csp = (RSACryptoServiceProvider)GetEsiaCertificate().PublicKey.Key;

                return csp.VerifyData(message, CryptoConfig.MapNameToOID("SHA256"), signature);
            }

            if ( alg.ToUpperInvariant() == "GOST3410_2012_256" )
            {
                var contentInfo = new ContentInfo(new Oid("1.2.643.7.1.1.1.1"), message);
                var signedCms = new SignedCms(contentInfo, true);

                signedCms.Decode(signature);

                try
                {
                    signedCms.CheckSignature(true);
                }
                catch ( CryptographicException )
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns client system certificate. In default implementation simply call custom handler
        /// </summary>
        /// <returns>X509 certificate</returns>
        public X509Certificate2 GetCertificate()
        {
            return _getCertificateAction();
        }

        /// <summary>
        ///     Returns ESIA certificate. In default implementation simply call custom handler
        /// </summary>
        /// <returns>X509 certificate</returns>
        public X509Certificate2 GetEsiaCertificate()
        {
            return _getEsiaCertificateAction?.Invoke();
        }
    }
}