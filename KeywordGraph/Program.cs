// set up the client

using KeywordGraph;

var words = new[] { "cat", "mouse", "lion", "tiger", "helicopter", "train", "blue", "carrot", "space" };

await Helper.GenerateEmbeddings(words);

#region Attempt 1
// using Microsoft.Extensions.AI;
// using OllamaSharp;
// using OllamaSharp.Models.Chat;

// var uri = new Uri("http://localhost:11434");
// var ollama = new OllamaApiClient(uri);
// ollama.SelectedModel = "qwen3-embedding";
// var messages = new List<Message>();

// messages.Add(new Message()
// {
//     Role = OllamaSharp.Models.Chat.ChatRole.System,
//     Content = "You are a helpful assistant"
// });


// messages.Add(new Message()
// {
//     Role = OllamaSharp.Models.Chat.ChatRole.User,
//     Content = "wht is 1 +1?"
// });


// var request = new OllamaSharp.Models.Chat.ChatRequest()
// {
//     Model = "qwen3:4b",
//     Messages = messages
// };
// await foreach (var response in ollama.ChatAsync(request))
// {
//     System.Console.WriteLine(response);
// }
// Console.ReadKey();
#endregion

#region Attempt 2
// // Create Ollama client for the "llama 3.2" model
// using Microsoft.Extensions.AI;
// using OllamaSharp;

// IChatClient chatClient = new OllamaApiClient(
//     new Uri("http://localhost:11434/"),
//     "qwen3:4b");

// // Keep track of chat history
// List<ChatMessage> chatHistory = new();

// Console.WriteLine("Type 'exit' to quit");
// Console.WriteLine();

// while (true)
// {
//     Console.Write("You: ");
//     var userInput = Console.ReadLine();

//     if (string.IsNullOrWhiteSpace(userInput))
//     {
//         continue;
//     }

//     if (string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase))
//     {
//         break;
//     }

//     // Add user message
//     chatHistory.Add(new ChatMessage(Microsoft.Extensions.AI.ChatRole.User, userInput));

//     Console.Write("Assistant: ");
//     var assistantResponse = "";

//     // Stream the AI response in real time
//     await foreach (var update in chatClient.GetStreamingResponseAsync(chatHistory))
//     {
//         Console.Write(update.Text);
//         assistantResponse += update.Text;
//     }

//     chatHistory.Add(new ChatMessage(Microsoft.Extensions.AI.ChatRole.Assistant, assistantResponse));
//     Console.WriteLine("\n");
// }


// Console.ReadKey();

#endregion