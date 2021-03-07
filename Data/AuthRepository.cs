using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rpg.Models;

namespace rpg.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                response.success = false;
                response.message = "User not found.";
            }
            else if (!this.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.success = false;
                response.message = "Wrong Password.";
            }
            else {
                response.Data = user.Id.ToString();
            }

            return response;

        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();

            if (await this.UserExists(user.Username))
            {
                response.success = false;
                response.message = "User already exist";
                return response;
            }
            this.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            response.Data = user.Id;
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(user => user.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmec = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmec.Key;
                passwordHash = hmec.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmec = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmec.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}