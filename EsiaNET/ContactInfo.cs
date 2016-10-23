// EsiaNET version 0.1
// 
// Home page: 
// Author: https://github.com/xeltan

namespace EsiaNET
{
    public enum ContactType
    {
        Mobile, // MBT
        Phone, // PHN
        Email, // EML
        Cem // CEM
    }

    /// <summary>
    /// Provides owner contact information
    /// </summary>
    public class ContactInfo
    {
        public ContactInfo(ContactType type, string value, bool verified = false)
        {
            ContactType = type;
            Value = value;
            Verified = verified;
        }

        /// <summary>
        /// Contact type
        /// </summary>
        public ContactType ContactType { get; }

        /// <summary>
        /// Contact value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Contact is verified or not by government org
        /// </summary>
        public bool Verified { get; }
    }
}