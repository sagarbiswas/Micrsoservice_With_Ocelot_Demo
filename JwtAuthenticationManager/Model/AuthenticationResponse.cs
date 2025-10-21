namespace JwtAuthenticationManager.Model
{
    public class AuthenticationResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
