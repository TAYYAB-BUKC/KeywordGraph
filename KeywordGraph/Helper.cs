using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

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
        sw.WriteLine("id,title,x,y");

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
}