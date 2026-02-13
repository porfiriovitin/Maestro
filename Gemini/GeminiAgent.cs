using Google.GenAI;
using Google.GenAI.Types;
using Maestro.Gemini.Models;
using Newtonsoft.Json.Linq;

namespace Maestro.Gemini;

public class GeminiAgent(Client client, CreateGeminiAgentRequest parameters)
{
    /// <summary>
    /// Base declarations.
    /// </summary>
    private readonly Client _client = client ?? throw new ArgumentNullException(nameof(client));
    private readonly string[] modelsWithNoThinking = ["gemini-2.5-flash", "gemini-2.5-flash-lite", "gemini-2.5-pro", "gemini-2.0-flash"];

    private List<Content> _chatHistory = [];
    private readonly GeminiModel _model = parameters.Model != default ? parameters.Model : GeminiModel.Gemini_2_5_Flash_Lite;
    private readonly string _systemPrompt = parameters.SystemPrompt ?? "Você é um assistente muito útil";
    private string _userPrompt = parameters.UserPrompt;
    private readonly ThinkingLevel _thinkingLevel = parameters.ThinkingCapacity != default ? parameters.ThinkingCapacity.ToThinkingLevel() : ThinkingCapacity.LOW.ToThinkingLevel();
    private readonly float _temperature = parameters.Temperature;

    /// <summary>
    /// Invokes the AI model.
    /// </summary>
    /// <returns> Object ChatResponse </returns>
    public async Task<ChatResponse> Invoke()
    {
        try
        {
            /// :: Build the config for the request.    
            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts =
                    [
                        new() { Text = this._systemPrompt}
                    ]
                },
                Temperature = this._temperature,
            };

