namespace Maestro.Gemini.Models;

public struct ChatResponse
{
    public string Content { get; set; }
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; } 
    public int TotalTokens { get; set; }
}
