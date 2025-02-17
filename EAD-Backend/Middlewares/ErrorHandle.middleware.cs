/*
 * File: Error Handling Middleware
 * Author: Perera V. H. P.
 * Description: This file contains error handling middleware.
 * Created: 07/10/2024
*/

using EAD_Backend.OtherModels;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);  // Pass the request to the next middleware or controller
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            await HandleExceptionAsync(context, ex);  // Handle any unhandled exceptions
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Determine the appropriate status code based on the type of exception
        var statusCode = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            NotSupportedException => StatusCodes.Status405MethodNotAllowed,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            TimeoutException => StatusCodes.Status504GatewayTimeout,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new ApiResponse<object>(
            message: exception.Message,
            data: null
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}

//! Exceptions and their corresponding status codes
// ArgumentException -> 400 
// ArgumentNullException -> 400
// ArgumentOutOfRangeException -> 400
// UnauthorizedAccessException -> 401
// KeyNotFoundException -> 404
// NotSupportedException -> 405
