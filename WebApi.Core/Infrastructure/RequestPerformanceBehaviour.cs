using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using raBudget.Core.Interfaces;

namespace raBudget.Core.Infrastructure
{
    public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly IAuthenticationProvider _authenticationProvider;

        public RequestPerformanceBehaviour(ILogger<TRequest> logger, IAuthenticationProvider authenticationProvider)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var user =  _authenticationProvider.IsAuthenticated 
                            ?_authenticationProvider.User.UserId.ToString() 
                            : "<not signed in>";
            var requestName = typeof(TRequest).Name;
            if (_timer.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning("Request {Name} by {@User} took {ElapsedMilliseconds} ms {@Request}", requestName, user, _timer.ElapsedMilliseconds, request);
            }
            else
            {
                _logger.LogDebug("Request {Name} by {@User} took {ElapsedMilliseconds} ms {@Request}", requestName, user, _timer.ElapsedMilliseconds, request);
            }


            return response;
        }
    }
}
