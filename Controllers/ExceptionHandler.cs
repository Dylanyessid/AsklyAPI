using Microsoft.AspNetCore.Diagnostics;

namespace AcaHelpAPI.Controllers
{
    public class ExceptionHandler: IExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(
            ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error no controlado");

            httpContext.Response.StatusCode =
                StatusCodes.Status500InternalServerError;
            var payload = ApiResponse<object>.ErrorResponse("Error de la app", "INTERNAL_ERROR");
            await httpContext.Response.WriteAsJsonAsync(
                payload,
                cancellationToken);

            return true;
        }
    }
}
