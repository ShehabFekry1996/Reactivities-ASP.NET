using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Core
{
    public class Result<T>
    {
        public T? Value { get; set; }

        public bool IsSuccess { get; set; }

        public string? Error { get; set; }

        public int Code { get; set; }

        public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };

        public static Result<T> Failure(string error, int code) => new()
        {
            IsSuccess = false,
            Error = error,
            Code = code
        };
    }
}
