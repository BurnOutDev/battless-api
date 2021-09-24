namespace Domain.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }

        // refresh token Time To Live (in days)
        public int RefreshTokenTTL { get; set; }

        public string EmailFrom { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }

        // client app requests origin
        public string ClientAppUrl { get; set; }
    }
}