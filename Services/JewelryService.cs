using ProjectCore.Interfaces;
using ProjectCore.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProjectCore.Services
{
    public class JewelryService: IJewelryService
    {
        List<Jewelry> jewelryList { get;}
        int nextId = 4;
        public JewelryService()
        {
            jewelryList = new List<Jewelry> 
            {
                new Jewelry { Id = 1, Name = "pearlNecklace", Price = 1500, Category = "necklace" },
                new Jewelry { Id = 2, Name = "pandoraBracelet", Price = 250, Category = "bracelet" },
                new Jewelry { Id = 3, Name = "goldWatch", Price = 6000, Category = "watch" }
            };
        }

        public List<Jewelry> GetAll() => jewelryList;

        public Jewelry Get(int id) => jewelryList.FirstOrDefault(j => j.Id == id);

        public void Add(Jewelry jewelry)
        {
            jewelry.Id = nextId++;
            jewelryList.Add(jewelry);
        }

        public void Update(Jewelry jewelry)
        {
            var index = jewelryList.FindIndex(j => j.Id == jewelry.Id);
            if(index == -1)
                return;
            jewelryList[index] = jewelry;
        }

        public void Delete(int id)
        {
            var jewelry = Get(id);
            if (jewelry is null)
                return;
            jewelryList.Remove(jewelry);
        }

        public int Count { get => jewelryList.Count(); }
    }
}