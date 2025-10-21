namespace CameraTuanA.Models
{
    using Microsoft.AspNetCore.Identity;

    public class PasswordHelper
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string password)
        {
            return _hasher.HashPassword(null, password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return _hasher.VerifyHashedPassword(null, hashedPassword, providedPassword) == PasswordVerificationResult.Success;
        }
    }

}
