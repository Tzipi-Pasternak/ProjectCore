using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ProjectCore.Models;
using ProjectCore.Interfaces;
using ProjectCore.Services;

namespace ProjectCore.Controllers
{
[ApiController]
[Route("[controller]")]
public class JewelryController : ControllerBase
{
    private IJewelryService jewelryService;
    public JewelryController(IJewelryService jewelryService)
    {
        this.jewelryService = jewelryService;
    }

    [HttpGet]
    public ActionResult<List<Jewelry>> GetAll() =>
        jewelryService.GetAll();

    [HttpGet("{id}")]
    public ActionResult<Jewelry> Get(int id)
    {
        var Jewelry = jewelryService.Get(id);
        if (Jewelry == null)
            return NotFound();
        return Jewelry;
    }

    [HttpPost]
    public IActionResult Create(Jewelry jewelry)
    {        
        jewelryService.Add(jewelry);
        return CreatedAtAction(nameof(Create), new { id = jewelry.Id }, jewelry);
    }  

    
    [HttpPut("{id}")]
    public ActionResult Update(int id, Jewelry jewelry)
    { 
        if(id != jewelry.Id)
            return BadRequest();
        var exitingJewelry = jewelryService.Get(id);
        if (exitingJewelry is null)
            return NotFound();
        jewelryService.Update(jewelry);
        return NoContent();
    } 

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var jewelry = jewelryService.Get(id);
        if (jewelry is null) 
            return NotFound();
        jewelryService.Delete(id);
        return Content(jewelryService.Count.ToString());
    }
}
}