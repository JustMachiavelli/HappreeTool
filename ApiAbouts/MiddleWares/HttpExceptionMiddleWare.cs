using FluentValidation;
using HappreeTool.ApiAbouts.Exceptions;
using HappreeTool.ApiAbouts.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HappreeTool.ApiAbouts.MiddleWares
{
    public class HttpExceptionMiddleWare(ILogger<HttpExceptionMiddleWare> logger) : IMiddleware
    {
        private readonly ILogger<HttpExceptionMiddleWare> _logger = logger;

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

                var response = ApiResponseMessage<object?>.Fail(code, message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsJsonAsync(response);
            }
        }

        private static (ApiCode code, string message, int status) MapException(Exception ex)
        {
            return ex switch
            {
                BadRequestExcepiton => (
                    ApiCode.资源已存在,
                    ex.Message,
                    StatusCodes.Status400BadRequest
                ),

                ValidationException ve => (
                    ApiCode.请求参数非法,
                    string.Join(";", ve.Errors.Select(e => e.ErrorMessage)),
                    StatusCodes.Status400BadRequest
                ),

                _ => (
                    ApiCode.服务器内部错误,
                    "服务器内部错误",
                    StatusCodes.Status500InternalServerError
                )
            };
        }
    }
}