namespace ASP.NET_Core_API2.Models.Dtos
{
    public class UserToConfirmLoginDto
    {
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordSalt { get; set; } = new byte[0];
    }
}
