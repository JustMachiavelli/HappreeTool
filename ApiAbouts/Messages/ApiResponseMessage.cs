namespace HappreeTool.ApiAbouts.Messages
{
    /// <summary>
    /// 通用 API 返回模型（无数据）
    /// </summary>
    public class ApiResponseMessage
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public ApiCode Code { get; set; } = ApiCode.成功;

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; } = "成功";

        public ApiResponseMessage()
        {
        }

        protected ApiResponseMessage(ApiCode code, string message)
        {
            Code = code;
            Message = message;
        }

        #region Success

        public static ApiResponseMessage Success()
        {
            return new ApiResponseMessage(ApiCode.成功, "成功");
        }

        #endregion

        #region Fail

        public static ApiResponseMessage Fail(ApiCode code, string message)
        {
            return new ApiResponseMessage(code, message);
        }

        #endregion

        #region Created

        public static ApiResponseMessage Created(string message)
        {
            return new ApiResponseMessage(ApiCode.资源抓取中可以稍后重试, message);
        }

        #endregion
    }

    /// <summary>
    /// 通用 API 返回模型（强类型）
    /// </summary>
    public class ApiResponseMessage<T> : ApiResponseMessage
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T? Data { get; set; }

        public ApiResponseMessage()
        {
        }

        private ApiResponseMessage(ApiCode code,
                                   string message,
                                   T? data = default)
            : base(code, message)
        {
            Data = data;
        }

        #region Success

        public static ApiResponseMessage<T> Success(T? data = default)
        {
            return new ApiResponseMessage<T>(ApiCode.成功, "成功", data);
        }

        #endregion
    }
}