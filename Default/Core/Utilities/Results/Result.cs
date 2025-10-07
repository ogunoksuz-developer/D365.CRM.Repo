

namespace LCW.Core.Utilities.Results
{
 
    public class Result : IResult
    {
       // [JsonConstructor]
        public Result(bool success, string message) : this(success)
        {
            Message = message;
        }

       // [JsonConstructor]
        public Result(bool success)
        {
            Success = success;
        }

        public Result() { }

        public bool Success { get; private set; }

        public string Message { get; private set; }
    }
}
