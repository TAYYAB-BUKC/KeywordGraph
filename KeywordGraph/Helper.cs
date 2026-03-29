using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using KeywordGraph;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models;

namespace KeywordGraph;

public class Helper
{
    public static string GetEnvironmentVariable(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
            throw new Exception($"Missing Environment Variable: {key}");
        return value!;
    }

    public static void TransformAndSaveCsv(List<(string Word, float[] Vec)> data, string path)
    {
        if (data.Count == 0)
        {
            Console.WriteLine("No vectors to project.");
            return;
        }

        // Build an n x d matrix (double) and mean-center
        int n = data.Count;
        int d = data[0].Vec.Length;

        var X = Matrix<double>.Build.Dense(n, d, (i, j) => data[i].Vec[j]);
        // Mean-center columns
        var means = Vector<double>.Build.Dense(d);
        for (int j = 0; j < d; j++)
        {
            means[j] = X.Column(j).Average();
            for (int i = 0; i < n; i++)
            {
                X[i, j] -= means[j];
            }
        }

        // PCA via SVD of mean-centered X
        // X = U * S * V^T, principal directions = V columns
        var svd = X.Svd(computeVectors: true);
        var V = svd.VT.Transpose(); // d x d

        // Take first two principal components
        var V2 = V.SubMatrix(0, d, 0, 2); // d x 2
        var Y = X * V2; // n x 2

        // Write CSV: id,title,x,y (culture-invariant)
        using var sw = new StreamWriter(path, false, Encoding.UTF8);
        sw.WriteLine("WORD,X,Y");

        for (int i = 0; i < n; i++)
        {
            var x = Y[i, 0];
            var y = Y[i, 1];

            sw.WriteLine($"{CsvEscape(data[i].Word)}, {x.ToString(CultureInfo.InvariantCulture)},{y.ToString(CultureInfo.InvariantCulture)}");
        }
    }

    public static string CsvEscape(string s)
    {
        if (s is null) return "";
        var needsQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n');
        if (needsQuotes)
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }

    public static async Task GenerateEmbeddings(string[] words)
    {
        var uri = new Uri("http://localhost:11434");
        var ollama = new OllamaApiClient(uri);
        var vectors = new List<(string Word, float[] Vectors)>();

        ollama.SelectedModel = "qwen3-embedding:0.6b";
        EmbeddingGenerationOptions embeddingOptions = new()
        {
            Dimensions = 512
        };

        foreach (var word in words)
        {
            var embeddings = await ollama.GenerateVectorAsync(word, embeddingOptions);
            vectors.Add((word, embeddings.ToArray()));
        }

        TransformAndSaveCsv(vectors, "animals.csv");
    }

    public static async Task GenerateEmbeddingsUsingEmbedRequest(string[] words)
    {
        var uri = new Uri("http://localhost:11434");
        var ollama = new OllamaApiClient(uri);
        var vectors = new List<(string Word, float[] Vectors)>();

        ollama.SelectedModel = "qwen3-embedding";

        EmbeddingGenerationOptions embeddingOptions = new();
        embeddingOptions.Dimensions = 512;
        embeddingOptions.ModelId = "qwen3-embedding";

        foreach (var word in words)
        {
            try
            {
                EmbedRequest request = new();
                request.Dimensions = 512;
                request.Input = new List<string>() { "word" };
                request.Model = "qwen3-embedding";

                var embeddings = await ollama.EmbedAsync(request);
                //Console.WriteLine(embeddings.ToArray());
                vectors.Add((word, embeddings.Embeddings[0].ToArray()));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        TransformAndSaveCsv(vectors, "animals.csv");
    }
}