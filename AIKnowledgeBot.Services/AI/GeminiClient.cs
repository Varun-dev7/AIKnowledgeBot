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
            //_httpClient = httpClient;
            //_settings = options.Value;
            //_httpClient.BaseAddress =new Uri("https://generativelanguage.googleapis.com/v1beta/");
            //_httpClient.DefaultRequestHeaders.Clear();
            _httpClient = httpClient;
            _settings = options.Value;

            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");

            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    _settings.ApiKey);

        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var body = new
            {
                model = _settings.EmbeddingModel,
                input = text
            };

            var json = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                "embeddings",
                new StringContent(json, Encoding.UTF8, "application/json"));

            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseBody);

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(responseBody);

            var embedding = doc.RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding");

            float[] vector = new float[embedding.GetArrayLength()];

            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] = embedding[i].GetSingle();
            }

            return vector;
        }

        //public async Task<float[]> GenerateEmbeddingAsync(string text)
        //{
        //    var body = new
        //    {
        //        model = $"models/{_settings.EmbeddingModel}",
        //        content = new
        //        {
        //            parts = new[]
        //            {
        //        new
        //        {
        //            text
        //        }
        //    }
        //        }
        //    };

        //    var json = JsonSerializer.Serialize(body);

        //    var response = await _httpClient.PostAsync(
        //        $"models/{_settings.EmbeddingModel}:embedContent?key={_settings.ApiKey}",
        //        new StringContent(json, Encoding.UTF8, "application/json"));

        //    var responseBody = await response.Content.ReadAsStringAsync();

        //    response.EnsureSuccessStatusCode();

        //    using var doc = JsonDocument.Parse(responseBody);

        //    var embedding = doc.RootElement
        //        .GetProperty("embedding")
        //        .GetProperty("values");

        //    float[] vector = new float[embedding.GetArrayLength()];

        //    int i = 0;

        //    foreach (var item in embedding.EnumerateArray())
        //    {
        //        vector[i++] = item.GetSingle();
        //    }

        //    return vector;
        //}

        public async Task<string> GenerateContentAsync(string prompt)
        {
            var body = new
            {
                model = _settings.ChatModel,
                messages = new[]
                {
            new
            {
                role = "user",
                content = prompt
            }
        },
            };

            var json = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                "chat/completions",
                new StringContent(json, Encoding.UTF8, "application/json"));

            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseBody);

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(responseBody);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }

        //public async Task<string> GenerateContentAsync(string prompt)
        //{
        //    var body = new
        //    {
        //        contents = new[]
        //        {
        //    new
        //    {
        //        parts = new[]
        //        {
        //            new
        //            {
        //                text = prompt
        //            }
        //        }
        //    }
        //},
        //        generationConfig = new
        //        {
        //            temperature = 0.1
        //        }
        //    };

        //    var json = JsonSerializer.Serialize(body);

        //    var response = await _httpClient.PostAsync(
        //        $"models/{_settings.ChatModel}:generateContent?key={_settings.ApiKey}",
        //        new StringContent(json, Encoding.UTF8, "application/json"));

        //    response.EnsureSuccessStatusCode();

        //    var responseBody = await response.Content.ReadAsStringAsync();

        //    using var doc = JsonDocument.Parse(responseBody);

        //    return doc.RootElement
        //        .GetProperty("candidates")[0]
        //        .GetProperty("content")
        //        .GetProperty("parts")[0]
        //        .GetProperty("text")
        //        .GetString() ?? string.Empty;
        //}

        public Task<string> GenerateAnswerAsync(string prompt)
        {
            return GenerateContentAsync(prompt);
        }
    }
}
