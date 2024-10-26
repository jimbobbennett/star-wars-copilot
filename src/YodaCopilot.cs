using Microsoft.Extensions.AI;
using Pieces.Extensions.AI;

namespace StarWars.Copilot;

/// <summary>
/// A copilot based off Yoda
/// </summary>
public class YodaCopilot: Copilot
{
    /// <summary>
    /// Creates a new Yoda inspired copilot. This has message guidance around being like Yoda, and sets up
    /// Pieces Long-Term Memory in the additional properties so Yoda can sense your feelings.
    /// </summary>
    /// <param name="piecesChatClient"></param>
    public YodaCopilot(IChatClient piecesChatClient)
     : base(piecesChatClient, "You are a helpful copilot who will try to answer all questions. Reply in the style of Yoda, including using Yoda's odd sentence structure. Refer to anger being a path to the dark side often, and when referencing context from the user workflow refer to communing with the living force.")
    {
        // Turn on Pieces Long-Term Memory for the last 3 hours in the default additional properties
        DefaultAdditionalProperties.Add(PiecesChatClient.LongTermMemoryPropertyName, true);
        DefaultAdditionalProperties.Add(PiecesChatClient.LongTermMemoryTimeSpanPropertyName, TimeSpan.FromHours(3));
    }
}