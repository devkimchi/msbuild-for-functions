using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionAppWithMSBuild;

public class HttpExampleTrigger
{
    private readonly ILogger<HttpExampleTrigger> _logger;

    public HttpExampleTrigger(ILogger<HttpExampleTrigger> logger)
    {
        _logger = logger;
    }

    [Function("HttpExampleTrigger")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions with MSBuild!");
    }
}
