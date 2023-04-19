namespace Application.Core
{
	public class Result<T>
	{

		public Result() { }
		public Result(T data, string message = null)
		{
			IsSuccess = true;
			Message = message;
			Data = data;
		}
		public Result(string message)
		{
			IsSuccess = false;
			Message = message;
		}

		public bool IsSuccess { get; set; }
		public T? Data { get; set; }
		public List<string> Errors { get; set; }
		public string? Message { get; set; }

		public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Data = value };
		public static Result<T> Failure(string error) => new Result<T> { IsSuccess = false, Errors = new List<string> { error } };
	}
}
