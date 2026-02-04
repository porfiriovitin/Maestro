namespace Maestro.Gemini.Interfaces;

public interface IAgent
{
    string Name { get; }
    string Role { get; }
    string Description { get; }
    Task<string> ExecuteAsync(string inputContext);
}
