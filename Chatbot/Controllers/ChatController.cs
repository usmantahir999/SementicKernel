using Chatbot.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(SemanticKernelService semanticKernelService, ChatService _chatService) : ControllerBase
    {
        private readonly SemanticKernelService _semanticKernelService = semanticKernelService;

        [HttpGet("{sessionId}")]
        public async Task<ActionResult> GetMessagesAsync(string sessionId)
        {
            var messages = await _chatService.GetMessagesAsync(sessionId);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<ActionResult> GetChatResponseAsync(ChatRequest request)
        {
            var sessionId = request.SessionId;
            var messages = await _chatService.GetMessagesAsync(sessionId);
            await _chatService.AddMessageAsync(sessionId, request.UserMessage, "User");
            //var result = await _semanticKernelService.GetChatResponseAsync(request.UserMessage, messages);
            var result = await _semanticKernelService.GetChatResponseWithRagAsync(request.UserMessage);
            await _chatService.AddMessageAsync(sessionId, result, "Bot");
            return Ok(result);
        }
    }

    public record ChatRequest(string UserMessage, string SessionId);
}
