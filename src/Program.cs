using Microsoft.Extensions.AI;
using Pieces.Extensions.AI;
using Pieces.OS.Client;
using Spectre.Console;
using StarWars.Copilot;

// Set up a Pieces client
var piecesClient = new PiecesClient();
var models = await piecesClient.GetModelsAsync();

var cloudModel = models.FirstOrDefault(m => m.Name.Contains("Gemini-1.5 Pro Chat", StringComparison.OrdinalIgnoreCase));
var localModel = models.FirstOrDefault(m => m.Name.Contains("Llama-3 8B", StringComparison.OrdinalIgnoreCase));

// Hello there!
var font = FigletFont.Load("starwars.flf");
AnsiConsole.Write(
    new FigletText(font, "Hello there!")
        .Centered()
        .Color(Color.Yellow));

// Get the users preferred copilot
var copilot = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Who do you want as your copilot?")
        .AddChoices([
            "R2-D2", "Yoda",
        ]));

const string R2D2ChatName = "Hey there, R2";
const string YodaChatName = "Looking? Found someone, you have, I would say, hmmmm?";

// Create the copilot
IChatClient client = copilot switch {
    "R2-D2" => new R2D2Copilot(new PiecesChatClient(piecesClient, R2D2ChatName, model: cloudModel)),
    "Yoda" => new YodaCopilot(new PiecesChatClient(piecesClient, YodaChatName, model: cloudModel)),
    _ => throw new NotImplementedException()
};

static async Task AskQuestionAndStreamAnswer(IChatClient client, List<ChatMessage> chatMessages)
{
    await foreach (var r in client.CompleteStreamingAsync(chatMessages))
    {
        if (r.Text is not null)
        {
            if (r.FinishReason == ChatFinishReason.Stop)
            {
                // If we are finished, add the response to the chat messages
                chatMessages.Add(new(ChatRole.Assistant, r.Text));
            }
            else
            {
                Console.Write(r.Text);
            }
        }
    }

    // End the line
    Console.WriteLine("");
}

// Output a ruled line
var rule = new Rule();
rule.RuleStyle("yellow");
AnsiConsole.Write(rule);

// Start building up the chat messages with a hello
var chatMessages = new List<ChatMessage>{ new(ChatRole.User, "Hello, who are you?") };
await AskQuestionAndStreamAnswer(client, chatMessages);

var question = AnsiConsole.Prompt(new TextPrompt<string>(":"));

while (!question.Equals("goodbye", StringComparison.OrdinalIgnoreCase))
{
    // Process the question
    chatMessages.Add(new(ChatRole.User, question));
    await AskQuestionAndStreamAnswer(client, chatMessages);

    // Prepare for the next question
    question = AnsiConsole.Prompt(new TextPrompt<string>(":"));
}
