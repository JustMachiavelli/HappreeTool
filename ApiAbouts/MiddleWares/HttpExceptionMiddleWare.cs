using FluentValidation;
using HappreeTool.ApiAbouts.Exceptions;
using HappreeTool.ApiAbouts.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HappreeTool.ApiAbouts.MiddleWares;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class HttpExceptionMiddleWare(ILogger<HttpExceptionMiddleWare> _logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            var (code, message, statusCode) = MapException(ex);

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            ApiResponseMessage response = ApiResponseMessage.Fail(code, message);
            await context.Response.WriteAsJsonAsync(response);
        }
    }

    /// <summary>
    /// 异常映射
    /// </summary>
    private static (ApiCode code, string message, int statusCode) MapException(Exception ex)
    {
        return ex switch
        {
            // FluentValidation 参数校验失败
            ValidationException validationException => (
                ApiCode.请求参数格式错误,
                string.Join("; ", validationException.Errors.Select(e => e.ErrorMessage)),
                StatusCodes.Status400BadRequest
            ),

            // 业务异常
            BadRequestException => (
                ApiCode.请求参数不合业务,
                ex.Message,
                StatusCodes.Status400BadRequest
            ),

            // 未知异常
            _ => (
                ApiCode.服务器内部错误,
                "服务器内部错误",
                StatusCodes.Status500InternalServerError
            )
        };
    }
}