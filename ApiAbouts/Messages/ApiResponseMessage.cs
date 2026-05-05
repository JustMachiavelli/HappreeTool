namespace HappreeTool.ApiAbouts.Messages
{
    /// <summary>
    /// 通用 API 返回模型（强类型）
    /// </summary>
    public class ApiResponseMessage<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public ApiCode Code { get; init; } = ApiCode.成功;

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; init; } = "成功";

        /// <summary>
        /// 返回数据
        /// </summary>
        public T? Data { get; init; }

        private ApiResponseMessage() { }

        private ApiResponseMessage(ApiCode code, string message, T? data = default)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        #region ✅ 成功

        public static ApiResponseMessage<T> Success(T? data = default)
        {
            return new ApiResponseMessage<T>(
                ApiCode.成功,
                "成功",
                data
            );
        }

        #endregion

        #region ❌ 失败

        public static ApiResponseMessage<T> Fail(ApiCode code, string message)
        {
            return new ApiResponseMessage<T>(
                code,
                message,
                default
            );
        }

        public static ApiResponseMessage<T> InternalError(string message = "服务器内部错误")
        {
            return new ApiResponseMessage<T>(
                ApiCode.服务器内部错误,
                message,
                default
            );
        }

        #endregion

        #region 🆕 创建成功

        public static ApiResponseMessage<T> Created(T? data = default, string message = "创建成功")
        {
            return new ApiResponseMessage<T>(
                ApiCode.已成功处理请求,
                message,
                data
            );
        }

        #endregion

        #region 🔍 辅助判断（很实用）

        public bool IsSuccess => Code == ApiCode.成功 || Code == ApiCode.已成功处理请求;

        #endregion
    }
}