using System.Text.Json;
using ProjectCore.Interfaces;
using ProjectCore.Models;

namespace ProjectCore.Services
{
    public class JewelryService : IJewelryService
    {
        int nextId;
        string text;
        private List<Jewelry>? jewelryList { get; }

        public JewelryService()
        {
            text = Path.Combine(
                "Data",
                "Jewelry.json"
            );

            using (var jsonOpen = File.OpenText(text))
            {
                jewelryList = JsonSerializer.Deserialize<List<Jewelry>>(jsonOpen.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            nextId = jewelryList != null ? jewelryList[jewelryList.Count - 1].Id + 1 : 1;
        }

        private void saveToFile()
        {
            File.WriteAllText(text, JsonSerializer.Serialize(jewelryList));
        }

        public List<Jewelry> GetAll() => jewelryList ?? new List<Jewelry>();

        public Jewelry Get(int id) => jewelryList?.FirstOrDefault(j => j.Id == id) ?? new Jewelry();

        public void Add(Jewelry jewelry, int userId)
        {
            jewelry.Id = nextId++;
            jewelry.UserId = userId;
            jewelryList?.Add(jewelry);
            saveToFile();
        }

        public void Update(Jewelry jewelry, int userId)
        {
            if (jewelryList == null)
                return;
            var index = jewelryList.FindIndex(j => j.Id == jewelry.Id);
            if (index == -1)
                return;
            jewelry.UserId = userId;
            jewelryList[index] = jewelry;
            saveToFile();
        }

        public void Delete(int id)
        {
            var jewelry = Get(id);
            if (jewelry is null)
                return;
            jewelryList?.Remove(jewelry);
            saveToFile();
        }

        public int Count { get => jewelryList?.Count() ?? 0; }

        public void DeleteJewelryByUserId(int userId)
        {
            var itemsToRemove = jewelryList?.Where(jewelry => jewelry.UserId == userId).ToList();
            if (itemsToRemove == null)
                return;
            foreach (var item in itemsToRemove)
            {
                jewelryList?.Remove(item);
            }
            saveToFile();
        }
    }

    public static class JewelryServiceHelper
    {
        public static void AddJewelryService(this IServiceCollection services)
        {
            services.AddSingleton<IJewelryService, JewelryService>();
        }
    }
}