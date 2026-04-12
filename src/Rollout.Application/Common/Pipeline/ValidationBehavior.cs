using System.Reflection;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Rollout.Application.Common.Pipeline;

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

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Fail(errors);
        }

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

        throw new ValidationException(failures);
    }
}
