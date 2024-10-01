using Pieces.OS.Client;
using StarWars.Copilot.Copilots;

// Create the Pieces client
var client = new PiecesClient();

var r2 = new R2D2();
await r2.InitAsync(client).ConfigureAwait(false);

var obiWan = new ObiWan();
await obiWan.InitAsync(client).ConfigureAwait(false);

await foreach (var token in r2.AskQuestionAsync("Who are you?"))
{
    Console.Write(token);
}

Console.WriteLine();

await foreach (var token in obiWan.AskQuestionAsync("Who are you?"))
{
    Console.Write(token);
}

Console.WriteLine();

await foreach (var token in obiWan.AskQuestionAsync("How can I create a hello world application in C#"))
{
    Console.Write(token);
}

Console.WriteLine();