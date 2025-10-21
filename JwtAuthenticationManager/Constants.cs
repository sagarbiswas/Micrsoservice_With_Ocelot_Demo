namespace JwtAuthenticationManager
{
    public class Constants
    {
        public const string SecretKey = "your-super-secret-key-with-at-least-32-characters";
        public const string Issuer = "microservices-demo";
        public const string Audience =  "microservices-clients";
        public const int ExpirationMinutes = 45;
    }
}
