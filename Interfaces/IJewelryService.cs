using ProjectCore.Models;

namespace ProjectCore.Interfaces
{
    public interface IJewelryService
    {
        List<Jewelry> GetAll();

        Jewelry Get(int id);

        void Add(Jewelry jewelry, int userId);

        void Delete(int id);

        void Update(Jewelry jewelry, int userId);

        int Count { get; }

        void DeleteJewelryByUserId(int userId);
    }
}