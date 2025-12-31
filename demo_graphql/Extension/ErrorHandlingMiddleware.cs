using System.Net;
using System.Text.Json;
using demo_graphql.Models;

namespace demo_graphql.Extension
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBody = context.Response.Body;
            try
            {
                // await _next(context);
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                await _next(context); // Execute pipeline

                memoryStream.Position = 0;
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                // ✅ Global response inspection (GraphQL or REST)
                if (!string.IsNullOrWhiteSpace(responseBody)
                    && context.Response.ContentType?.Contains("application/json") == true)
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(responseBody);

                        if (doc.RootElement.TryGetProperty("responseMessages", out var messages))
                        {
                            foreach (var msg in messages.EnumerateArray())
                            {
                                if (msg.TryGetProperty("statusCode", out var statusProp))
                                {
                                    var statusCode = statusProp.GetInt32();

                                    context.Response.StatusCode = statusCode switch
                                    {
                                        401 => StatusCodes.Status401Unauthorized,
                                        403 => StatusCodes.Status403Forbidden,
                                        >= 400 and < 500 => StatusCodes.Status400BadRequest,
                                        >= 500 => StatusCodes.Status500InternalServerError,
                                        _ => context.Response.StatusCode
                                    };

                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Response parsing failed");
                    }
                }

                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught by middleware");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new Response();
                response.responseMessages.Add(new ResponseMessage() { message = ex.Message, type = "E" });

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}