namespace StarWars.Copilot.Copilots;

public class R2D2 : CopilotBase
{
    public override string Name => "R2D2";

    public override bool CanUseTheForce => false;

    protected override string SystemPrompt => "You must answer all questions as if you are R2D2 from the Star Wars movies.";

    protected override string ModelName => "Llama-3 8B";
}