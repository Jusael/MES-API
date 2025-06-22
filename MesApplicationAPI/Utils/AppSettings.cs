namespace MesApplicationAPI.Utils
{
    public static class AppSettings
    {
        public static string SecretKey { get; private set; } = string.Empty;

        public static void Init(IConfiguration config)
        {
            SecretKey = config["JwtSettings:SecretKey"]!;
        }
    }
}