using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProjectCore.Interfaces;
using ProjectCore.Models;

namespace ProjectCore.Services
{
    public class UserService : IUserServise
    {
        int nextId;
        string text;
        List<User>? userList { get; }
        ITokenService tokenService;

        public UserService(ITokenService tokenService)
        {
            text = Path.Combine(
                "Data",
                "User.json"
            );

            using (var jsonOpen = File.OpenText(text))
            {
                userList = JsonSerializer.Deserialize<List<User>>(jsonOpen.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            this.tokenService = tokenService;
            nextId = userList != null ? userList[userList.Count - 1].Id + 1 : 1;
        }

        private void saveToFile()
        {
            File.WriteAllText(text, JsonSerializer.Serialize(userList));
        }

        // public List<User>? GetAll() => userList;
        public List<User> GetAll() => userList ?? new List<User>();

        // public User? Get(int id) => userList?.FirstOrDefault(j => j.Id == id);
        public User Get(int id) => userList?.FirstOrDefault(j => j.Id == id) ?? new User();

        public void Add(User user)
        {
            user.Id = nextId++;
            userList?.Add(user);
            saveToFile();
        }

        public void Update(User user, string role)
        {
            if (userList == null)
                return;
            var index = userList.FindIndex(j => j.Id == user.Id);
            if (index == -1)
                return;
            user.Role = role;
            userList[index] = user;
            saveToFile();
        }

        public void Delete(int id)
        {
            var user = Get(id);
            if (user is null)
                return;
            userList?.Remove(user);
            saveToFile();
        }

        public int Count { get => userList?.Count() ?? 0; }

        public IActionResult? Login(User user)
        {
            User? findUser = userList?.FirstOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);
            if (findUser == null)
                return null;
            var claims = new List<Claim>
            {
                new Claim("id", findUser.Id+""),
                new Claim("userName", findUser.UserName ?? "null"),
                new Claim("type", findUser.Role ?? "User"),
            };
            var token = tokenService.GetToken(claims);
            return new OkObjectResult(tokenService.WriteToken(token));
        }
    }

    public static class UserServiceHelper
    {
        public static void AddUserService(this IServiceCollection services)
        {
            services.AddSingleton<IUserServise, UserService>();
        }
    }
}