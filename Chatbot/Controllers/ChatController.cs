using Chatbot.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(SemanticKernelService semanticKernelService) : ControllerBase
    {
        private readonly SemanticKernelService _semanticKernelService = semanticKernelService;

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return Ok("Welcome to the Chatbot API. Use the /GetChatResponseAsync endpoint to interact with the chatbot.");
        }

        [HttpPost]
        public async Task<ActionResult> GetChatResponseAsync(ChatRequest request)
        {
           var result = await _semanticKernelService.GetChatResponseAsync(request.UserMessage);
            return Ok(result);
        }
    }

    public record ChatRequest(string UserMessage);
}
