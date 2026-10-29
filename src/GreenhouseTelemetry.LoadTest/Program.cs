using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

// usage: LoadTest <method> <path> <concurrency> <requests> <baseUrl> [<baseUrl> ...]
var method = new HttpMethod(args[0]);
var path = args[1];
var concurrency = int.Parse(args[2]);
var total = int.Parse(args[3]);
var baseUrls = args[4..];

using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
var outcomes = new ConcurrentDictionary<string, int>();
var latencies = new ConcurrentBag<double>();
var next = -1;

var clock = Stopwatch.StartNew();
await Parallel.ForAsync(0, concurrency, new ParallelOptions { MaxDegreeOfParallelism = concurrency }, async (_, ct) =>
{
    int i;
    while ((i = Interlocked.Increment(ref next)) < total)
    {
        // Round-robin over the instances, like a load balancer would.
        var url = baseUrls[i % baseUrls.Length] + path.Replace("{n}", (i % 12 + 1).ToString());
        var started = Stopwatch.GetTimestamp();
        string outcome;
        try
        {
            using var request = new HttpRequestMessage(method, url);
            if (method == HttpMethod.Post && path == "/readings")
                request.Content = JsonContent(i);
            using var response = await http.SendAsync(request, ct);
            outcome = response.IsSuccessStatusCode
                ? $"{(int)response.StatusCode}"
                : $"{(int)response.StatusCode} {ErrorOf(await response.Content.ReadAsStringAsync(ct))}";
        }
        catch (Exception ex)
        {
            outcome = ex.GetType().Name;
        }
        latencies.Add(Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        outcomes.AddOrUpdate(outcome, 1, (_, count) => count + 1);
    }
});
clock.Stop();

var sorted = latencies.Order().ToArray();
Console.WriteLine($"{total} requests, concurrency {concurrency}, {clock.Elapsed.TotalSeconds:F1} s, {total / clock.Elapsed.TotalSeconds:F0} req/s");
Console.WriteLine($"latency p50 {Percentile(0.50):F0} ms, p95 {Percentile(0.95):F0} ms, max {sorted[^1]:F0} ms");
foreach (var (outcome, count) in outcomes.OrderByDescending(o => o.Value))
    Console.WriteLine($"  {count,5} x {outcome}");

double Percentile(double p) => sorted[(int)Math.Min(sorted.Length - 1, p * sorted.Length)];

static string ErrorOf(string body)
{
    try { return JsonDocument.Parse(body).RootElement.GetProperty("error").GetString() ?? body; }
    catch { return body; }
}

static StringContent JsonContent(int i) => new(
    JsonSerializer.Serialize(new { greenhouseId = i % 12 + 1, sensorCode = $"GH{i % 12 + 1}-S{i % 8}", temperatureC = 22.5m, humidityPct = 61m }),
    System.Text.Encoding.UTF8, "application/json");
