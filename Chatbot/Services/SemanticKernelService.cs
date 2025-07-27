using Microsoft.SemanticKernel;

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

        public async Task<string> GetChatResponseAsync(string prompt)
        {
            var response = await _kernel.InvokePromptAsync(prompt);
            Console.WriteLine($"Response: {response}");
            return response.ToString();
        }
    }
}
