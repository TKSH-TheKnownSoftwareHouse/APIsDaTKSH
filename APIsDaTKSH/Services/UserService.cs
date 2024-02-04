using System;
using System.Threading.Tasks;
using APIsDaTKSH.Models;
using Microsoft.EntityFrameworkCore;
using static AuthController;

public class UserService
{
    private readonly MyDbContext _dbContext;

    public UserService(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RegisterUserAsync(RegisterModel model)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Email == model.Email))
        {
            throw new InvalidOperationException("Email already registered.");
        }
        model.HashPassword();
        _dbContext.Users.Add(model);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);

        if (user != null)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
    public IEnumerable<RegisterModel> GetAllUsers()
    {
        return _dbContext.Users.ToList();
    }
    public RegisterModel GetUserById(int userId)
    {
        return _dbContext.Users.Find(userId);
    }
    public async Task<(RegisterModel user, bool isAdmin)> ValidateUserAsync(string email, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && !string.IsNullOrEmpty(password) && user.VerifyPassword(password))
            {
                return (user, user.IsAdmin);
            }

            return (null, false);
        }

 }
