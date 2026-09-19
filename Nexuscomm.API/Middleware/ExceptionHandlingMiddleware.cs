using System.Net;
using System.Text.Json;
using Nexuscomm.API.DTOs;
using Nexuscomm.API.Exceptions;

namespace Nexuscomm.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;
            List<string>? errors = null;

            switch (exception)
            {
                case ValidationException validationEx:
                    statusCode = validationEx.StatusCode;
                    message = "Validation failed.";
                    errors = validationEx.Errors;
                    break;

                case NotFoundException notFoundEx:
                    statusCode = notFoundEx.StatusCode;
                    message = notFoundEx.Message;
                    break;

                case ForbiddenException forbiddenEx:
                    statusCode = forbiddenEx.StatusCode;
                    message = forbiddenEx.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = "You are not authorized to perform this action.";
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred. Please try again later.";
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = ApiResponseDto<object>.ErrorResponse(message, errors);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}