namespace Maestro.Gemini.Models;

public enum GeminiModel
{
    Gemini_3_Pro,
    Gemini_3_Flash,
    Gemini_2_5_Flash,
    Gemini_2_5_Flash_Lite,
    Gemini_2_5_Pro,
    Gemini_2_0_Flash
}

public static class GeminiModelExtensions
{
    public static string Value(this GeminiModel model) => model switch
    {
        GeminiModel.Gemini_3_Pro => "gemini-3-pro-preview",
        GeminiModel.Gemini_3_Flash => "gemini-3-flash-preview",
        GeminiModel.Gemini_2_5_Flash => "gemini-2.5-flash",
        GeminiModel.Gemini_2_5_Flash_Lite => "gemini-2.5-flash-lite",
        GeminiModel.Gemini_2_5_Pro => "gemini-2.5-pro",
        GeminiModel.Gemini_2_0_Flash => "gemini-2.0-flash",
        _ => throw new ArgumentOutOfRangeException(nameof(model), model, "Modelo Gemini não suportado.")
    };
}
