# Maestro 🤖

> Framework para orquestração de múltiplos provedores de IA e criação de agentes especializados.

## 📋 Sobre o Projeto

**Maestro** é um framework desenvolvido em .NET 10.0 que simplifica a integração com diferentes provedores de IA e a criação de agentes especializados. Atualmente suporta Google Gemini com funcionalidades avançadas como transcrição de áudio, análise de sentimentos e saídas estruturadas, com planos de expansão para incluir outros provedores como OpenAI, Anthropic, e mais.

## ✨ Características

- ✅ **Integração com Google Gemini**: Suporte completo aos modelos Gemini 2.0, 2.5 e 3.0
- 📚 **Gerenciamento de Histórico**: Controle de contexto de conversação
- 🔄 **Múltiplos Modelos**: Suporte para diversos modelos Gemini
- 🎯 **API Simples e Intuitiva**: Interface clara e fácil de usar
- ⚡ **Async/Await**: Suporte completo para operações assíncronas
- 📊 **Monitoramento de Tokens**: Acompanhamento detalhado do uso de tokens
- 📐 **Saídas Estruturadas**: Respostas em formato JSON com schema definido
- 🔍 **Google Search Integration**: Capacidade de pesquisa web integrada
- 🖼️ **Multimodal**: Processamento de imagens, documentos e áudio

## 🚀 Modelos Suportados até o momento

### Google Gemini

#### Gemini 3.0
| Modelo | Identificador | Suporte Thinking |
|--------|---------------|------------------|
| `Gemini_3_Pro` | `gemini-3-pro-preview` | ✅ |
| `Gemini_3_Flash` | `gemini-3-flash-preview` | ✅ |

#### Gemini 2.5
| Modelo | Identificador | Suporte Thinking |
|--------|---------------|------------------|
| `Gemini_2_5_Pro` | `gemini-2.5-pro` | ❌ |
| `Gemini_2_5_Flash` | `gemini-2.5-flash` | ❌ |
| `Gemini_2_5_Flash_Lite` | `gemini-2.5-flash-lite` | ❌ |

#### Gemini 2.0
| Modelo | Identificador | Suporte Thinking |
|--------|---------------|------------------|
| `Gemini_2_0_Flash` | `gemini-2.0-flash` | ❌ |

## 📦 Instalação

### Adicionar Referência ao Projeto

1. Clone o repositório Maestro
2. Adicione a referência ao seu projeto:

```xml
<ItemGroup>
  <ProjectReference Include="..\Maestro\Maestro.csproj" />
</ItemGroup>
```

Ou via linha de comando:

```bash
dotnet add reference ..\Maestro\Maestro.csproj
```

### Dependências Necessárias

O projeto requer as seguintes dependências que serão instaladas automaticamente:

```xml
<PackageReference Include="Google.GenAI" Version="0.14.0" />
<PackageReference Include="Microsoft.AspNetCore.StaticFiles" Version="2.3.9" />
```

## 🔧 Uso

### Configuração Básica

```csharp
using Maestro.Gemini;
using Maestro.Gemini.Models;

// Inicialize o GeminiClient com sua API Key
var client = new GeminiClient("sua-api-key-aqui");

// Crie um agente Gemini
var agent = client.CreateAgent(new CreateGeminiAgentRequest
{
    Model = GeminiModel.Gemini_2_5_Flash,
    SystemPrompt = "Você é um assistente útil e amigável.",
    UserPrompt = "Olá! Como você está?"
});
```

### Exemplo Simples

```csharp
var client = new GeminiClient("sua-api-key-aqui");

var agent = client.CreateAgent(new CreateGeminiAgentRequest
{
    Model = GeminiModel.Gemini_2_5_Flash,
    SystemPrompt = "Você é um assistente útil e amigável.",
    UserPrompt = "Olá! Como você está?"
});

var response = await agent.Invoke();

Console.WriteLine($"Resposta: {response.Content}");
Console.WriteLine($"Tokens usados: {response.TotalTokens}");
```

### Conversação com Histórico

```csharp
var client = new GeminiClient("sua-api-key-aqui");

var agent = client.CreateAgent(new CreateGeminiAgentRequest
{
    Model = GeminiModel.Gemini_2_5_Pro,
    SystemPrompt = "Você é um especialista em programação.",
    UserPrompt = "Como faço um loop em C#?",
    Temperature = 0.7f
});

// Adicione histórico de conversação
var chatHistory = new List<ChatMessage>
{
    new() { Role = "user", Message = "Olá!" },
    new() { Role = "assistant", Message = "Olá! Como posso ajudar?" }
};

agent.UpdateMemory(chatHistory);

var response = await agent.Invoke();
```

### Pesquisa Web

```csharp
var client = new GeminiClient("sua-api-key-aqui");

var agent = client.CreateAgent(new CreateGeminiAgentRequest
{
    Model = GeminiModel.Gemini_2_5_Flash,
    SystemPrompt = "Você é um assistente de pesquisas web.",
    UserPrompt = "Quais são as últimas notícias sobre IA?"
});

var response = await agent.WebSearchInvoke();
```

### Saídas Estruturadas

