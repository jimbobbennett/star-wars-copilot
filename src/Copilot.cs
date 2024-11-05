using Microsoft.Extensions.AI;
using Pieces.Extensions.AI;

namespace StarWars.Copilot;

/// <summary>
/// An abstract base class for a copilot.
/// 
/// This implements <see cref="IChatClient"/> from Microsoft.Extensions.AI, and wraps a
/// <see cref="PiecesChatClient"/>  that also implements this same interface.
/// 
/// When this is constructed, the derived class can set the system prompt,
/// and set any default additional properties. These are used to manage the PiecesChatClient.
/// The keys and values for the supported additional properties are listed in the Pieces.OS.Client GitHub repo
/// https://github.com/pieces-app/pieces-os-client-sdk-for-csharp/blob/main/src/Extensions/README.md
/// </summary>
/// <param name="piecesChatClient">The chat client to wrap</param>
/// <param name="systemPrompt">The system prompt to use</param>
public abstract class Copilot(IChatClient piecesChatClient, string systemPrompt) : IChatClient
{
    private readonly IChatClient piecesChatClient = piecesChatClient;
    private readonly string systemPrompt = systemPrompt;

    /// <summary>
    /// Default additional properties - these are added to the options for every call to CompleteAsync
    /// and CompleteStreamingAsync.
    /// </summary>
    protected AdditionalPropertiesDictionary DefaultAdditionalProperties { get; set; } = [];

    /// <inheritdoc />
    public ChatClientMetadata Metadata => piecesChatClient.Metadata;

    /// <inheritdoc />
    public Task<ChatCompletion> CompleteAsync(IList<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Add the message guidance and default additional properties
        chatMessages = AddSystemPromptIfRequired(chatMessages);
        options = AddDefaultOptionsIfRequired(options);
        
        // Call the wrapped IChatClient
        return piecesChatClient.CompleteAsync(chatMessages, options, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(IList<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Add the message guidance and default additional properties
        chatMessages = AddSystemPromptIfRequired(chatMessages);
        options = AddDefaultOptionsIfRequired(options);

        // Call the wrapped IChatClient
        return piecesChatClient.CompleteStreamingAsync(chatMessages, options, cancellationToken);
    }

    /// <summary>
    /// Adds the given message as the first chat message as a system prompt if needed.
    /// </summary>
    /// <param name="chatMessages"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private IList<ChatMessage> AddSystemPromptIfRequired(IList<ChatMessage> chatMessages)
    {
        // Copy the chat messages so we can manipulate them, and remove the existing system prompt
        chatMessages = [.. chatMessages];

        if (chatMessages.Any() && 
            chatMessages.First().Role == ChatRole.System)
        {
            chatMessages.RemoveAt(0);
        }

        // ensure the last message is a user message
        if (chatMessages.Last()?.Role != ChatRole.User)
        {
            throw new ArgumentException(nameof(chatMessages));
        }

        // Insert the system prompt at the start
        chatMessages.Insert(0, new(ChatRole.System, systemPrompt));

        return chatMessages;
    }

    /// <summary>
    /// Adds any default options to the chat options.
    /// 
    /// This makes a copy of the chat options so the original is untouched. This copy 
    /// has the <see cref="DefaultAdditionalProperties"/> copied in.
    /// If there are no DefaultAdditionalProperties, this returns the original options.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private ChatOptions? AddDefaultOptionsIfRequired(ChatOptions? options)
    {
        // If we don't have any default additional properties, just return the original options
        if (DefaultAdditionalProperties == null || !DefaultAdditionalProperties.Any())
        {
            return options;
        }

        // Create a copy of the options. This is a deep copy
        options = options?.Clone() ?? new ChatOptions();

        // Initialize the additional properties collection if it is null
        options.AdditionalProperties ??= [];

        // Add all the default additional properties to the copy
        foreach (var kvp in DefaultAdditionalProperties)
        {
            options.AdditionalProperties[kvp.Key] = kvp.Value;
        }

        // Return the copy
        return options;
    }

    /// <inheritdoc />
    public void Dispose() => piecesChatClient.Dispose();

    /// <inheritdoc />
    public TService? GetService<TService>(object? key = null) where TService : class => piecesChatClient.GetService<TService>();
}
