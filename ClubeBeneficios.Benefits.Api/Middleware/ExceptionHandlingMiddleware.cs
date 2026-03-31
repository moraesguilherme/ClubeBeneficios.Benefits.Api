using System.Net;
using System.Text.Json;
using FluentValidation;
using ClubeBeneficios.Benefits.Domain.Dtos;
using ClubeBeneficios.Benefits.Domain.Exceptions;

namespace ClubeBeneficios.Benefits.Api.Middleware;

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
        catch (ValidationException ex)
        {
            await WriteValidationErrorAsync(context, ex);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(context, (int)HttpStatusCode.NotFound, ex.Code, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            await WriteErrorAsync(context, (int)HttpStatusCode.Forbidden, ex.Code, ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status422UnprocessableEntity, ex.Code, ex.Message);
        }
        catch (DomainException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, ex.Code, ex.Message);
        }
        catch (Exception ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "unexpected_error", ex.Message);
        }
    }

    private static async Task WriteValidationErrorAsync(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var errors = ex.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).Distinct().ToArray());

        var response = new ApiErrorResponseDto
        {
            Code = "validation_error",
            Message = "Ocorreram erros de validaÃ§Ã£o na requisiÃ§Ã£o.",
            Status = StatusCodes.Status400BadRequest,
            TraceId = context.TraceIdentifier,
            Errors = errors
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string code, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponseDto
        {
            Code = code,
            Message = message,
            Status = statusCode,
            TraceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}