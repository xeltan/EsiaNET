// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

using System.Security.Cryptography.X509Certificates;

namespace EsiaNET
{
    public interface ISignProvider
    {
        X509Certificate2 GetCertificate();

        X509Certificate2 GetEsiaCertificate();

        byte[] SignMessage(byte[] message, X509Certificate2 certificate);

        bool VerifyMessage(string alg, byte[] message, byte[] signature, X509Certificate2 certificate);
    }
}