using Maestro.Gemini.Models;

namespace Maestro.Gemini.Examples.Chatbot;

/// <summary>
/// Example application that demonstrates how to use Gemini agents to create a chatbot that can retrieve information from a database (or any other source) and use it to answer user questions. The example focuses on the retrieval aspect, showcasing how to update the agent's memory with retrieved context before invoking it to generate a response. The retrieval logic itself is represented as a placeholder, where you can implement your own logic to fetch relevant information based on the user's query.
/// </summary>
public class Chatbot
{
    private readonly GeminiClient _maestro;
    private readonly GeminiAgent _chatbotAgent;

    public Chatbot(string apiKey)
    {
        _maestro = new GeminiClient(apiKey);

        /// :: Creating the agent with the desired model and system prompts.
        _chatbotAgent = _maestro.CreateAgent(new CreateGeminiAgentRequest
        {
            Model = GeminiModel.Gemini_2_5_Flash,
            SystemPrompt = "",
            UserPrompt = "",
        });
    }

    public async Task<string> Chat(string userMessage)
    {
        /// :: Logic for retrieving from database (Redis, SQL, etc.) or other sources should be implemented here.
        var exampleRetrievedContext = new List<ChatMessage>
        {
            new() {
                Role = "user",
                Message = "What is the weather like in New York?"
            },
            new() {
                Role = "assistant",
                Message = "The weather in New York is currently sunny with a high of 75°F."
            }
        };

        /// :: Updating agent memory with retrieval content.
        _chatbotAgent.UpdateMemory(exampleRetrievedContext);

        /// :: Adding the latest user message to the agent.
        _chatbotAgent.NewMessage(userMessage);

        /// :: Invoking the agent to get a response based on the conversation history and retrieved context.
        ChatResponse response = await _chatbotAgent.Invoke();

        /// :: Returning the assistant's response to the user.
        return response.Content;
    }
}
