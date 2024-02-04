using APIsDaTKSH.Models;
using System.Collections.Generic;
using System.Linq;

public class UserService
{
    private readonly MyDbContext _dbContext;

    public UserService(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void RegisterUser(RegisterModel model)
    {
        if (_dbContext.Users.Any(u => u.Email == model.Email))
        {
            throw new ApplicationException("E-mail já registrado.");
        }

        var newUser = new RegisterModel
        {
            Name = model.Name,
            Email = model.Email,
            Password = model.Password,
            IsAdmin = model.IsAdmin
        };

        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();
    }

    public RegisterModel ValidateUser(string email, string password)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

        return user;
    }
}
