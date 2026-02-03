using Google.GenAI;
using Google.GenAI.Types;
using Maestro.Gemini.Models;

namespace Maestro.Gemini;

public class ChatGemini
{
    private readonly string _apiKey;
    private readonly Client _client;
    private readonly List<Content> _chatHistory;

    public ChatGemini(string apiKey)
    {
        _apiKey = apiKey;
        _client = new Client(apiKey:_apiKey);
        _chatHistory = [];
    }

    public async Task<ChatResponse> Invoke(ChatRequest parameters)
    {
        try
        {
            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts =
                    [
                        new() { Text = parameters.SystemPrompt ?? "Você é um assistente muito útil"}
                    ]
                }
            };


            if (parameters.History != null)
            {
                foreach (var msg in parameters.History) 
                {
                    _chatHistory.Add(new Content
                    {
                        Role = msg.Role == "assistant" ? "model" : "user", 
                        Parts = [new Part { Text = msg.Message }]
                    });
                }
            }

            _chatHistory.Add(new Content
            {
                Role = "user",
                Parts = [new Part { Text = parameters.UserPrompt }]
            });

            GenerateContentResponse geminiResponse = await _client.Models.GenerateContentAsync(
                model: parameters.Model.Value(),
                contents: _chatHistory,
                config: config 
            );

            if (geminiResponse.Candidates == null || geminiResponse.Candidates.Count == 0)
                return new ChatResponse
                {
                    Content = string.Empty,
                    InputTokens = geminiResponse.UsageMetadata?.PromptTokenCount ?? 0,
                    OutputTokens = geminiResponse.UsageMetadata?.CandidatesTokenCount ?? 0,
                    TotalTokens = geminiResponse.UsageMetadata?.TotalTokenCount ?? 0
                };

            var candidate = geminiResponse.Candidates[0];
            if (candidate.Content is null || candidate.Content.Parts is null || candidate.Content.Parts.Count is 0)
                return new ChatResponse
                {
                    Content = string.Empty,
                    InputTokens = geminiResponse.UsageMetadata?.PromptTokenCount ?? 0,
                    OutputTokens = geminiResponse.UsageMetadata?.CandidatesTokenCount ?? 0,
                    TotalTokens = geminiResponse.UsageMetadata?.TotalTokenCount ?? 0,
                };

            return new ChatResponse
            {
                Content = candidate.Content.Parts[0].Text ?? string.Empty,
                InputTokens = geminiResponse.UsageMetadata?.PromptTokenCount ?? 0,
                OutputTokens = geminiResponse.UsageMetadata?.CandidatesTokenCount ?? 0,
                TotalTokens = geminiResponse.UsageMetadata?.TotalTokenCount ?? 0
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
            return new ChatResponse
            {
                Content = ex.Message,
                InputTokens = 0,
                OutputTokens = 0,
                TotalTokens = 0
            };
        }
    }

    

}
