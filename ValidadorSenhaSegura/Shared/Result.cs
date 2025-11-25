namespace ValidadorSenhaSegura.Shared
{

    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public List<Error> Errors { get; }

        protected Result(bool isSuccess, List<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors ?? [];
        }

        public static Result Success() => new(true, new());

        public static Result Failure(params Error[] errors)
            => new(false, errors.ToList());

        public static Result Failure(List<Error> errors)
            => new(false, errors);

        public static Result<T> Success<T>(T value)
            => new(value, true, []);

        public static Result<T> Failure<T>(params Error[] errors)
            => new(default!, false, errors.ToList());

        public static Result<T> Failure<T>(List<Error> errors)
            => new(default!, false, errors);

        public static Result<T> Failure<T>(T value, List<Error> errors)
            => new(value, false, errors);


        public static Result<T> Failure<T>(T value)
            => new(value, false, []);
    }

    public class Result<T> : Result
    {
        public T Data { get; }

        internal Result(T data, bool isSuccess, List<Error> errors)
            : base(isSuccess, errors)
        {
            Data = data;
        }
    }
}
