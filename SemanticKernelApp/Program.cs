using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernelApp.Config;

class Program
{
    private static readonly Kernel _kernel;

    static Program()
    {
        var config = LoadConfiguration();
        var azureOpenAIConfig = config.GetSection("AzureOpenAI").Get<AzureOpenAIConfig>();
        var builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
           deploymentName: azureOpenAIConfig!.DeploymentName,
           endpoint: azureOpenAIConfig.Endpoint,
           apiKey: azureOpenAIConfig.ApiKey);


#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        builder.Plugins.AddFromType<TimePlugin>();
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        _kernel = builder.Build();
    }

    private static IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Needed for console apps
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
    static async Task Main(string[] args)
    {
        await TestAzureOpenAIChatCompletion();
        await TestTimePlugin();
    }

    //chat completion app
    private static async Task TestAzureOpenAIChatCompletion()
    {
        Console.WriteLine("Enter your inquiry!");
        var prompt = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(prompt))
        {
            var response = await GetChatCompletionAsync(prompt);
            Console.WriteLine(response);
        }

        Console.ReadKey();
    }

    //built-in Time plugin
    private static async Task TestTimePlugin()
    {
       var currentDay = await _kernel.InvokeAsync("TimePlugin", "DayOfWeek");
       var timeZone = await _kernel.InvokeAsync("TimePlugin", "TimeZoneName");
       Console.WriteLine($"Current Day: {currentDay}");
       Console.WriteLine($"Time Zone: {timeZone}");
       Console.ReadKey();
    }

    private static async Task<string> GetChatCompletionAsync(string prompt)
    {
        var result = await _kernel.InvokePromptAsync(prompt);
        return result.ToString();
    }
}
