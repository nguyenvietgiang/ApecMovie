using MediatR;

namespace CQRS_Core
{
    public interface IQueryHandler<in TQuery, TResponse>
     : IRequestHandler<TQuery, TResponse>
     where TQuery : IQuery<TResponse>
     where TResponse : notnull
    {
    }
}
