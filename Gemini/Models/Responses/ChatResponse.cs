namespace Maestro.Gemini.Models;

public struct ChatResponse
{
    /// <summary>
    /// Main Ai response content.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Amount of tokens used in the input.
    /// </summary>
    public int InputTokens { get; set; }

    /// <summary>
    /// Amount of tokens used in the output.
    /// </summary>
    public int OutputTokens { get; set; } 

    /// <summary>
    /// Amount of tokens used.
    /// </summary>
    public int TotalTokens { get; set; }
}
