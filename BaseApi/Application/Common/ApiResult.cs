namespace BaseApi.Application.Common
{
    /// <summary>
    /// Base API response for operations without data return
    /// </summary>
    public class ApiResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Creates a successful result
        /// </summary>
        public static ApiResult SuccessResult(string message = "Operation completed successfully")
        {
            return new ApiResult
            {
                Success = true,
                Message = message
            };
        }

        /// <summary>
        /// Creates a failed result with single error
        /// </summary>
        public static ApiResult FailureResult(string error)
        {
            return new ApiResult
            {
                Success = false,
                Message = "Operation failed",
                Errors = new List<string> { error }
            };
        }

        /// <summary>
        /// Creates a failed result with multiple errors
        /// </summary>
        public static ApiResult FailureResult(List<string> errors)
        {
            return new ApiResult
            {
                Success = false,
                Message = "Operation failed",
                Errors = errors
            };
        }

        /// <summary>
        /// Creates a failed result with message and errors
        /// </summary>
        public static ApiResult FailureResult(string message, List<string> errors)
        {
            return new ApiResult
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// Generic API response for operations with data return
    /// </summary>
    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }

        /// <summary>
        /// Creates a successful result with data
        /// </summary>
        public static ApiResult<T> SuccessResult(T data, string message = "Operation completed successfully")
        {
            return new ApiResult<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed result with single error
        /// </summary>
        public static new ApiResult<T> FailureResult(string error)
        {
            return new ApiResult<T>
            {
                Success = false,
                Message = "Operation failed",
                Errors = new List<string> { error }
            };
        }

        /// <summary>
        /// Creates a failed result with multiple errors
        /// </summary>
        public static new ApiResult<T> FailureResult(List<string> errors)
        {
            return new ApiResult<T>
            {
                Success = false,
                Message = "Operation failed",
                Errors = errors
            };
        }

        /// <summary>
        /// Creates a failed result with message and errors
        /// </summary>
        public static new ApiResult<T> FailureResult(string message, List<string> errors)
        {
            return new ApiResult<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }

        /// <summary>
        /// Creates a not found result
        /// </summary>
        public static ApiResult<T> NotFoundResult(string message = "Resource not found")
        {
            return new ApiResult<T>
            {
                Success = false,
                Message = message,
                Errors = new List<string> { message }
            };
        }
    }
}