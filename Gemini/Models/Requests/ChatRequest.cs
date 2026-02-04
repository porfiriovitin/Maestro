using Google.GenAI.Types;

namespace Maestro.Gemini.Models;

public struct ChatRequest
{
    public GeminiModel Model { get; set; }
    public string SystemPrompt { get; set; }
    public string UserPrompt { get; set; }
    public List<ChatMessage>? History { get; set; }
    public float? Temperature { get; set; }
    public ThinkingLevel? ThinkingLevel { get; set; }
    public Schema? ResponseSchema { get; set; }

}

public class ChatMessage
{
    public required string Role { get; set; } 
    public required string Message { get; set; }
}
