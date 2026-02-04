using Google.GenAI.Types;

namespace Maestro.Gemini.Templates;

public class StructuredOutputSchemaTemplate
{
    Schema meuSchema = new()
    {
        Type = Google.GenAI.Types.Type.OBJECT,
        Properties = new Dictionary<string, Schema>
        {
            ["nome_receita"] = new Schema { Type = Google.GenAI.Types.Type.STRING },
            ["ingredientes"] = new Schema
            {
                Type = Google.GenAI.Types.Type.ARRAY,
                Items = new Schema { Type = Google.GenAI.Types.Type.STRING}
            }
        },
        Required = ["nome_receita", "ingredientes"]
    };
}
