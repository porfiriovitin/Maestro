# Maestro 🤖

> Framework para orquestração de múltiplos provedores de IA e criação de agentes.

## 📋 Sobre o Projeto

**Maestro** é um framework desenvolvido em .NET 10.0 que simplifica a integração com diferentes provedores de IA e a criação de agentes. Atualmente suporta Google Gemini, com planos de expansão para incluir outros provedores como OpenAI, Anthropic, e mais.

## ✨ Características

- ✅ **Integração com Google Gemini**: Suporte completo aos modelos Gemini 2.0, 2.5 e 3.0
- 📚 **Gerenciamento de Histórico**: Controle de contexto de conversação
- 🔄 **Múltiplos Modelos**: Suporte para diversos modelos Gemini
- 🎯 **API Simples e Intuitiva**: Interface clara e fácil de usar
- ⚡ **Async/Await**: Suporte completo para operações assíncronas
- 📊 **Monitoramento de Tokens**: Acompanhamento detalhado do uso de tokens

## 🚀 Modelos Suportados

### Google Gemini

#### Gemini 3.0
| Modelo | Identificador |
|--------|---------------|
| `Gemini_3_Pro` | `gemini-3-pro-preview` |
| `Gemini_3_Flash` | `gemini-3-flash-preview` |

#### Gemini 2.5
| Modelo | Identificador |
|--------|---------------|
| `Gemini_2_5_Pro` | `gemini-2.5-pro` |
| `Gemini_2_5_Flash` | `gemini-2.5-flash` |
| `Gemini_2_5_Flash_Lite` | `gemini-2.5-flash-lite` |

#### Gemini 2.0
| Modelo | Identificador |
|--------|---------------|
| `Gemini_2_0_Flash` | `gemini-2.0-flash` |

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
```

## 🔧 Uso

### Configuração Básica

```csharp
using Maestro.Gemini;
using Maestro.Gemini.Models;

// Inicialize o cliente com sua API Key
var chat = new ChatGemini("sua-api-key-aqui");
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
    }
};

var response = await chat.Invoke(request);
```

## 📊 Estrutura de Resposta

```csharp
public struct ChatResponse
{
    public string Content { get; set; }        // Conteúdo da resposta
    public int InputTokens { get; set; }       // Tokens do prompt
    public int OutputTokens { get; set; }      // Tokens da resposta
    public int TotalTokens { get; set; }       // Total de tokens
}
```

## 🗂️ Estrutura do Projeto

```
Maestro/
├── Gemini/
│   ├── ChatGemini.cs              # Cliente principal do Gemini
│   └── Models/
│       ├── GeminiModels.cs        # Enums e extensões de modelos
│       ├── Requests/
│       │   └── ChatRequest.cs     # Modelo de requisição
│       └── Responses/
│           └── ChatResponse.cs    # Modelo de resposta
├── OpenAI/                        # (Em desenvolvimento)
└── Maestro.csproj
```

## 📝 Requisitos

- .NET 10.0 ou superior
- Visual Studio (ou VS Code)
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

### Padrões de Código

- ✔️ Siga as convenções C# da Microsoft
- ✔️ Mantenha a cobertura de testes acima de 80%
- ✔️ Documente métodos públicos com XML comments
- ✔️ Use async/await para operações assíncronas
- 

## 👨‍💻 por:

**Victor Hugo Porfirio**  
*Software Engineer*

---

*Este projeto está em desenvolvimento ativo. Novas funcionalidades e agentes são adicionados regularmente.*

**© 2026 Usecorp - Bens por Assinatura. Todos os direitos reservados.**