namespace System.Net.Http
{
    public class AccessToken
    {
        public string Type { get; }

        public string Value { get; }

        public DateTime Issued { get; }

        public TimeSpan Validity { get; }

        public DateTime ExpiryUtc => Issued.Add(Validity);

        public bool HasExpired => DateTime.UtcNow > ExpiryUtc;

        public AccessToken(string value, string type, DateTime issued, TimeSpan validity)
        {
            Value = value;
            Type = type;
            Issued = issued;
            Validity = validity;
        }
    }
}
