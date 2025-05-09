namespace Domain.Models.Settings
{
    public class KafkaSetting
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string SaslUsername { get; set; } = string.Empty;
        public string SaslPassword { get; set;} = string.Empty;
        public string SaslMechanism { get; set; } = string.Empty;
        public string SecurityProtocol {  get; set; } = string.Empty;
        public int MessageSendMaxRetries { get; set; }
        public int MessageTimeoutMs { get; set; }
        public int RequestTimeoutMs { get; set; }
        public int RetryBackoffMs { get; set; }

        public bool IsAuthentication => !string.IsNullOrEmpty(SaslUsername) && !string.IsNullOrEmpty(SaslPassword);
    }
}
