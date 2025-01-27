using Microsoft.Extensions.AI;
using Pieces.Extensions.AI;
using Pieces.OS.Client;
using Spectre.Console;
using StarWars.Copilot;

// Set up a Pieces client using the default settings
// For this to work, you will need to have Pieces installed and running.
// You can install it from https://pieces.app
var piecesClient = new PiecesClient();

// Load the models, downloading the local one if needed
// We have a cloud model and local model. The cloud model is used by default, but you can swap to a local one for
// a fully offline experience.
// To see a list of models, use the piecesClient.GetModelsAsync method
var cloudModel = await piecesClient.GetModelByNameAsync("Gemini-2.0 Flash").ConfigureAwait(false);
var localModel = await piecesClient.GetModelByNameAsync("Llama-3 8B").ConfigureAwait(false);
await piecesClient.DownloadModelAsync(localModel).ConfigureAwait(false);

// Hello there! Show the star wars logo
var font = FigletFont.Load("starwars.flf");
AnsiConsole.Write(new FigletText(font, "Hello there!").Centered().Color(Color.Yellow));

// Get the users preferred copilot
var copilot = AnsiConsole.Prompt(new SelectionPrompt<string>()
                         .Title("Who do you want as your copilot?")
                         .AddChoices(["R2-D2", "Yoda",]));

const string R2D2ChatName = "Hey there, R2";
const string YodaChatName = "Looking? Found someone, you have, I would say, hmmmm?";

// Create the copilot depending on the selection from the prompt
// If you want to use the local model, you can change it here
IChatClient client = copilot switch {
    "R2-D2" => new R2D2Copilot(new PiecesChatClient(piecesClient, R2D2ChatName, model: cloudModel)),
    "Yoda" => new YodaCopilot(new PiecesChatClient(piecesClient, YodaChatName, model: cloudModel)),
    _ => throw new NotImplementedException()
};

// A help function to ask a question and stream the result.
// When streaming a result, if the FinishReason is Stop, then the response is finished, and the 
// Text property contains th entire chat response.
// If the FinishReason is not stop, then the Text property just contains the last token.
static async Task AskQuestionAndStreamAnswer(IChatClient client, List<ChatMessage> chatMessages)
{
    // Get each response from the streaming completion
    await foreach (var r in client.CompleteStreamingAsync(chatMessages))
    {
        // Make sure we have text
        if (r.Text is not null)
        {
            // A finish reason of stop means we have finished streaming the response
            // and the Text property contains the full response
            if (r.FinishReason == ChatFinishReason.Stop)
            {
                // Add the full response to the messages as an assistant message
                chatMessages.Add(new(ChatRole.Assistant, r.Text));
            }
            else
            {
                // If we are not finished, write the next token to the console
                Console.Write(r.Text);
            }
        }
    }

    // Once we have streamed everything, add a new line to the console
    Console.WriteLine("");
}

// Output a ruled line
var rule = new Rule();
rule.RuleStyle("yellow");
AnsiConsole.Write(rule);

// Start building up the chat messages with a hello. Stream the response to this
// as a way to introduce the copilot to the user
var chatMessages = new List<ChatMessage>{ new(ChatRole.User, "Hello, who are you?") };
await AskQuestionAndStreamAnswer(client, chatMessages);

// Get the users question
var question = AnsiConsole.Prompt(new TextPrompt<string>(":"));

// Loop till the user types goodbye
while (!question.Equals("goodbye", StringComparison.OrdinalIgnoreCase))
{
    // Add the question as a user message to the chat messages
    chatMessages.Add(new(ChatRole.User, question));

    // Send all the chat messages to the client - this includes the previous questions
    // and answers
    await AskQuestionAndStreamAnswer(client, chatMessages);

    // Get the next question from the user
    question = AnsiConsole.Prompt(new TextPrompt<string>(":"));
}
