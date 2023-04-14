namespace ASP.NET_Core_API2.Models.Dtos
{
    public class UserToRegisterDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string PasswordConfirm { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Gender { get; set; } = "";
    }
}
