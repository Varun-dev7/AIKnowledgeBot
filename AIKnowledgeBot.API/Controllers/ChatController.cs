using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.DTOs;
using AIKnowledgeBot.Services.SQL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AIKnowledgeBot.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask(ChatRequestDto request)
        {
            var response = await _chatService.AskAsync(request);

            return Ok(response);
        }
    }
}
