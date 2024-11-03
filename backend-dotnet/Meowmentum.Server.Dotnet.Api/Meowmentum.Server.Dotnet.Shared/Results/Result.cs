using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Shared.Results;

public class Result<T>
{
    public bool IsSuccess { get; }

    public string ErrorMessage { get; }

    public T Data { get; }

    public Result(bool isSuccess, T data)
    {
        IsSuccess = isSuccess;
        Data = data;
    }

    public Result(bool isSuccess, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }
}

public class Result
{
    public static Result<T> Success<T>(T data) => new(true, data);
    public static Result<T> Success<T>() => new(true);
    public static Result<T> Failure<T>(string message) => new(false, message);
}
