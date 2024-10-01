using Pieces.OS.Client;
using Pieces.OS.Client.Copilot;

namespace StarWars.Copilot.Copilots;

public abstract class CopilotBase
{
    protected abstract string SystemPrompt { get; }
    protected abstract string ModelName { get;}

    public abstract string Name { get; }
    public abstract bool CanUseTheForce { get; }

    private ICopilotChat? copilotChat;

    public async Task InitAsync(IPiecesClient client)
    {
        var copilot = await client.GetCopilotAsync().ConfigureAwait(false);
        var models = await client.GetModelsAsync().ConfigureAwait(false);
        var model = models.First(m => m.Name.Contains(ModelName, StringComparison.InvariantCultureIgnoreCase));
        if (!model.Cloud && !model.Downloaded)
        {
            await client.DownloadModelAsync(model).ConfigureAwait(false);
        }

        copilotChat = await copilot.CreateChatAsync($"Conversation with {Name}", model: model, useLiveContext: CanUseTheForce).ConfigureAwait(false);
        await copilotChat.AskQuestionAsync(SystemPrompt).ConfigureAwait(false);
    }

    public IAsyncEnumerable<string> AskQuestionAsync(string question)
    {
        if (copilotChat is null)
        {
            throw new ForceException("This copilot has not been initialized yet");
        }

        return copilotChat.AskStreamingQuestionAsync(question, cancellationToken: default);
    }
}