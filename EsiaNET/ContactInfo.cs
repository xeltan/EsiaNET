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

    public class ContactInfo
    {
        public ContactInfo(ContactType type, string value, bool verified = false)
        {
            ContactType = type;
            Value = value;
            Verified = verified;
        }

        public ContactType ContactType { get; }

        public string Value { get; }

        public bool Verified { get; }
    }
}