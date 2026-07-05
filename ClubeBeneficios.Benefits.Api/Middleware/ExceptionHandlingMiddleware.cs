using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace ClubeBeneficios.Benefits.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            await WriteValidationProblemAsync(context, exception);
        }
        catch (SqlException exception)
        {
            await WriteSqlProblemAsync(context, exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            await WriteProblemAsync(
                context,
                HttpStatusCode.Forbidden,
                "Acesso negado.",
                exception.Message,
                exception);
        }
        catch (KeyNotFoundException exception)
        {
            await WriteProblemAsync(
                context,
                HttpStatusCode.NotFound,
                "Recurso não encontrado.",
                exception.Message,
                exception);
        }
        catch (ArgumentException exception)
        {
            await WriteProblemAsync(
                context,
                HttpStatusCode.BadRequest,
                "Requisição inválida.",
                exception.Message,
                exception);
        }
        catch (InvalidOperationException exception)
        {
            await WriteProblemAsync(
                context,
                HttpStatusCode.BadRequest,
                "Operação não permitida.",
                exception.Message,
                exception);
        }
        catch (Exception exception)
        {
            await WriteProblemAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Erro interno.",
                "Ocorreu um erro inesperado ao processar a requisição.",
                exception);
        }
    }

    private async Task WriteValidationProblemAsync(
        HttpContext context,
        ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                x => x.Key,
                x => x.Select(error => error.ErrorMessage).ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Erro de validação.",
            Detail = "Um ou mais campos enviados são inválidos.",
            Instance = context.Request.Path
        };

        await WriteResponseAsync(context, StatusCodes.Status400BadRequest, problem);
    }

    private async Task WriteSqlProblemAsync(
    HttpContext context,
    SqlException exception)
    {
        var statusCode = exception.Number switch
        {
            2601 => StatusCodes.Status409Conflict,
            2627 => StatusCodes.Status409Conflict,
            547 => StatusCodes.Status400BadRequest,
            50000 => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var title = exception.Number switch
        {
            2601 => "Registro duplicado.",
            2627 => "Registro duplicado.",
            547 => "Regra de banco violada.",
            50000 => "Regra de negócio.",
            _ => "Erro de banco de dados."
        };

        var detail = exception.Number switch
        {
            2601 => "Já existe um registro com os dados informados.",
            2627 => "Já existe um registro com os dados informados.",
            547 => "A operação não pôde ser concluída por violar uma regra de relacionamento ou validação do banco.",
            50000 => exception.Message,
            _ => "Ocorreu um erro ao acessar o banco de dados."
        };

        await WriteProblemAsync(
            context,
            (HttpStatusCode)statusCode,
            title,
            detail,
            exception);
    }

    private async Task WriteProblemAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string title,
        string detail,
        Exception exception)
    {
        if ((int)statusCode >= 500)
        {
            _logger.LogError(exception, "{Title}: {Message}", title, exception.Message);
        }
        else
        {
            _logger.LogWarning(exception, "{Title}: {Message}", title, exception.Message);
        }

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = _environment.IsDevelopment()
                ? exception.Message
                : detail,
            Instance = context.Request.Path
        };

        if (_environment.IsDevelopment())
        {
            problem.Extensions["exceptionType"] = exception.GetType().Name;
            problem.Extensions["traceId"] = context.TraceIdentifier;
        }

        await WriteResponseAsync(context, (int)statusCode, problem);
    }

    private static async Task WriteResponseAsync(
        HttpContext context,
        int statusCode,
        ProblemDetails problem)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(
            problem,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        await context.Response.WriteAsync(json);
    }
}