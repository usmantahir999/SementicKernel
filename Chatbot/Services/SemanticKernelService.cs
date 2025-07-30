using Chatbot.Data;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace Chatbot.Services
{
    public class SemanticKernelService
    {
        private readonly Kernel _kernel;
        public SemanticKernelService(IConfiguration configuration)
        {
            string apiKey = configuration["SementicKernel:ApiKey"];
            string endpoint = configuration["SementicKernel:Endpoint"];
            string deploymentName = configuration["SementicKernel:DeploymentName"];
            var builder = Kernel.CreateBuilder();

            builder.AddAzureOpenAIChatCompletion(
           deploymentName: deploymentName,
           endpoint: endpoint,
           apiKey: apiKey);

            _kernel = builder.Build();
        }

        public async Task<string> GetChatResponseAsync(string userMessage, IEnumerable<ChatMessage> messages)
        {
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            ChatHistory chatHistory = [];
            foreach (var message in messages)
            {
                if (message.Sender == "User")
                {
                    chatHistory.AddUserMessage(message.Message);
                }
                else
                {
                    chatHistory.AddAssistantMessage(message.Message);
                }
            }
            chatHistory.AddUserMessage(userMessage);
            var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
            chatHistory: chatHistory,
            kernel: _kernel
                );

            var response = new StringBuilder();
            await foreach (var chunk in result)
            {
                response.Append(chunk);
            }
            return response.ToString();
        }
    }
}
