using Microsoft.Extensions.AI;
using Pieces.Extensions.AI;

namespace StarWars.Copilot;

/// <summary>
/// A copilot based off R2-D2
/// </summary>
public class R2D2Copilot : Copilot
{
    /// <summary>
    /// Creates a new R2-D2 inspired copilot. This has message guidance around being like R2, and sets up the
    /// Death Star plans in the additional properties.
    /// </summary>
    /// <param name="piecesChatClient"></param>
    public R2D2Copilot(IChatClient piecesChatClient)
     : base(piecesChatClient, "You are a helpful copilot who will try to answer all questions. Reply in the style of R2-D2. Be slightly sarcastic in your responses, and start and end everything with beeps.")
    {
        // Add the death star plans from the GalacticEmpire folder in this repo
        var deathStarPlans = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..", "GalacticEmpire"));
        DefaultAdditionalProperties.Add(PiecesChatClient.FoldersPropertyName, new List<string>{ deathStarPlans });
    }
}
