namespace Datalayer.ViewModels
{
    public class ApiSettings
    {
        public string LoginPath { get; set; }

        public string LogoutPath { get; set; }
        public string RefreshTokenPath { get; set; }
        public string AccessTokenObjectKey { get; set; }
        public string RefreshTokenObjectKey { get; set; }
        public string UserObjectKey { get; set; }
        public string PersonObjectKey { get; set; }
        
        public string AdminRoleName { get; set; }
        public string Consts { get; set; }
        public string ApiPath { get; set; }
    }

    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class FirstLogin
    {
        public string Username { get; set; }
    }
    public class SecondLogin
    {
        public string Password { get; set; }
    }
}