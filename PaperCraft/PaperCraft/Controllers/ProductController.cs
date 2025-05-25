using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaperCraft.Data;
using PaperCraft.Models;

namespace PaperCraft.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) => _db = db;

        // GET: /api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _db.Products
                .OrderBy(p => p.Id)
                .ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
    }
}
