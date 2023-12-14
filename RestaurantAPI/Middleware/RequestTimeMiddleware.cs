using System.Diagnostics;

namespace RestaurantAPI.Middleware
{
	public class RequestTimeMiddleware : IMiddleware
	{
		private ILogger<RequestTimeMiddleware> _logger;

		public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			await next.Invoke(context);
			stopwatch.Stop();

			var ellapsedMiliseconds = stopwatch.ElapsedMilliseconds;
			_logger.LogInformation($"request method {context.Request.Method} at {context.Request.Path} took {ellapsedMiliseconds} miliseconds");
		}
	}
}
