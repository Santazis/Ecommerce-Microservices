namespace SharedKernel.Common;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new ArgumentException("Error can not be set when success is true");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new ArgumentException("Error can not be empty when success is false");       
        }
        IsSuccess = isSuccess;
        Error = error;
    }
    public bool IsSuccess { get; }
    public bool IsFailure=> !IsSuccess;
    public Error Error { get; }
    
    public static Result Success => new(true,Error.None);
    public static Result Failure(Error error)=> new(false,error);
}

public class Result<T> : Result
{
    public readonly T? _value;

    public T Value =>
        IsSuccess ? _value! : throw new ArgumentException("Value can not be accessed when success is false");

    public Result(T value) : base(true, Error.None){_value = value;}
    public Result(Error error) : base(false, error)
    {
        _value = default;
    }
    
    public new static Result<T> Success(T value)=> new(value);
    public new static Result<T> Failure(Error error)=> new(error);
}