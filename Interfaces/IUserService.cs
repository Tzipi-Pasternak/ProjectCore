using Microsoft.AspNetCore.Mvc;
using ProjectCore.Models;

namespace ProjectCore.Interfaces
{
    public interface IUserServise
    {
        List<User> GetAll();

        User Get(int id);

        void Add(User user);

        void Delete(int id);

        void Update(User user, string role);

        int Count { get; }

        IActionResult? Login(User user);
    }
}
