using System;
using System.Threading.Tasks;
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

        public AuthService(IUserRepository userRepository, ConvertHelpers convertHelpers)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.ConvertHelpers = convertHelpers ?? throw new ArgumentNullException(nameof(convertHelpers));
        }

        private bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt)
        {
            // Create new hmac class with existing user salt, hash input pass, compare with existing pass
            using (var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(passwordSalt)))
            {
                var hashBytes = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    if (hashBytes[i] != passwordHash[i]) 
                        return false;
                }
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

        public async Task<bool> Login(UserLoginModel user)
        {
              if (user == null)
                return false;

            // user.Username = user.Username.ToLower();
            
            var existingUser = await this.UserRepository.Get(user.Username);
            if (existingUser == null)
                return false;

            if (!VerifyPasswordHash(user.Password, existingUser.PasswordHash, existingUser.PasswordSalt))
                return false;

            return true;
        }


        public async Task<bool> UserExists(string username)
        {
            if (!await this.UserRepository.Exists(username))
                return false;

            return true;
        }
    }
}