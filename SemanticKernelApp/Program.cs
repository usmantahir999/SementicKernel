using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernelApp.Config;
using SemanticKernelApp.Plugins.CareerHistoryPlugin;

class Program
{
    private static readonly Kernel _kernel;
    private static readonly KernelPlugin _plugins;

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
        builder.Plugins.AddFromType<ConversationSummaryPlugin>();
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        _kernel = builder.Build();
        _kernel.ImportPluginFromType<CareerTrackerPlugin>();
        _plugins = _kernel.CreatePluginFromPromptDirectory("Plugins");
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
         //await TestAzureOpenAIChatCompletion();
         //await TestTimePlugin();
         //await TestConversationSummaryPlugin();
         //await TestCareerAdvisory();
         //await TestCareerCoachPlugin();
         await TestCareerTrackerPlugin();
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

    private static async Task TestConversationSummaryPlugin()
    {
        Console.WriteLine("Enter converation of your inquiry!");
        var prompt = Console.ReadLine();
        Console.WriteLine("******* Action Items ************");
        var actionItemResult = await _kernel.InvokeAsync("ConversationSummaryPlugin", "GetConversationActionItems",
            arguments: new(){ { "input", prompt } });
        Console.WriteLine(actionItemResult);

        Console.WriteLine("******* Topics ************");
        var topicResult = await _kernel.InvokeAsync("ConversationSummaryPlugin", "GetConversationTopics",
            arguments: new() { { "input", prompt } });
        Console.WriteLine(topicResult);

        Console.WriteLine("******* Summary ************");

        var summaryResult = await _kernel.InvokeAsync("ConversationSummaryPlugin", "SummarizeConversation",
            arguments: new() { { "input", prompt } });
        Console.WriteLine(summaryResult);

    }

    private static async Task TestCareerAdvisory()
    {
        Console.WriteLine("Enter your Career History!");
        var carrerHistory = Console.ReadLine();
        string prompt = @"This is the information about user background: {{$carrerHistory}}

         Given the user background, provide a list of relevant carrer choices";

        var result = await _kernel.InvokePromptAsync(prompt, new()
        {
            { "carrerHistory", carrerHistory }
        });
        Console.WriteLine(result);
    }

    private static async Task TestCareerCoachPlugin()
    {
        Console.WriteLine("Enter your Career career or area of expertise!");
        string careerFocus = Console.ReadLine();
        var result = await _kernel.InvokeAsync(_plugins["CareerCoach"], new()
        {
            {"carrerFocus",careerFocus }
        });
        Console.WriteLine(result);
        Console.ReadKey();
    }

    private static async Task TestCareerTrackerPlugin()
    {
        Console.WriteLine("Enter job title");
        var jobTitle = Console.ReadLine();
        Console.WriteLine("Enter Company Name");
        var companyName = Console.ReadLine();
        var validRanks = new HashSet<string> { "Employee", "Supervisor", "Manager" };
        string rank = string.Empty;
        while (!validRanks.Contains(rank))
        {
            Console.WriteLine("Enter Rank (Employee, Supervisor, Manager):");
            rank = Console.ReadLine();
           
        }
        var result = await _kernel.InvokeAsync("CareerTrackerPlugin", "AddToCareerHistoryList", new()
        {
            { "title", jobTitle },
            { "company", companyName },
            { "rank", rank }
        });
        Console.WriteLine(result);
        Console.WriteLine("Fetching updated career history...");
        var careerHistory = await _kernel.InvokeAsync("CareerTrackerPlugin", "GetCareerHistory");
        Console.WriteLine(careerHistory);
        Console.ReadKey();
    }

    private static async Task<string> GetChatCompletionAsync(string prompt)
    {
        var result = await _kernel.InvokePromptAsync(prompt);
        return result.ToString();
    }
}
