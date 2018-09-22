using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Utillities;

namespace ZestMonitor.Api.Services
{
    public class AuthService
    {
        private IUserRepository UserRepository { get; }
        public ConvertHelpers ConvertHelpers { get; }
        public IConfiguration Config { get; }

        public AuthService(IUserRepository userRepository, ConvertHelpers convertHelpers, IConfiguration config)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.ConvertHelpers = convertHelpers ?? throw new ArgumentNullException(nameof(convertHelpers));
            Config = config;
        }

        private bool VerifyPasswordHash(string password, string dbPasswordHash, string passwordSalt)
        {
            // Create new hmac class with existing user salt, hash input pass, compare with existing pass
            using (var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(passwordSalt)))
            {
                var hashBytes = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var hash = Convert.ToBase64String(hashBytes);

                if (dbPasswordHash != hash)
                    return false;

                return true;
            }
        }

        public async Task<bool> Register(UserRegistrationModel user)
        {
            if (user == null)
                return false;

            var newUser = new User();
            newUser.Username = user.Username;

            var passwordModel = this.CreatePasswordHash(user.Password);

            newUser.PasswordHash = passwordModel.PasswordHash;
            newUser.PasswordSalt = passwordModel.PasswordSalt;
            newUser.CreatedAt = DateTime.Now;

            var result = await this.CreateUser(newUser);
            return result;
        }

        private async Task<bool> CreateUser(User entity)
        {
            await this.UserRepository.Add(entity);
            var result = await this.UserRepository.SaveAll();

            return result;
        }

        private PasswordModel CreatePasswordHash(string password)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var result = new PasswordModel();

                // random key generated from hmac hash algo
                result.PasswordSalt = Convert.ToBase64String(hmac.Key);

                // Hash bytes of password
                // (Computes / hashes password with key generated on HMACSHA512 construction)
                var hashBytes = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                result.PasswordHash = Convert.ToBase64String((hashBytes));

                return result;
            }
        }

        public async Task<User> Login(UserLoginModel user)
        {
            if (user == null)
                return null;

            user.Username = user.Username.ToLower();

            var dbUser = await this.UserRepository.Get(user.Username);
            if (dbUser == null)
                return null;

            if (!VerifyPasswordHash(user.Password, dbUser.PasswordHash, dbUser.PasswordSalt))
                return null;

            return dbUser;
        }


        public async Task<bool> UserExists(string username)
        {
            if (!await this.UserRepository.Exists(username))
                return false;

            return true;
        }

        public string CreateJwtAccessToken(User validUser)
        {
            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, validUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, validUser.Username)
                };

            // Create Key for signing
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Config.GetSection("AppSettings:Token").Value));

            // Credentials for signing
            var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha512Signature);

            // Create token descriptor or "placeholder"
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // Create actual complete token from descriptor
            var rawToken = tokenHandler.CreateToken(tokenDescriptor);

            // encode token
            var accessToken = tokenHandler.WriteToken(rawToken);
            return accessToken;
        }
    }
}