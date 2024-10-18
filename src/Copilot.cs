using Microsoft.Extensions.AI;
using Pieces.Extensions.AI;

namespace StarWars.Copilot;

public abstract class Copilot(IChatClient piecesChatClient, string messageGuidance) : IChatClient
{
    private readonly IChatClient piecesChatClient = piecesChatClient;
    private readonly string messageGuidance = messageGuidance;
    protected AdditionalPropertiesDictionary DefaultAdditionalProperties { get; set; } = [];

    public ChatClientMetadata Metadata => piecesChatClient.Metadata;

    public Task<ChatCompletion> CompleteAsync(IList<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        chatMessages = AddSystemPromptIfRequired(chatMessages);
        options = AddDefaultOptionsIfRequired(options);
        
        return piecesChatClient.CompleteAsync(chatMessages, options, cancellationToken);
    }

    public IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(IList<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        chatMessages = AddSystemPromptIfRequired(chatMessages);
        options = AddDefaultOptionsIfRequired(options);

        return piecesChatClient.CompleteStreamingAsync(chatMessages, options, cancellationToken);
    }

    private IList<ChatMessage> AddSystemPromptIfRequired(IList<ChatMessage> chatMessages)
    {
        // Copy the chat messages so we can manipulate them, and remove the existing system prompt
        chatMessages = [.. chatMessages];

        // ensure the last message is a user message
        if (chatMessages.Last()?.Role != ChatRole.User)
        {
            throw new ArgumentException(nameof(chatMessages));
        }

        // Adjust the last message to contain the relevant guidance for the prompt
        var last = chatMessages.Last();
        chatMessages.Remove(last);
        chatMessages.Add(new(ChatRole.User, last.Text + $". {messageGuidance}"));

        return chatMessages;
    }

    private ChatOptions? AddDefaultOptionsIfRequired(ChatOptions? options)
    {
        // Copy the options
        options = options?.Clone() ?? new ChatOptions();
        options.AdditionalProperties ??= [];

        foreach (var kvp in DefaultAdditionalProperties)
        {
            options.AdditionalProperties[kvp.Key] = kvp.Value;
        }

        return options;
    }

    public void Dispose() => piecesChatClient.Dispose();

    public TService? GetService<TService>(object? key = null) where TService : class => piecesChatClient.GetService<TService>();
}

public class R2D2Copilot : Copilot
{
    public R2D2Copilot(IChatClient piecesChatClient)
     : base(piecesChatClient, "Reply in the style of R2-D2. Be slightly sarcastic in your responses, and start and end everything with beeps.")
    {
        var deathStarPlans = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..", "GalacticEmpire"));
        DefaultAdditionalProperties.Add(PiecesChatClient.FoldersPropertyName, new List<string>{ deathStarPlans });
    }
}

public class YodaCopilot: Copilot
{
    public YodaCopilot(IChatClient piecesChatClient)
     : base(piecesChatClient, "Reply in the style of Yoda, including using Yoda's odd sentence structure. Refer to anger being a path to the dark side often, and when referencing context from the user workflow refer to communing with the living force.")
    {
        DefaultAdditionalProperties.Add(PiecesChatClient.LiveContextPropertyName, true);
        DefaultAdditionalProperties.Add(PiecesChatClient.LiveContextTimeSpanPropertyName, TimeSpan.FromHours(3));
    }
}