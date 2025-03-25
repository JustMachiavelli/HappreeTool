namespace HappreeTool.Objects
{
    public class Result
    {
        public ResultState State { get; set; } = ResultState.Success;
        public string Message { get; set; } = "成功";
        public Exception? Exception { get; set; }
        public object? Value { get; set; }

        public static Result Success(object? value = null)
        {
            return new Result
            {
                State = ResultState.Success,
                Value = value
            };
        }

        public static Result Fail(Exception exception)
        {
            return new Result
            {
                State = ResultState.Fail,
                Exception = exception
            };
        }

        public static Result Fail(string message)
        {
            return new Result
            {
                State = ResultState.Fail,
                Message = message
            };
        }

        public bool IsSuccess()
        {
            return State == ResultState.Success;
        }
    }
    public enum ResultState
    {
        Fail = 0,
        Success = 1
    }
}
