using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SemanticKernelApp.Plugins.CareerHistoryPlugin
{
    public class CareerTrackerPlugin
    {
        [KernelFunction, Description("Get a list of jobs worked during the user's career")]
        public static string GetCareerHistory()
        {
            string dir = Directory.GetCurrentDirectory();
            string content = File.ReadAllText($"{dir}/Data/recentjobs.txt");
            return content;
        }

        [KernelFunction, Description("Get a list of jobs worked during the user's career")]
        public static string AddToCareerHistoryList(
            [Description("The job title of a user")]string title,
            [Description("The company")]string company,
            [Description("The rank of the job title")]string rank)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}/Data/recentjobs.txt";
            string jsonContent = File.ReadAllText(filePath);
            var careerHistory = (JsonArray)JsonNode.Parse(jsonContent)!;
            var newJob = new JsonObject
            {
                ["title"] = title,
                ["company"] = company,
                ["rank"] = rank
            };  
            careerHistory.Insert(0, newJob);
            File.WriteAllText(filePath, JsonSerializer.Serialize(careerHistory,new JsonSerializerOptions
            {
                WriteIndented = true
            }));
            return $"Added '{title}' to career history";
        }
    }
}
