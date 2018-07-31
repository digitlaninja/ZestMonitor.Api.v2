using System;
using System.Threading.Tasks;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Services
{
    public class AuthService
    {
        private IRepository<User> UserRepository {get;}

        public AuthService(IRepository<User> userRepository)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        

        // public async Task<User> Login(string username, string password)
        // {
        // //    var user = await this.UserRepository.Get()
        // }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await this.UserRepository.Add(user);
            await this.UserRepository.SaveAll();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // random key generated from hmac hash algo
                passwordSalt = hmac.Key;

                // hash bytes of password
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public Task<User> UserExists(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}