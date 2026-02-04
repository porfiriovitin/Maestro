using Google.GenAI;

namespace Maestro.Gemini;

/// <summary>
/// Sets the Gemini API key for the Client for dependency injection.
/// </summary>
public class GeminiMaestro
{
    public Client Client { get; }

    public GeminiMaestro(string apiKey)
    {
        Client = new Client(apiKey: apiKey);
    }
}