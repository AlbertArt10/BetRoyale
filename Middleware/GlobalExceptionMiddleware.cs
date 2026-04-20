using System.Text.Json;
using BetRoyale.API.DTOs.Common;
using BetRoyale.API.Services.Exceptions;

namespace BetRoyale.API.Middleware;

public class GlobalExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}.", context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception while processing {Method} {Path}.", context.Request.Method, context.Request.Path);
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new ErrorResponseDto
        {
            Message = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private static (int StatusCode, string Message) MapException(Exception exception)
    {
        return exception switch
        {
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, exception.Message),
            PredictionUserNotFoundException => (StatusCodes.Status401Unauthorized, exception.Message),
            SubscriptionUserNotFoundException => (StatusCodes.Status401Unauthorized, exception.Message),
            ArticleLikeUserNotFoundException => (StatusCodes.Status401Unauthorized, exception.Message),
            ArticleAuthorNotFoundException => (StatusCodes.Status401Unauthorized, exception.Message),
            CommentAuthorNotFoundException => (StatusCodes.Status401Unauthorized, exception.Message),

            DuplicateUsernameException => (StatusCodes.Status409Conflict, exception.Message),
            DuplicateEmailException => (StatusCodes.Status409Conflict, exception.Message),

            ArticleUpdateForbiddenException => (StatusCodes.Status403Forbidden, exception.Message),
            CommentUpdateForbiddenException => (StatusCodes.Status403Forbidden, exception.Message),

            ArticleNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            ArticleLikeNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            CommentNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            MatchNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            PredictionNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            ProfileNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            SubscriptionNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            UserNotFoundException => (StatusCodes.Status404NotFound, exception.Message),

            InvalidArticleException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidArticleLikeException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidCommentException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidEmailException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidMatchException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidPasswordException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidPredictionException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidProfileException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidSubscriptionException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidUserRoleChangeException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidUsernameException => (StatusCodes.Status400BadRequest, exception.Message),

            RoleNotFoundException => (StatusCodes.Status500InternalServerError, exception.Message),

            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };
    }
}
