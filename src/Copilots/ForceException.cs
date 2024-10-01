namespace StarWars.Copilot.Copilots;

public class ForceException : Exception
{
    public ForceException()
    {
    }

    public ForceException(string? message) : base(message)
    {
    }

    public ForceException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}