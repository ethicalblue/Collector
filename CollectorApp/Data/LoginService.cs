using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CollectorApp.Data
{
    public class LoginService
    {
        public async Task<bool> IsAuthenticated(ProtectedSessionStorage storage)
        {
            var result = await storage.GetAsync<string>("auth");

            var login = await GetLoginAsync();

            if (login == null)
                return false;

            if (result.Value == null)
                return false;

            return result.Value.Equals(login.SessionToken);
        }

        public async Task<bool> LogInAsync(string login, string password, ProtectedSessionStorage storage)
        {
            var user = db.Login?.FirstOrDefault(a => a.UserName == login);

            if (user != null)
            {
                if (VerifyPasswordHash(user.PasswordHash ?? string.Empty, password, user.PasswordSalt ?? string.Empty).Result)
                {
                    var token = await CreateSession();

                    if (string.IsNullOrEmpty(token))
                        return false;

                    await storage.SetAsync("auth", token);

                    return true;
                }
                else
                {
                    user.LastIncorrectLogin = DateTime.Now;
                    await UpdateLoginAsync(user);
                }
            }
            return false;
        }

        public async Task<bool> LogOffAsync()
        {
            var login = await GetLoginAsync();

            if (login == null)
                return false;

            login.SessionToken = string.Empty;

            await UpdateLoginAsync(login);

            return true;
        }

        public async Task<string> CreateSession()
        {
            var login = await GetLoginAsync();

            string token = string.Empty;

            using (var sha512 = SHA512.Create())
            {
                var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString() + "ethical.blue"));

                token = BitConverter.ToString(bytes).ToLower();
            }

            if (login == null)
                return string.Empty;

            login.SessionToken = token;

            await UpdateLoginAsync(login);

            return token;
        }

        public async Task<Tuple<string, string>> GetPasswordHash(string text)
        {
            var salt = BitConverter.ToString(RandomNumberGenerator.GetBytes(64)).Replace("-", "").ToLower();

            using (var sha512 = SHA512.Create())
            {
                var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(text + salt.Reverse() + "ethical.blue"));

                string hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();

                var tuple = new Tuple<string, string>(hash, salt);

                return await Task.FromResult(tuple);
            }
        }

        public async Task<bool> VerifyPasswordHash(string hash, string text, string salt)
        {
            using (var sha512 = SHA512.Create())
            {
                var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(text + salt.Reverse() + "ethical.blue"));

                string hash2 = BitConverter.ToString(bytes).Replace("-", "").ToLower();

                return await Task.FromResult(hash.Equals(hash2));
            }
        }

        private ApplicationDbContext db;

        public LoginService(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public async Task<Login?> GetLoginAsync()
        {
            if (db.Login == null)
                return null;

            if (db.Login.Any() == false)
                return null;

            return await db.Login.SingleOrDefaultAsync();
        }

        public async Task<Login> AddLoginAsync(Login login)
        {
            if (db.Login != null && db.Login.Any() == false)
            {
                db.Login.Add(login);
                await db.SaveChangesAsync();
            }

            return login;
        }

        public async Task<Login> UpdateLoginAsync(Login login)
        {
            if (db.Login == null)
                throw new NullReferenceException();

            var exists = db.Login.Single(p => p.Id == login.Id);
            if (exists != null)
            {
                db.Update(login);
                await db.SaveChangesAsync();
            }

            return login;
        }

        public async Task DeleteLoginAsync(Login login)
        {
            if (db.Login == null)
                throw new NullReferenceException();

            db.Login.Remove(login);
            await db.SaveChangesAsync();
        }

    }
}
