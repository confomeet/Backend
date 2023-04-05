using System.Net;
using System.Text.Json;

namespace VideoProjectCore6
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            ValidatorException validatorException = new ValidatorException();
            string result;
            if (ex is ValidatorException)
            {
                code = HttpStatusCode.BadRequest;
                validatorException = (ValidatorException)ex;
                result = JsonSerializer.Serialize(new { error = validatorException.AttributeMessages });
            }
            else
            {
                code = HttpStatusCode.InternalServerError;
                result = JsonSerializer.Serialize(new { error = ex.Message });
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
