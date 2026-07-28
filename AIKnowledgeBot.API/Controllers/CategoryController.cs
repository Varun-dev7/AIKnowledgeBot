using AIKnowledgeBot.InterFace.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeBot.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryService;

        public CategoryController(ICategoryRepository categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
    }
}
