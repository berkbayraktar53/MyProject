namespace Core.Utilities.Results
{
    public class Result(bool success) : IResult
    {
        public Result(bool success, string message) : this(success)
        {
            Message = message;
        }

        public bool Success { get; } = success;

        public string Message { get; }
    }
}