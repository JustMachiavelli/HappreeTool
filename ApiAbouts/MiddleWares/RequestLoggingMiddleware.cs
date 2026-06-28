using System.Diagnostics;
using System.Text;
using System.Text.Json;
using HappreeTool.ApiAbouts.Formats;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HappreeTool.ApiAbouts.MiddleWares;

/// <summary>
/// HTTP 请求日志中间件
/// </summary>
public class HttpRequestLoggingMiddleWare(
    ILogger<HttpRequestLoggingMiddleWare> logger)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        string requestMessage = await BuildRequestMessageAsync(context);

        logger.LogInformation(requestMessage);

        Stream originalResponseBody = context.Response.Body;

        await using MemoryStream responseBody = new();

        context.Response.Body = responseBody;

        try
        {
            await next(context);

            stopwatch.Stop();

            string responseMessage = await BuildResponseMessageAsync(context, stopwatch.ElapsedMilliseconds);

            logger.LogInformation(responseMessage);

            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalResponseBody);
        }
        finally
        {
            context.Response.Body = originalResponseBody;
        }
    }

    /// <summary>
    /// 构建请求日志
    /// </summary>
    private static async Task<string> BuildRequestMessageAsync(HttpContext context)
    {
        StringBuilder builder = new();

        builder.AppendLine("HTTP Request");
        builder.AppendLine($"{context.Request.Method} {context.Request.Path}");

        if (context.Request.Query.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("Query:");

            foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> item in context.Request.Query)
            {
                builder.AppendLine($"  {item.Key} = {item.Value}");
            }
        }

        if (IsJsonRequest(context))
        {
            string body = await ReadRequestBodyAsync(context);

            if (!string.IsNullOrWhiteSpace(body))
            {
                builder.AppendLine();
                builder.AppendLine("Body:");
                builder.AppendLine(FormatJson(body));
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// 构建响应日志
    /// </summary>
    private static async Task<string> BuildResponseMessageAsync(HttpContext context, long elapsedMilliseconds)
    {
        StringBuilder builder = new();

        builder.AppendLine($"HTTP Response ({elapsedMilliseconds} ms)");
        builder.AppendLine($"StatusCode: {context.Response.StatusCode}");

        if (IsJsonResponse(context))
        {
            string body = await ReadResponseBodyAsync(context);

            if (!string.IsNullOrWhiteSpace(body))
            {
                builder.AppendLine();
                builder.AppendLine("Body:");
                builder.AppendLine(FormatJson(body));
            }
        }
        else
        {
            builder.AppendLine($"ContentType: {context.Response.ContentType}");
        }

        return builder.ToString();
    }

    /// <summary>
    /// 读取请求 Body
    /// </summary>
    private static async Task<string> ReadRequestBodyAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        context.Request.Body.Position = 0;

        using StreamReader reader = new(
            context.Request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        string body = await reader.ReadToEndAsync();

        context.Request.Body.Position = 0;

        return body;
    }

    /// <summary>
    /// 读取响应 Body
    /// </summary>
    private static async Task<string> ReadResponseBodyAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;

        using StreamReader reader = new(
            context.Response.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        string body = await reader.ReadToEndAsync();

        context.Response.Body.Position = 0;

        return body;
    }

    /// <summary>
    /// 是否为 Json 请求
    /// </summary>
    private static bool IsJsonRequest(HttpContext context)
    {
        return context.Request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true;
    }

    /// <summary>
    /// 是否为 Json 响应
    /// </summary>
    private static bool IsJsonResponse(HttpContext context)
    {
        return context.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true;
    }

    /// <summary>
    /// 格式化 Json
    /// </summary>
    private static string FormatJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return json;

        try
        {
            JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            return JsonSerializer.Serialize(jsonElement, JsonApiFormat.JSON_SERIALIZER_OPTIONS);
        }
        catch
        {
            // 不是合法 Json，直接返回原内容
            return json;
        }
    }
}