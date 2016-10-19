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
    public class DefaultSignProvider : ISignProvider
    {
        private readonly Func<X509Certificate2> _getCertificateAction;
        private readonly Func<X509Certificate2> _getEsiaCertificateAction;

        public DefaultSignProvider(Func<X509Certificate2> action, Func<X509Certificate2> esiaAction = null)
        {
            _getCertificateAction = action;
            _getEsiaCertificateAction = esiaAction;
        }

        public X509Certificate2 GetCertificate()
        {
            return _getCertificateAction();
        }

        public X509Certificate2 GetEsiaCertificate()
        {
            return _getEsiaCertificateAction == null ? null : _getEsiaCertificateAction();
        }

        public byte[] SignMessage(byte[] message, X509Certificate2 certificate)
        {
            var contentInfo = new ContentInfo(message);
            var signedCms = new SignedCms(contentInfo, true);
            var cmsSigner = new CmsSigner(certificate);

            signedCms.ComputeSignature(cmsSigner);

            return signedCms.Encode();
        }

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