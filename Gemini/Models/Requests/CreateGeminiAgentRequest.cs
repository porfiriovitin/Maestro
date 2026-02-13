using Google.GenAI.Types;

namespace Maestro.Gemini.Models;

public struct CreateGeminiAgentRequest
{
    public GeminiModel Model { get; set; }
    public string SystemPrompt { get; set; }
    public string UserPrompt { get; set; }
    public ThinkingCapacity ThinkingCapacity { get; set; }
    public float Temperature { get; set; }
}

public enum ThinkingCapacity
{
    MINIMAL,
    LOW,
    MEDIUM,
    HIGH
}

public static class ThinkingLevelExtensions
{
    public static ThinkingLevel ToThinkingLevel(this ThinkingCapacity t)
    {
        return t switch
        {
            ThinkingCapacity.MINIMAL => ThinkingLevel.MINIMAL,
            ThinkingCapacity.LOW => ThinkingLevel.LOW,
            ThinkingCapacity.MEDIUM => ThinkingLevel.MEDIUM,
            ThinkingCapacity.HIGH => ThinkingLevel.HIGH,
            _ => throw new ArgumentOutOfRangeException(nameof(t), t, null),
        };
    }
}