            /// :: Thinking config only for models that support it.
            if (!modelsWithNoThinking.Contains(this._model.Value()))
            {
                config.ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = this._thinkingLevel
                };
            }

            /// :: Add the user prompt to the chat history.
            _chatHistory.Add(new Content
            {
                Role = "user",
                Parts = [new Part { Text = this._userPrompt }]
            });

            /// :: Make the request to the Gemini API.
            GenerateContentResponse geminiResponse = await _client.Models.GenerateContentAsync(
                model: this._model.Value(),
                contents: _chatHistory,
                config: config
            );

            /// :: Process and validate the response.
            if (geminiResponse.Candidates == null || geminiResponse.Candidates.Count == 0)
                return new ChatResponse
                {
                    Content = string.Empty,
                    InputTokens = geminiResponse.UsageMetadata?.PromptTokenCount ?? 0,
                    OutputTokens = geminiResponse.UsageMetadata?.CandidatesTokenCount ?? 0,
                    TotalTokens = geminiResponse.UsageMetadata?.TotalTokenCount ?? 0
                };

            /// :: Get the first candidate from the response and validate.
            var candidate = geminiResponse.Candidates[0];
            if (candidate.Content is null || candidate.Content.Parts is null || candidate.Content.Parts.Count is 0)
                return new ChatResponse
                {
                    Content = string.Empty,
                    InputTokens = geminiResponse.UsageMetadata?.PromptTokenCount ?? 0,
                    OutputTokens = geminiResponse.UsageMetadata?.CandidatesTokenCount ?? 0,
                    TotalTokens = geminiResponse.UsageMetadata?.TotalTokenCount ?? 0,
                };

            /// :: Return the final ChatResponse.
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

    /// <summary>
    /// Invokes the AI model with web search capability.
    /// </summary>
    /// <returns> Object ChatResponse </returns>
    public async Task<ChatResponse> WebSearchInvoke()
    {
        try
        {
            /// :: Build the config for the request.
            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts =
                    [
                        new() { Text = this._systemPrompt}
                    ]
                },
                Tools = [
                    new Tool
                    {
                        GoogleSearch = new GoogleSearch()
                    }
                    ],
                Temperature = this._temperature
            };

            /// :: Thinking config only for models that support it.
            if (!modelsWithNoThinking.Contains(this._model.Value()))
            {
                config.ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = this._thinkingLevel
                };
            }

            /// :: Add the user prompt to the chat history.
            _chatHistory.Add(new Content
            {
                Role = "user",
                Parts = [new Part { Text = this._userPrompt }]
            });

            /// :: Make the request to the Gemini API.
            GenerateContentResponse geminiResponse = await _client.Models.GenerateContentAsync(
                model: this._model.Value(),
                contents: _chatHistory,
                config: config
            );

            /// :: Process and validate the response.
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

            /// :: Return final ChatResponse.
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

    /// <summary>
    /// Invokes the AI model with structured output.
    /// </summary>
    /// <returns> Object ChatResponse with the Json string, must serialize </returns>
    public async Task<ChatResponse> InvokeWithStructuredOutput(Schema responseSchema)
    {
        try
        {
            /// :: Build the config for the request
            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts =
                    [
                        new() { Text = this._systemPrompt}
                    ]
                },
                Temperature = this._temperature,
                ResponseSchema = responseSchema /// :: Sets the output response schema.

            };

            /// :: Thinking config only for models that support it.
            if (!modelsWithNoThinking.Contains(this._model.Value()))
            {
                config.ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = this._thinkingLevel
                };
            }

            /// :: Adds the user prompt to the chat.
            _chatHistory.Add(new Content
            {
                Role = "user",
                Parts = [new Part { Text = this._userPrompt }]
            });


            /// :: Make the request to the Gemini API.
            GenerateContentResponse geminiResponse = await _client.Models.GenerateContentAsync(
                model: this._model.Value(),
                contents: _chatHistory,
                config: config
            );

            /// :: Processes and validates the response.
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

            /// :: Return the final ChatResponse.
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

    /// <summary>
    /// Invokes the AI model with multimodal capability like image OCR, docs processing, etc.
    /// </summary>
    /// <returns> Object ChatResponse </returns>
    public async Task<ChatResponse> InvokeMultimodal(string filePath, Schema? responseSchema = null)
    {
        try
        {
            /// :: Verify if there is a file path.
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("You must pass a file path for this agent, if you only want text, use Invoke method");
            }

            /// :: Upload the file to Gemini API.
            var uploadResult = await _client.Files.UploadAsync(
                filePath: filePath,
                config: new UploadFileConfig { DisplayName = "My file" }
            );

            /// :: Wait until the file is processed.
            var currentFileState = uploadResult;

            while (uploadResult.State == FileState.PROCESSING)
            {
                await Task.Delay(2000);

                currentFileState = await _client.Files.GetAsync(uploadResult.Name!);
            }

            /// :: Build the config for the request.
            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts =
                    [
                        new() { Text = this._systemPrompt}
                    ]
                },
                Temperature = this._temperature,
            };

            /// :: Sets the response schema, if exists.
            if (responseSchema is not null)
            {
                config.ResponseSchema = responseSchema;
            }

            /// :: Thinking config only for models that support it.
            if (!modelsWithNoThinking.Contains(this._model.Value()))
            {
                config.ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = this._thinkingLevel
                };
            }

            /// :: Adds the uploaded file to the chat history.
            _chatHistory.Add(new Content
            {
                Role = "user",
                Parts =
                [
                    new() {
                        FileData = new FileData
                        {
                            FileUri = uploadResult.Uri,
                            MimeType = uploadResult.MimeType
                        }
                    },
                ]
            });

            /// :: Adds the user prompt to the chat history.
            _chatHistory.Add(new Content
            {
                Role = "user",
                Parts = [new Part { Text = this._userPrompt }]
            });

            /// :: Make the request to the Gemini API.
            GenerateContentResponse geminiResponse = await _client.Models.GenerateContentAsync(
                model: this._model.Value(),
                contents: _chatHistory,
                config: config
            );

            /// :: Process and validate the response.
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

            /// :: Return final ChatResponse.
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

    /// <summary>
    /// Sets the user prompt to a new value.
    /// </summary>
    /// <param name="newPrompt">The new prompt text to assign. Cannot be null.</param>
    public void NewMessage(string newPrompt)
    {
        this._userPrompt = newPrompt;
    }

    /// <summary>
    /// Update agent memory.
    /// </summary>
    /// <param name="messages">History chat messages</param>
    public void UpdateMemory(List<ChatMessage> messages)
    {
        if (messages == null || messages.Count == 0)
            return;

        foreach (var message in messages)
        {
            _chatHistory.Add(new Content
            {
                Role = message.Role == "assistant" ? "model" : "user",
                Parts = [new Part { Text = message.Message }]
            });
        }
    }

}