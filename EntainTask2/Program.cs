
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
        .AddConsole();
});
ILogger _logger = loggerFactory.CreateLogger<Program>();

var urls = new List<string> {
    "https://google.com",
    "https://linkedin.com",
    "https://x.com"
};
var contents = await DownloadPagesAsync(urls);
foreach (var content in contents) { Console.WriteLine(content); }

async Task<string[]> DownloadPagesAsync(IEnumerable<string> urls)
{
    var tasks = new List<Task<string>>();
    using var httpClient = new HttpClient();
    foreach (var url in urls)
    {
        tasks.Add(DownloadPageAsync(httpClient, url));
    }
    try
    {
        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while downloading pages asynchronously."); throw;
    }
}

async Task<string> DownloadPageAsync(HttpClient httpClient, string url)
{
    try
    {
        var response = await httpClient.GetAsync(url).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, message: $"Wrong Http Response: {url}");
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"An error occurred while downloading the page: {url}");
        throw;
    }
}
