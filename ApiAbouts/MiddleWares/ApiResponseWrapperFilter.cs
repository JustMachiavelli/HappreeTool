using HappreeTool.ApiAbouts.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HappreeTool.ApiAbouts.MiddleWares
{
    /// <summary>
    /// 返回包装器
    /// </summary>
    public class ApiResponseWrapperFilter : IAsyncResultFilter
    {
        /// <summary>
        /// 促使所有返回不是ApiResponseMessage类型的，用ApiResponseMessage包装
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            switch (context.Result)
            {
                case ObjectResult objectResult:
                    {
                        var value = objectResult.Value;

                        // null → Success(null)
                        if (value is null)
                        {
                            objectResult.Value = ApiResponseMessage<object?>.Success();
                            break;
                        }

                        // 已经是 ApiResponseMessage<T>，直接放行
                        if (IsApiResponse(value))
                        {
                            break;
                        }

                        // 包装
                        objectResult.Value = ApiResponseMessage<object?>.Success(value);
                        break;
                    }

                case EmptyResult:
                    {
                        context.Result = new ObjectResult(ApiResponseMessage<object?>.Success());
                        break;
                    }

                case ContentResult contentResult:
                    {
                        context.Result = new ObjectResult(
                            ApiResponseMessage<object?>.Success(contentResult.Content)
                        );
                        break;
                    }

                default:
                    {
                        // 其他类型（很少见）
                        context.Result = new ObjectResult(
                            ApiResponseMessage<object?>.Success(context.Result)
                        );
                        break;
                    }
            }

            await next();
        }

        /// <summary>
        /// 判断是否是 ApiResponseMessage 或 ApiResponseMessage<T>
        /// </summary>
        private static bool IsApiResponse(object value)
        {
            var type = value.GetType();

            if (!type.IsGenericType)
                return type == typeof(ApiResponseMessage<object?>);

            return type.GetGenericTypeDefinition() == typeof(ApiResponseMessage<>);
        }

    }

}
