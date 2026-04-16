using System.Reflection;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Rollout.Application.Common.Pipeline;

/// <summary>
/// A MediatR pipeline behavior that enforces FluentValidation rules across all requests.
/// This implementation integrates with FluentResults to provide a unified error handling experience
/// without relying on exceptions for flow control.
/// </summary>
/// <typeparam name="TRequest">The type of the request being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response, expected to be a Result or Result&lt;T&gt;.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .ToList();

        if (!failures.Any())
        {
            return await next();
        }

        var errors = failures.Select(x => new Error(x.ErrorMessage)).ToArray();

        // The following logic uses reflection to instantiate the appropriate FluentResults failure object.
        // This is done to avoid hard-coding every specific Result<T> type while maintaining a strongly-typed pipeline.

        // Case 1: Response is a non-generic Result
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Fail(errors);
        }

        // Case 2: Response is a generic Result<T>
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var failMethod = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(method => method.Name == "Fail"
                                 && method.IsGenericMethodDefinition
                                 && method.GetParameters().Length == 1
                                 && method.GetParameters()[0].ParameterType == typeof(Error[]));

            var genericFail = failMethod.MakeGenericMethod(resultType);
            return (TResponse)genericFail.Invoke(null, new object[] { errors })!;
        }

        // Fallback: If the response is not a Result type, we revert to throwing an exception as a safe default.
        throw new ValidationException(failures);
    }
}

