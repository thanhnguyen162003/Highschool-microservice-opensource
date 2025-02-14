namespace Domain.Models.Settings
{
    public class RedisSetting
    {
        public string ClientName { get; set; } = null!;
        public string Host { get; set; } = null!;
        public int Port { get; set; } = 6379;
        public string User { get; set; } = null!;
        public string Password { get; set; } = null!;

        public string ConnectionString => $"{Host}:{Port},password={Password},user={User}";
    }
}
