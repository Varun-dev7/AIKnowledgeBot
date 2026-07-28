using AIKnowledgeBot.InterFace.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeBot.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectRepository _subjectService;
        public SubjectController(ISubjectRepository subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var subjects = await _subjectService.GetAllAsync();
            return Ok(subjects);
        }
    }
}
