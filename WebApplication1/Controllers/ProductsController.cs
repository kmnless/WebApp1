using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : Controller
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return Ok(await _context.Product.ToListAsync());
        }
        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return Created("created", product);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Product>> UpdatePrice(int id, [FromBody] int price)
        {
            var product = await _context.Product.FindAsync(id);
            product.Price = price;
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpGet("sorted")]
        public async Task<ActionResult<IEnumerable<Product>>> GetSortedProducts(bool isDesc, string sortBy)
        {
            IQueryable<Product> products = _context.Product;

            if (!isDesc)
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        products = products.OrderBy(p => p.Name);
                        break;
                    case "price":
                        products = products.OrderBy(p => p.Price);
                        break;
                    default:
                        return BadRequest("Invalid sort by parameter");
                }
            }
            else if (isDesc)
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        products = products.OrderByDescending(p => p.Name);
                        break;
                    case "price":
                        products = products.OrderByDescending(p => p.Price);
                        break;
                    default:
                        return BadRequest("Invalid sort by parameter");
                }
            }

            return Ok(await products.ToListAsync());
        }

        [HttpDelete("delete-many")]
        public async Task<ActionResult> DeleteProducts([FromBody] List<int> ids)
        {
            var products = await _context.Product.Where(p => ids.Contains(p.Id)).ToListAsync();

            _context.Product.RemoveRange(products);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("by-description")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByDescription(string description)
        {
            IQueryable<Product> products = _context.Product;

            products = products.Where(p => p.Description == description);

            return Ok(await products.ToListAsync());
        }
    }
}