namespace LadeviVentasApi.Helpers;


using System.Net.Http.Headers;
using System.Text;

public class HttpLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpLoggingHandler> logger;

    private readonly string[] types = new[] { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };

    public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger)
    {
        this.logger = logger;
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var req = request;
        var id = Guid.NewGuid().ToString();
        var logBuilder = new StringBuilder();
        var requestMsg = $"[{id} - Request]";
        var responseMsg = $"[{id} - Response]";

        // Request logging
        logBuilder.AppendLine($"{requestMsg}========Start==========");
        logBuilder.AppendLine($"{requestMsg} {req.Method} {req.RequestUri.PathAndQuery} {req.RequestUri.Scheme}/{req.Version}");
        logBuilder.AppendLine($"{requestMsg} Host: {req.RequestUri.Scheme}://{req.RequestUri.Host}");

        foreach (var header in req.Headers)
        {
            logBuilder.AppendLine($"{requestMsg} {header.Key}: {string.Join(", ", header.Value)}");
        }

        if (req.Content != null)
        {
            foreach (var header in req.Content.Headers)
            {
                logBuilder.AppendLine($"{requestMsg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (req.Content is StringContent || this.IsTextBasedContentType(req.Headers) ||
                this.IsTextBasedContentType(req.Content.Headers))
            {
                var result = await req.Content.ReadAsStringAsync();
                logBuilder.AppendLine($"{requestMsg} Content:");
                logBuilder.AppendLine($"{requestMsg} {result}");
            }
        }

        var start = DateTime.Now;
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var end = DateTime.Now;

        logBuilder.AppendLine($"{requestMsg} Duration: {end - start}");
        logBuilder.AppendLine($"{requestMsg}==========End==========");

        // Response logging
        logBuilder.AppendLine($"{responseMsg}=========Start=========");
        var resp = response;

        logBuilder.AppendLine(
            $"{responseMsg} {req.RequestUri.Scheme.ToUpper()}/{resp.Version} {(int)resp.StatusCode} {resp.ReasonPhrase}");

        foreach (var header in resp.Headers)
        {
            logBuilder.AppendLine($"{responseMsg} {header.Key}: {string.Join(", ", header.Value)}");
        }

        if (resp.Content != null)
        {
            foreach (var header in resp.Content.Headers)
            {
                logBuilder.AppendLine($"{responseMsg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (resp.Content is StringContent || this.IsTextBasedContentType(resp.Headers) ||
                this.IsTextBasedContentType(resp.Content.Headers))
            {
                start = DateTime.Now;
                var result = await resp.Content.ReadAsStringAsync();
                end = DateTime.Now;

                logBuilder.AppendLine($"{responseMsg} Content:");
                logBuilder.AppendLine($"{responseMsg} {result}");
                logBuilder.AppendLine($"{responseMsg} Duration: {end - start}");
            }
        }

        logBuilder.AppendLine($"{responseMsg}==========End==========");

        // Single log call with the entire message
        var logString = logBuilder.ToString();
        Console.Write(logString);
        return response;
    }

    private bool IsTextBasedContentType(HttpHeaders headers)
    {
        IEnumerable<string> values;
        if (!headers.TryGetValues("Content-Type", out values))
        {
            return false;
        }

        var header = string.Join(" ", values).ToLowerInvariant();

        return this.types.Any(t => header.Contains(t));
    }
}
