using Microsoft.AspNetCore.Mvc;
using ProjectCore.Models;
using ProjectCore.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ProjectCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "User")]
    public class JewelryController : ControllerBase
    {
        private IJewelryService jewelryService;

        public JewelryController(IJewelryService jewelryService)
        {
            this.jewelryService = jewelryService;
        }

        private int? UserId
        {
            get
            {
                var idClaim = User.FindFirst("id");
                return idClaim != null && int.TryParse(idClaim.Value, out int userId) ? userId : (int?)null;
            }
        }

        private string? UserRole
        {
            get
            {
                var RoleClaim = User.FindFirst("type");
                return RoleClaim?.Value;
            }
        }

        [HttpGet]
        public ActionResult<List<Jewelry>> GetAll()
        {
            if (UserRole == "Admin")
                return jewelryService.GetAll();
            return jewelryService.GetAll().Where(j => j.UserId == UserId).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Jewelry> Get(int id)
        {
            var jewelry = jewelryService.Get(id);
            if (jewelry == null)
                return NotFound();
            if (jewelry.UserId != UserId)
                return Unauthorized("User ID is missing or invalid.");
            return jewelry;
        }

        [HttpPost]
        public IActionResult Create(Jewelry jewelry)
        {
            jewelryService.Add(jewelry, UserId.GetValueOrDefault());
            return CreatedAtAction(nameof(Create), new { id = jewelry.Id }, jewelry);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, Jewelry jewelry)
        {
            if (id != jewelry.Id && UserRole != "Admin")
                return Unauthorized();
            var exitingJewelry = jewelryService.Get(id);
            if (exitingJewelry is null)
                return NotFound();
            if (UserId != exitingJewelry.UserId && UserRole != "Admin")
                return Unauthorized("User ID is missing or invalid.");
            jewelryService.Update(jewelry, UserId.GetValueOrDefault());
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var jewelry = jewelryService.Get(id);
            if (jewelry is null)
                return NotFound();
            if (jewelry.UserId != UserId && UserRole != "Admin")
                return Unauthorized("User ID is missing or invalid.");
            jewelryService.Delete(id);
            return Content(jewelryService.Count.ToString());
        }
    }
}