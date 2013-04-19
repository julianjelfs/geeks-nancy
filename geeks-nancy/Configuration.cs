namespace geeks_nancy
{
    public class Configuration
    {
        static Configuration()
        {
            EncryptionKey = "SuperSecretPass";
            HmacKey = "UberSuperSecret";
            Password = "password";
        }

        public static string ConnectionString { get; set; }

        public static string EncryptionKey { get; set; }

        public static string HmacKey { get; set; }

        public static string Password { get; set; }
    }
}