// set up the client

// using KeywordGraph;

// var words = new[] { "cat", "mouse", "lion", "tiger", "helicopter", "train", "blue", "carrot", "space" };

// await Helper.GenerateEmbeddings(words);

#region Attempt 1
using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models.Chat;

var uri = new Uri("http://localhost:11434");
var ollama = new OllamaApiClient(uri);
ollama.SelectedModel = "qwen3-embedding";
var messages = new List<Message>();

messages.Add(new Message()
{
    Role = OllamaSharp.Models.Chat.ChatRole.System,
    Content = "You are a helpful assistant"
});


messages.Add(new Message()
{
    Role = OllamaSharp.Models.Chat.ChatRole.User,
    Content = "wht is 1 +1?"
});


var request = new OllamaSharp.Models.Chat.ChatRequest()
{
    Model = "qwen3:4b",
    Messages = messages
};
await foreach (var response in ollama.ChatAsync(request))
{
    System.Console.WriteLine(response);
}
Console.ReadKey();
#endregion

Console.ReadKey();