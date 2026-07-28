using AIKnowledgeBot.InterFace.IRepository;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.Entities;
using AIKnowledgeBot.Services.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeBot.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConversationController(
            IConversationRepository conversationRepository,
            IUnitOfWork unitOfWork)
        {
            _conversationRepository = conversationRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var conversations = await _conversationRepository.GetAllAsync();
            return Ok(conversations);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var conversation = await _conversationRepository.GetByIdAsync(id);

            if (conversation == null)
                return NotFound();

            return Ok(conversation);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Conversation conversation)
        {
            conversation.Id = Guid.NewGuid();
            conversation.CreatedAt = DateTime.UtcNow;
            conversation.UpdatedAt = DateTime.UtcNow;

            await _conversationRepository.AddAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = conversation.Id },
                conversation);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Conversation conversation)
        {
            conversation.Id = Guid.NewGuid();
            conversation.CreatedAt = DateTime.UtcNow;
            conversation.UpdatedAt = DateTime.UtcNow;

            await _conversationRepository.UpdateAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = conversation.Id },
                conversation);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var conversation = await _conversationRepository.GetByIdAsync(id);

            if (conversation == null)
                return NotFound();

            _conversationRepository.DeleteAsync(conversation);

            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
