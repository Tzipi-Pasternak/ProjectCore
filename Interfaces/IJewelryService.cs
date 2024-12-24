using ProjectCore.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProjectCore.Interfaces
{
    public interface IJewelryService
    {
        List<Jewelry> GetAll();

        Jewelry Get(int id);

        void Add(Jewelry jewelry);

        void Delete(int id);

        void Update(Jewelry jewelry);

        int Count { get; }
    }
}