```csharp
var client = new GeminiClient("sua-api-key-aqui");

var agent = client.CreateAgent(new CreateGeminiAgentRequest
{
    Model = GeminiModel.Gemini_2_5_Flash,
    SystemPrompt = "Você é um assistente culinário.",
    UserPrompt = "Me dê uma receita de bolo de chocolate"
});

var responseSchema = new Schema
{
    Type = Google.GenAI.Types.Type.OBJECT,
    Properties = new Dictionary<string, Schema>
    {
        ["nome_receita"] = new Schema { Type = Google.GenAI.Types.Type.STRING },
        ["ingredientes"] = new Schema
        {
            Type = Google.GenAI.Types.Type.ARRAY,
            Items = new Schema { Type = Google.GenAI.Types.Type.STRING }
        }
    },
    Required = ["nome_receita", "ingredientes"]
};

var response = await agent.InvokeWithStructuredOutput(responseSchema);
```

### Processamento Multimodal (Imagens, Documentos)

```csharp
var client = new GeminiClient("sua-api-key-aqui");

var agent = client.CreateAgent(new CreateGeminiAgentRequest
{
    Model = GeminiModel.Gemini_2_5_Flash,
    SystemPrompt = "Você é um especialista em análise de documentos.",
    UserPrompt = "Descreva o conteúdo desta imagem"
});

var response = await agent.InvokeMultimodal("caminho/para/imagem.jpg");
```

## 📊 Estruturas de Dados

### CreateGeminiAgentRequest

```csharp
public struct CreateGeminiAgentRequest
{
    public GeminiModel Model { get; set; }
    public string SystemPrompt { get; set; }
    public string UserPrompt { get; set; }
    public ThinkingCapacity ThinkingCapacity { get; set; }
    public float Temperature { get; set; }
}
```

### ChatResponse

```csharp
public struct ChatResponse
{
    public string Content { get; set; }        // Conteúdo da resposta
    public int InputTokens { get; set; }       // Tokens do prompt
    public int OutputTokens { get; set; }      // Tokens da resposta
    public int TotalTokens { get; set; }       // Total de tokens
}
```

### ChatMessage

```csharp
public class ChatMessage
{
    public required string Role { get; set; }    // "user" ou "assistant"
    public required string Message { get; set; }
}
```

### ThinkingCapacity

```csharp
public enum ThinkingCapacity
{
    MINIMAL,
    LOW,
    MEDIUM,
    HIGH
}
```

## 🗂️ Estrutura do Projeto

```
Maestro/
├── Gemini/
│   ├── GeminiClient.cs            # Cliente base do Gemini
│   ├── GeminiAgent.cs             # Agente principal do Gemini
│   ├── Examples/
│   │   ├── Chatbot/
│   │   │   └── Chatbot.cs         # Exemplo de chatbot
│   │   └── TextTranscription/
│   │       ├── TranscriptionAgent.cs  # Agente de transcrição
│   │       └── Models/
│   │           ├── Prompts.cs     # Prompts do sistema
│   │           └── Outputs/
│   │               └── FeelingAnalysys.cs # Modelos de análise de sentimento
│   └── Models/
│       ├── ChatMessage.cs         # Modelo de mensagem de chat
│       ├── GeminiModels.cs        # Enums e extensões de modelos
│       ├── Requests/
│       │   └── CreateGeminiAgentRequest.cs # Modelo de criação de agente
│       └── Responses/
│           └── ChatResponse.cs    # Modelo de resposta
├── OpenAI/                        # (Em desenvolvimento)
└── Maestro.csproj
```

## 📝 Requisitos

- .NET 10.0 ou superior
- Visual Studio 2022 (ou VS Code)
- Chave de API do Google Gemini

## 🔐 Configuração de API Keys

### Variáveis de Ambiente (Recomendado)

**Windows:**
```powershell
setx GEMINI_API_KEY "sua-chave-aqui"
```

**Linux/Mac:**
```bash
export GEMINI_API_KEY="sua-chave-aqui"
```

### User Secrets (Desenvolvimento)

```bash
dotnet user-secrets init
dotnet user-secrets set "Gemini:ApiKey" "sua-chave-aqui"
```

## 🎯 Funcionalidades Avançadas

### Thinking Mode

Para modelos que suportam Thinking (Gemini 3.0), você pode configurar o nível de raciocínio:

```csharp
var agent = client.CreateAgent(new CreateGeminiAgentRequest
{
    Model = GeminiModel.Gemini_3_Pro,
    SystemPrompt = "Você é um matemático especialista.",
    UserPrompt = "Resolva este problema complexo...",
    ThinkingCapacity = ThinkingCapacity.HIGH
});
```

### Atualização de Mensagens

Você pode alterar a mensagem do usuário em um agente existente:

```csharp
agent.NewMessage("Nova pergunta para o agente");
var response = await agent.Invoke();
```

### Gerenciamento de Memória

Atualize o histórico de conversação do agente:

```csharp
var chatHistory = new List<ChatMessage>
{
    new() { Role = "user", Message = "Primeira pergunta" },
    new() { Role = "assistant", Message = "Primeira resposta" }
};

agent.UpdateMemory(chatHistory);
```

### Upload de Arquivos

A classe [`GeminiAgent`](Gemini/GeminiAgent.cs) faz upload automático de arquivos para a API Gemini e aguarda o processamento antes de enviar a requisição.

## 🛠️ Padrões de Código

- ✔️ Siga as convenções C# da Microsoft
- ✔️ Documente métodos públicos com XML comments
- ✔️ Use async/await para operações assíncronas
- ✔️ Utilize using statements para recursos descartáveis
- ✔️ Prefira structs para DTOs pequenos e imutáveis

## 👨‍💻 Desenvolvido por:

**Victor Hugo Porfirio**  

## 📄 Licença

Este projeto está em desenvolvimento ativo. Novas funcionalidades e agentes são adicionados regularmente.

