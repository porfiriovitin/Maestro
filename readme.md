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
- 🎙️ **Transcrição de Áudio**: Suporte para transcrição de múltiplos formatos de áudio
- 🧠 **Análise de Sentimentos**: Análise automática de emoções em áudio
- 📐 **Saídas Estruturadas**: Respostas em formato JSON com schema definido
- 🔍 **Google Search Integration**: Capacidade de pesquisa web integrada
- 🖼️ **Multimodal**: Processamento de imagens, documentos e áudio

## 🚀 Modelos Suportados

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
using Maestro.Gemini.Features;
using Maestro.Gemini.Models;

// Inicialize o Maestro com sua API Key
var maestro = new GeminiMaestro("sua-api-key-aqui");

// Inicialize o ChatGemini
var chat = new ChatGemini(maestro);
```

### Exemplo Simples

```csharp
var request = new ChatRequest
{
    Model = GeminiModel.Gemini_2_5_Flash,
    SystemPrompt = "Você é um assistente útil e amigável.",
    UserPrompt = "Olá! Como você está?"
};

var response = await chat.Invoke(request);

Console.WriteLine($"Resposta: {response.Content}");
Console.WriteLine($"Tokens usados: {response.TotalTokens}");
```

### Conversação com Histórico

```csharp
var request = new ChatRequest
{
    Model = GeminiModel.Gemini_2_5_Pro,
    SystemPrompt = "Você é um especialista em programação.",
    UserPrompt = "Como faço um loop em C#?",
    History = new List<ChatMessage>
    {
        new() { Role = "user", Message = "Olá!" },
        new() { Role = "assistant", Message = "Olá! Como posso ajudar?" }
    },
    Temperature = 0.7f
};

var response = await chat.Invoke(request);
```

### Pesquisa Web

```csharp
var request = new ChatRequest
{
    Model = GeminiModel.Gemini_2_5_Flash,
    SystemPrompt = "Você é um assistente de pesquisas web.",
    UserPrompt = "Quais são as últimas notícias sobre IA?"
};

var response = await chat.WebSearchInvoke(request);
```

### Saídas Estruturadas

```csharp
var request = new ChatRequest
{
    Model = GeminiModel.Gemini_2_5_Flash,
    SystemPrompt = "Você é um assistente culinário.",
    UserPrompt = "Me dê uma receita de bolo de chocolate",
    ResponseSchema = new Schema
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
    }
};

var response = await chat.InvokeWithStructuredOutput(request);
```

### Transcrição de Áudio

```csharp
var transcription = new Transcription(maestro);

// Transcrição simples
string texto = await transcription.TranscribeAudio("caminho/para/audio.mp3");
Console.WriteLine($"Transcrição: {texto}");

// Transcrição com análise de sentimentos
FeelingAnalysysOutput resultado = await transcription.TranscribeAndAnalyze("caminho/para/audio.wav");

Console.WriteLine($"Texto: {resultado.TranscriptedText}");
Console.WriteLine($"Sentimento: {resultado.FeelingAnalysys.DominantFeeling}");
Console.WriteLine($"Confiança: {resultado.FeelingAnalysys.ConfidenceLevel}");
Console.WriteLine($"Justificativa: {resultado.FeelingAnalysys.Justification}");
```

### Processamento Multimodal (Imagens, Documentos)

```csharp
var request = new ChatRequest
{
    Model = GeminiModel.Gemini_2_5_Flash,
    SystemPrompt = "Você é um especialista em análise de documentos.",
    UserPrompt = "Descreva o conteúdo desta imagem"
};

var response = await chat.InvokeMultimodalAgent(request, "caminho/para/imagem.jpg");
```

## 🎙️ Formatos de Áudio Suportados

A classe [`Transcription`](Gemini/Features/Transcription.cs) suporta os seguintes formatos de áudio:

- **WAV** (`.wav`) - RIFF WAVE
- **MP3** (`.mp3`) - MPEG Audio Layer 3
- **FLAC** (`.flac`) - Free Lossless Audio Codec
- **OGG** (`.ogg`) - Ogg Vorbis
- **M4A/MP4** (`.m4a`, `.mp4`) - MPEG-4 Audio
- **AAC** (`.aac`) - Advanced Audio Coding (ADTS)

## 📊 Estruturas de Dados

### ChatRequest

```csharp
public struct ChatRequest
{
    public GeminiModel Model { get; set; }
    public string SystemPrompt { get; set; }
    public string UserPrompt { get; set; }
    public List<ChatMessage>? History { get; set; }
    public float? Temperature { get; set; }
    public ThinkingLevel? ThinkingLevel { get; set; }
    public Schema? ResponseSchema { get; set; }
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

### FeelingAnalysysOutput

```csharp
public struct FeelingAnalysysOutput
{
    public string TranscriptedText { get; set; }
    public FeelingAnalysysDetail FeelingAnalysys { get; set; }
}

public struct FeelingAnalysysDetail
{
    public string DominantFeeling { get; set; }
    public string ConfidenceLevel { get; set; }  // "alto", "médio" ou "baixo"
    public string Justification { get; set; }
}
```

## 🗂️ Estrutura do Projeto

```
Maestro/
├── Gemini/
│   ├── GeminiMaestro.cs           # Cliente base do Gemini
│   ├── Features/
│   │   ├── ChatGemini.cs          # Funcionalidades de chat
│   │   └── Transcription.cs       # Transcrição e análise de áudio
│   ├── Models/
│   │   ├── GeminiModels.cs        # Enums e extensões de modelos
│   │   ├── Prompts.cs             # Prompts do sistema
│   │   ├── Requests/
│   │   │   └── ChatRequest.cs     # Modelo de requisição
│   │   ├── Responses/
│   │   │   └── ChatResponse.cs    # Modelo de resposta
│   │   └── Outputs/
│   │       └── FeelingAnalysys.cs # Modelos de saída estruturada
│   └── Templates/
│       └── StructuredOutputSchemaTemplate.cs
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
var request = new ChatRequest
{
    Model = GeminiModel.Gemini_3_Pro,
    SystemPrompt = "Você é um matemático especialista.",
    UserPrompt = "Resolva este problema complexo...",
    ThinkingLevel = ThinkingLevel.HIGH
};
```

### Upload de Arquivos

A classe [`ChatGemini`](Gemini/Features/ChatGemini.cs) faz upload automático de arquivos para a API Gemini e aguarda o processamento antes de enviar a requisição.

## 🛠️ Padrões de Código

- ✔️ Siga as convenções C# da Microsoft
- ✔️ Mantenha a cobertura de testes acima de 80%
- ✔️ Documente métodos públicos com XML comments
- ✔️ Use async/await para operações assíncronas
- ✔️ Utilize using statements para recursos descartáveis
- ✔️ Prefira structs para DTOs pequenos e imutáveis

## 🔄 Roadmap

- [ ] Suporte para OpenAI GPT-4
- [ ] Suporte para Anthropic Claude
- [ ] Sistema de cache de respostas
- [ ] Suporte para streaming de respostas
- [ ] Interface CLI para testes rápidos
- [ ] Package NuGet oficial

## 👨‍💻 Desenvolvido por:

**Victor Hugo Porfirio**  
*Software Engineer*

## 📄 Licença

Este projeto está em desenvolvimento ativo. Novas funcionalidades e agentes são adicionados regularmente.

**© 2026 Usecorp - Bens por Assinatura. Todos os direitos reservados.**