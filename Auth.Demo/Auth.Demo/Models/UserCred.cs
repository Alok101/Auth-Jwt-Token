namespace Auth.Demo.Models
{
    public class UserCred
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class RefreshCred
    {
        public string RefreshToken { get; set; }
        public string JwtToken { get; set; }
    }
}
