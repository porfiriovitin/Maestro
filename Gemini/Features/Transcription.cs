using Maestro.Gemini.Models;
using System.Buffers.Binary;
using System.Text.Json;

namespace Maestro.Gemini.Features;

public class Transcription
{
    /// <summary>
    /// Base.
    /// </summary>
    private ChatGemini _gemini;

    public Transcription(GeminiMaestro maestro )
    {
        _gemini = new ChatGemini(maestro);
    }

    /// <summary>
    /// Transcribes the audio file to text.
    /// </summary>
    /// <param name="audioPath"> Path for the audio file </param>
    /// <returns> The transcribed text </returns>
    public async Task<String> TranscribeAudio(string audioPath)
    {
        if (!IsAudioFile(audioPath, out var reason))
            throw new ArgumentException($"O arquivo fornecido não é um arquivo de áudio válido: {reason}");

        ChatResponse response = await _gemini.InvokeMultimodalAgent(new ChatRequest()
        {
            Model = GeminiModel.Gemini_2_5_Flash,
            SystemPrompt = Prompts.AudioTranscription.systemPrompt,
            UserPrompt = Prompts.AudioTranscription.userPrompt,
        }, filePath: audioPath);

        return response.Content;
    }

    /// <summary>
    /// Transcribes the audio file to text and analyzes the feeling expressed in it.
    /// </summary>
    /// <param name="audioPath"> Path for the audio file </param>
    /// <returns> Object FeelingAnalysysOutput </returns>
    public async Task<FeelingAnalysysOutput> TranscribeAndAnalyze(string audioPath)
    {
        if (!IsAudioFile(audioPath, out var reason))
            throw new ArgumentException($"O arquivo fornecido não é um arquivo de áudio válido: {reason}");

        ChatResponse response = await _gemini.InvokeMultimodalAgent(new ChatRequest()
        {
            Model = GeminiModel.Gemini_2_5_Flash,
            SystemPrompt = Prompts.FeelingAnalysys.systemPrompt,
            UserPrompt = Prompts.FeelingAnalysys.userPrompt,
            ResponseSchema = FeelingAnalysys.OutputSchema
        }, filePath: audioPath);

        FeelingAnalysysOutput parsedResponse = JsonSerializer.Deserialize<FeelingAnalysysOutput>(response.Content);

        return parsedResponse;
    }

    /// <summary>
    /// Verifies if the inputed file is an audio file.
    /// </summary>
    /// <param name="audioPath">Path for the audio file</param>
    /// <param name="reason">Validation failed reason</param>
    internal static bool IsAudioFile(string audioPath, out string reason)
    {
        reason = string.Empty;

        /// :: Base validations;
        if (string.IsNullOrWhiteSpace(audioPath))
        {
            reason = "caminho vazio.";
            return false;
        }

        if (!File.Exists(audioPath))
        {
            reason = "arquivo não encontrado.";
            return false;
        }

        var fi = new FileInfo(audioPath);
        if ((fi.Attributes & FileAttributes.Directory) != 0)
        {
            reason = "o caminho aponta para um diretório.";
            return false;
        }

        if (fi.Length < 12)
        {
            reason = "arquivo muito pequeno para conter cabeçalho válido.";
            return false;
        }

        /// :: Magig number checks;
        Span<byte> header = stackalloc byte[64];
        using (var fs = new FileStream(audioPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var read = fs.Read(header);
            header = header[..read];
        }

        /// :: Check known audio formats;
        if (LooksLikeWav(header) ||
            LooksLikeMp3(header) ||
            LooksLikeFlac(header) ||
            LooksLikeOgg(header) ||
            LooksLikeM4aMp4(header) ||
            LooksLikeAacAdts(header))
        {
            return true;
        }

        reason = "formato de áudio não reconhecido.";
        return false;
    }

    /// <summary>
    /// Format validations based on magic numbers.
    /// </summary>
    private static bool LooksLikeWav(ReadOnlySpan<byte> h)
        => h.Length >= 12
           && h[0] == (byte)'R' && h[1] == (byte)'I' && h[2] == (byte)'F' && h[3] == (byte)'F'
           && h[8] == (byte)'W' && h[9] == (byte)'A' && h[10] == (byte)'V' && h[11] == (byte)'E';

    private static bool LooksLikeFlac(ReadOnlySpan<byte> h)
        => h.Length >= 4
           && h[0] == (byte)'f' && h[1] == (byte)'L' && h[2] == (byte)'a' && h[3] == (byte)'C';

    private static bool LooksLikeOgg(ReadOnlySpan<byte> h)
        => h.Length >= 4
           && h[0] == (byte)'O' && h[1] == (byte)'g' && h[2] == (byte)'g' && h[3] == (byte)'S';

    private static bool LooksLikeMp3(ReadOnlySpan<byte> h)
    {
        if (h.Length < 3) return false;

        /// :: ID3v2 tag
        if (h[0] == (byte)'I' && h[1] == (byte)'D' && h[2] == (byte)'3')
            return true;

        /// :: MP3 frame sync (MPEG-1, MPEG-2, MPEG-2.5)
        if (h.Length >= 2 && h[0] == 0xFF && (h[1] & 0xE0) == 0xE0)
            return true;

        return false;
    }

    private static bool LooksLikeM4aMp4(ReadOnlySpan<byte> h)
    {
        /// :: 'ftyp' box at offset 4
        if (h.Length < 12) return false;
        if (!(h[4] == (byte)'f' && h[5] == (byte)'t' && h[6] == (byte)'y' && h[7] == (byte)'p'))
            return false;

        var brand = BinaryPrimitives.ReadUInt32BigEndian(h.Slice(8, 4));
        /// :: 'M4A ' / 'isom' / 'mp42' / 'MSNV' etc. (inclui variações)
        return brand == 0x4D344120u   /// :: M4A<space>
               || brand == 0x69736F6Du /// :: isom
               || brand == 0x6D703432u /// :: mp42
               || brand == 0x4D534E56u /// :: MSNV
               || brand == 0x4D503431u; //// :: MP41
    }

    private static bool LooksLikeAacAdts(ReadOnlySpan<byte> h)
    {
        /// :: ADTS: 12 bits syncword 0xFFF
        if (h.Length < 2) return false;
        return h[0] == 0xFF && (h[1] & 0xF0) == 0xF0;
    }
}
