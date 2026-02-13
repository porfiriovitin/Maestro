using Google.GenAI;
using Maestro.Gemini.Models;

namespace Maestro.Gemini;

public sealed class GeminiClient
{
    private readonly Client _client;

    public GeminiClient(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key cannot be null or empty.", nameof(apiKey));

        _client = new Client(apiKey: apiKey);
    }

    /// <summary>
    /// Creates a new instance of a Gemini agent using the specified configuration parameters.
    /// </summary>
    /// <param name="parameters">The configuration parameters used to initialize the Gemini agent. Cannot be null.</param>
    /// <returns>A new instance of <see cref="GeminiAgent"/> initialized with the provided parameters.</returns>
    public GeminiAgent CreateAgent(CreateGeminiAgentRequest parameters)
        => new(_client, parameters);
}
