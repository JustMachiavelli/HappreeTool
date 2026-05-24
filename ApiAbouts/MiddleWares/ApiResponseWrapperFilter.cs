using HappreeTool.ApiAbouts.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HappreeTool.ApiAbouts.MiddleWares;

/// <summary>
/// API 响应包装过滤器
/// </summary>
public class ApiResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next)
    {
        // 跳过特殊响应
        if (ShouldSkip(context.Result))
        {
            await next();
            return;
        }

        // 仅处理 ObjectResult
        if (context.Result is ObjectResult objectResult)
        {
            // ProblemDetails 不处理
            if (objectResult.Value is ProblemDetails)
            {
                await next();
                return;
            }

            // 已经是 ApiResponseMessage
            if (objectResult.Value is ApiResponseMessage)
            {
                await next();
                return;
            }

            // 自动包装
            objectResult.Value =
                ApiResponseMessage<object?>
                    .Success(objectResult.Value);
        }

        await next();
    }

    /// <summary>
    /// 是否跳过包装
    /// </summary>
    private static bool ShouldSkip(IActionResult result)
    {
        return result switch
        {
            // 文件
            FileResult => true,

            // 重定向
            RedirectResult => true,
            RedirectToActionResult => true,
            RedirectToRouteResult => true,
            RedirectToPageResult => true,

            // 登录鉴权
            ChallengeResult => true,
            ForbidResult => true,
            SignInResult => true,
            SignOutResult => true,

            _ => false
        };
    }
}