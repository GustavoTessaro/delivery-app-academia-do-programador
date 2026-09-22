using System.Diagnostics;
using DeliveryApp.Aplicacao.Compartilhado;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DeliveryApp.WebApi.Compartilhado.Http;

public static class ResultExtensions
{
    public static ActionResult ProblemDetails(this ControllerBase controller, ResultBase result)
    {
        var tipoErro = (TipoErro)result.Errors.First().Metadata[nameof(TipoErro)];
        var mensagemErro = result.Errors.First().Message;

        if (tipoErro.Equals(TipoErro.NaoEncontrado))
        {
            return CriarProblem(
                controller,
                StatusCodes.Status404NotFound,
                mensagemErro,
                "Recurso Não Encontrado",
                ProblemDetailsTypes.NotFound
            );
        }

        if (tipoErro.Equals(TipoErro.Conflito))
        {
            return CriarProblem(
                controller,
                StatusCodes.Status409Conflict,
                mensagemErro,
                "Conflito",
                ProblemDetailsTypes.Conflict
            );
        }

        if (tipoErro.Equals(TipoErro.Validacao))
        {
            var modelState = new ModelStateDictionary();

            foreach (var erro in result.Errors)
            {
                var campo = erro.Metadata["Campo"].ToString()!;

                modelState.AddModelError(campo, erro.Message);
            }

            ValidationProblemDetails problemDetails = new(modelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Requisição Inválida",
                Type = ProblemDetailsTypes.BadRequest
            };

            AdicionarTraceId(problemDetails, controller);

            return controller.ValidationProblem(problemDetails);
        }

        return CriarProblem(
            controller,
            StatusCodes.Status500InternalServerError,
            null,
            "Erro do Interno do Servidor",
            ProblemDetailsTypes.InternalServerError
        );
    }

    private static ObjectResult CriarProblem(
        ControllerBase controller,
        int statusCode,
        string? detail,
        string title,
        string type
    )
    {
        ProblemDetails problemDetails = new()
        {
            Status = statusCode,
            Detail = detail,
            Title = title,
            Type = type
        };

        AdicionarTraceId(problemDetails, controller);

        return controller.StatusCode(statusCode, problemDetails);
    }

    private static void AdicionarTraceId(ProblemDetails problemDetails, ControllerBase controller)
    {
        problemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;
    }
}
