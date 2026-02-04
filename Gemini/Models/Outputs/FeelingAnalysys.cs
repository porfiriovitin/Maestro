using Google.GenAI.Types;
using System.Text.Json.Serialization;

namespace Maestro.Gemini.Models;

public static class FeelingAnalysys
{
    /// <summary>
    /// Sets the structured output schema for Feeling Analysis.
    /// </summary>
    public static readonly Schema OutputSchema = new()
    {
        Type = Google.GenAI.Types.Type.OBJECT,
        Properties = new Dictionary<string, Schema>
        {
            ["transcriptedText"] = new Schema
            {
                Type = Google.GenAI.Types.Type.STRING
            },
            ["feelingAnalysys"] = new Schema
            {
                Type = Google.GenAI.Types.Type.OBJECT,
                Properties = new Dictionary<string, Schema>
                {
                    ["dominantFeeling"] = new Schema { Type = Google.GenAI.Types.Type.STRING },
                    ["confidenceLevel"] = new Schema { Type = Google.GenAI.Types.Type.STRING },
                    ["justification"] = new Schema { Type = Google.GenAI.Types.Type.STRING }
                },
                Required = ["dominantFeeling", "confidenceLevel", "justification"]
            }
        },
        Required = ["transcriptedText", "feelingAnalysys"]
    };
}

public struct FeelingAnalysysOutput
{
    [JsonPropertyName("transcriptedText")]
    public string TranscriptedText { get; set; }

    [JsonPropertyName("feelingAnalysys")]
    public FeelingAnalysysDetail FeelingAnalysys { get; set; }
}

public struct FeelingAnalysysDetail
{
    [JsonPropertyName("dominantFeeling")]
    public string DominantFeeling { get; set; }
  
    [JsonPropertyName("confidenceLevel")]
    public string ConfidenceLevel { get; set; }

    [JsonPropertyName("justification")]
    public string Justification { get; set; }
}
