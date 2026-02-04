using Google.GenAI;
using Google.GenAI.Types;
using Maestro.Gemini.Models;
using Newtonsoft.Json.Linq;

namespace Maestro.Gemini.Features;

public class ChatGemini
{
    /// <summary>
    /// Base declarations.
    /// </summary>
    private readonly Client _client;
    private readonly List<Content> _chatHistory;
    private readonly string[] modelsWithNoThinking = ["gemini-2.5-flash", "gemini-2.5-flash-lite", "gemini-2.5-pro", "gemini-2.0-flash"];

    public ChatGemini(GeminiMaestro maestro)
    {
        _client = maestro.Client;
        _chatHistory = [];
    }

    /// <summary>
    /// Invokes the AI model.
    /// </summary>
    /// <returns> Object ChatResponse </returns>
    public async Task<ChatResponse> Invoke(ChatRequest parameters)
    {
        try
        {
            /// :: Check if structured output is requested.
            if (parameters.ResponseSchema is not null)
            {
                throw new Exception("Use InvokeWithStructuredOutput method for structured outputs.");
            }

            /// :: Build the config for the request.    
            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts =
                    [
                        new() { Text = parameters.SystemPrompt ?? "Você é um assistente muito útil"}
                    ]
                },
                Temperature = parameters.Temperature ?? 0,
            };

            /// :: Thinking config only for models that support it.
            if (!modelsWithNoThinking.Contains(parameters.Model.Value()))
            {
                config.ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = parameters.ThinkingLevel ?? ThinkingLevel.LOW
                };
            }

            /// :: Build the chat history, if exists.
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

            /// :: Add the user prompt to the chat history.
            _chatHistory.Add(new Content
            {
                Role = "user",
                Parts = [new Part { Text = parameters.UserPrompt }]
            });

            /// :: Make the request to the Gemini API.
            GenerateContentResponse geminiResponse = await _client.Models.GenerateContentAsync(
                model: parameters.Model.Value(),
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
    public async Task<ChatResponse> WebSearchInvoke(ChatRequest parameters)
    {
        try
        {
            /// :: Check if structured output is requested.
            if (parameters.ResponseSchema is not null)
            {
                throw new Exception("Use InvokeWithStructuredOutput method for structured outputs.");
            }

            /// :: Build the config for the request.
            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts =
                    [
                        new() { Text = parameters.SystemPrompt ?? "Você é um assistente de pesquisas web muito útil"}
                    ]
                },
                Tools = [
                    new Google.GenAI.Types.Tool
                    {
                        GoogleSearch = new GoogleSearch()
                    }
                    ],
                Temperature = parameters.Temperature ?? 0
            };

            /// :: Thinking config only for models that support it.
            if (!modelsWithNoThinking.Contains(parameters.Model.Value()))
            {
                config.ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = parameters.ThinkingLevel ?? ThinkingLevel.LOW
                };
            }

            /// :: Build the chat history, if exists.
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

            /// :: Add the user prompt to the chat history.
            _chatHistory.Add(new Content
            {
                Role = "user",
                Parts = [new Part { Text = parameters.UserPrompt }]
            });

            /// :: Make the request to the Gemini API.
            GenerateContentResponse geminiResponse = await _client.Models.GenerateContentAsync(
                model: parameters.Model.Value(),
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
    public async Task<ChatResponse> InvokeWithStructuredOutput(ChatRequest parameters)
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
                        new() { Text = parameters.SystemPrompt ?? "Você é um assistente muito útil"}
                    ]
                },
                Temperature = parameters.Temperature ?? 0,
                ResponseSchema = parameters.ResponseSchema /// :: Sets the output response schema.

            };

            /// :: Thinking config only for models that support it.
            if (!modelsWithNoThinking.Contains(parameters.Model.Value()))
            {
                config.ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = parameters.ThinkingLevel ?? ThinkingLevel.LOW
                };
            }

            /// :: Sets cha hustory, if exists.
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

            /// :: Adds the user prompt to the chat.
            _chatHistory.Add(new Content
            {
                Role = "user",
                Parts = [new Part { Text = parameters.UserPrompt }]
            });


            /// :: Make the request to the Gemini API.
            GenerateContentResponse geminiResponse = await _client.Models.GenerateContentAsync(
                model: parameters.Model.Value(),
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
    public async Task<ChatResponse> InvokeMultimodalAgent(ChatRequest parameters, string filePath)
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
                config: new UploadFileConfig { DisplayName = "My file"}
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
                        new() { Text = parameters.SystemPrompt ?? "Você é um assistente muito útil"}
                    ]
                },
                Temperature = parameters.Temperature ?? 0,
            };

            /// :: Sets the response schema, if exists.
            if (parameters.ResponseSchema is not null)
            {
                config.ResponseSchema = parameters.ResponseSchema;
            }

            /// :: Thinking config only for models that support it.
            if (!modelsWithNoThinking.Contains(parameters.Model.Value()))
            {
                config.ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = parameters.ThinkingLevel ?? ThinkingLevel.LOW
                };
            }

            /// :: Sets chat history, if exists.
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
                Parts = [new Part { Text = parameters.UserPrompt }]
            });

            /// :: Make the request to the Gemini API.
            GenerateContentResponse geminiResponse = await _client.Models.GenerateContentAsync(
                model: parameters.Model.Value(),
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

}
