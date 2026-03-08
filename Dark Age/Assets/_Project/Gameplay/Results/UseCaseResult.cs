namespace DarkAge.Gameplay.Results
{
    public class UseCaseResult
    {
        protected UseCaseResult(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; }

        public string ErrorMessage { get; }

        public static UseCaseResult Success()
        {
            return new UseCaseResult(true, string.Empty);
        }

        public static UseCaseResult Failure(string errorMessage)
        {
            return new UseCaseResult(false, errorMessage);
        }
    }

    public sealed class UseCaseResult<T> : UseCaseResult
    {
        private UseCaseResult(bool isSuccess, T value, string errorMessage)
            : base(isSuccess, errorMessage)
        {
            Value = value;
        }

        public T Value { get; }

        public static UseCaseResult<T> Success(T value)
        {
            return new UseCaseResult<T>(true, value, string.Empty);
        }

        public static new UseCaseResult<T> Failure(string errorMessage)
        {
            return new UseCaseResult<T>(false, default, errorMessage);
        }
    }
}
