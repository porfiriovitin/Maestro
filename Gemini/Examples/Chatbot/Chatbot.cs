using Maestro.Gemini.Models;

namespace Maestro.Gemini.Examples.Chatbot
{
    public class Chatbot
    {

        private readonly GeminiClient _maestro;
        private readonly GeminiAgent _chatbotAgent;

        public Chatbot(string apiKey)
        {
            _maestro = new GeminiClient(apiKey);

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
                    Role = "system",
                    Message = "You are a helpful assistant for a travel booking website."
                },

                new() {
                    Role = "user",
                    Message = "What is the weather like in New York?"
                },

                new() {
                    Role = "assistant",
                    Message = "The weather in New York is currently sunny with a high of 75°F."
                }
            };

            _chatbotAgent.UpdateMemory(exampleRetrievedContext);

            _chatbotAgent.NewMessage(userMessage);

            ChatResponse response = await _chatbotAgent.Invoke();

            return response.Content;
        }
    }
}
