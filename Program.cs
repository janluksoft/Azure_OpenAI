//--- Program.cs ----
#nullable disable

using System.Text.Json;
using OpenAI.Chat;
using static System.Net.Mime.MediaTypeNames;
using static System.Environment;
using Azure;
using OpenAI;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;


namespace MyAzure.OpenAI;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine($"Start the OpenAI service\n\r");

        string questText =
            " 1) How much is 7+ 5*5 ?\n\r" +
            " 2) Jaka jest stolica Włoch?\n\r" +
            " 3) Ist BMW ein deutsches Auto?\n\r";

        var cOpenAIUse = new OpenAIUse();
        await cOpenAIUse.BasicChat(questText);

        // Output: Assistant:
        //  1) The sum of 7 and 5 multiplied by 5 is 32.
        //  2) Stolica Włoch to Rzym.
        //  3) Ja, BMW ist ein deutsches Auto.
    }
}

/// <summary>
/// OpenAI on Azure Usage Class
/// </summary>
public class OpenAIUse
{
    //class CAzureOpenAIUtils contains sensitive connection strings to the OpenAI service on Azure
    private readonly CAzureOpenAIUtils _openAIUtils;

    public OpenAIUse()
    {
        _openAIUtils = new CAzureOpenAIUtils();
    }

    public async Task BasicChat(string userQuestion)
    {
        string systemText = "You are a helpful assistant. Answer in languages ​​like questions.";

        // Prepare messages
        var messList = new List<ChatMessage>();
        messList.Add(new SystemChatMessage(systemText));
        messList.Add(new UserChatMessage(userQuestion));

        var options = new ChatCompletionOptions { MaxOutputTokenCount = 800 };

        try
        {
            // Create a chat completion request
            ChatCompletion completion = await _openAIUtils.AskHistoryQuestionComp(messList, options);

            Console.WriteLine($"Question: \n\r{userQuestion}");

            // Print answer
            if (completion != null)
            {
                Console.WriteLine($"\n\rThe answer AI:");
                Console.WriteLine($"{completion.Role}: \n\r{completion.Content[0].Text}");
                Console.WriteLine($"\n\rThe answer params:");
                Console.WriteLine(JsonSerializer.Serialize(completion, new JsonSerializerOptions()
                { WriteIndented = true }));
                //ChatCompletion completion = await this.chatClient.CompleteChatAsync(myQuest); //  CompleteChat(myQuest);
                //Console.WriteLine($"{completion.Role}: {completion.Content[0].Text}");
            }
            else
            {
                Console.WriteLine("No response received.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
