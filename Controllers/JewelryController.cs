using Microsoft.AspNetCore.Mvc;
using ProjectCore.Models;

namespace ProjectCore.Controllers;

[ApiController]
[Route("[controller]")]
public class JewelryController : ControllerBase
{
    private static List<Jewelry> list;
    static JewelryController()
    {
        list = new List<Jewelry> 
        {
            new Jewelry { Id = 1, Name = "pearlNecklace", Price = 1500, Category = "necklace" },
            new Jewelry { Id = 2, Name = "pandoraBracelet", Price = 250, Category = "bracelet" },
            new Jewelry { Id = 3, Name = "goldWatch", Price = 6000, Category = "watch" }
        };
    }

    [HttpGet]
    public IEnumerable<Jewelry> Get()
    {
        return list;
    }

    [HttpGet("{id}")]
    public ActionResult<Jewelry> Get(int id)
    {
        var Jewelry = list.FirstOrDefault(p => p.Id == id);
        if (Jewelry == null)
            return BadRequest("invalid id");
        return Jewelry;
    }

    [HttpPost]
    public ActionResult Insert(Jewelry newJewelry)
    {        
        var maxId = list.Max(p => p.Id);
        newJewelry.Id = maxId + 1;
        list.Add(newJewelry);
        return CreatedAtAction(nameof(Insert), new { id = newJewelry.Id }, newJewelry);
    }  

    
    [HttpPut("{id}")]
    public ActionResult Update(int id, Jewelry newJewelry)
    { 
        var oldJewelry = list.FirstOrDefault(p => p.Id == id);
        if (oldJewelry == null) 
            return BadRequest("invalid id");
        if (newJewelry.Id != oldJewelry.Id)
            return BadRequest("id mismatch");
        oldJewelry.Name = newJewelry.Name;
        oldJewelry.Price = newJewelry.Price;
        oldJewelry.Category = newJewelry.Category;
        return NoContent();
    } 

    [HttpDelete("{id}")]
    public ActionResult Remove(int id)
    {
        var oldJewelry = list.FirstOrDefault(p => p.Id == id);
        if (oldJewelry == null) 
            return BadRequest("invalid id");
        list.Remove(oldJewelry);
        return NoContent();
    }
}
