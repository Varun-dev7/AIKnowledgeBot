using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.Models.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.AI
{
    public class GeminiClient : IGeminiClient
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;

        public GeminiClient(
            HttpClient httpClient,
            IOptions<GeminiSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.EmbeddingModel}:embedContent?key={_settings.ApiKey}";

            var body = new
            {
                model = $"models/{_settings.EmbeddingModel}",
                content = new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = text
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(responseString);

            var values = document.RootElement
                .GetProperty("embedding")
                .GetProperty("values");

            var vector = new float[values.GetArrayLength()];

            int i = 0;

            foreach (var value in values.EnumerateArray())
            {
                vector[i++] = value.GetSingle();
            }

            return vector;
        }

        public async Task<string> GenerateAnswerAsync(string prompt)
        {
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.ChatModel}:generateContent?key={_settings.ApiKey}";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(responseString);

            return document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;
        }
    }
}
