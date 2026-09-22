using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.WebApi.Compartilhado.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DeliveryApp.WebApi.Compartilhado;

public static class ControllerExtensions
{
    public static ActionResult ErroDeValidacao(
        this ControllerBase controller,
        IEnumerable<ErroValidacao> erros
    )
    {
        ModelStateDictionary modelState = new();

        foreach (var erro in erros)
            modelState.AddModelError(erro.Campo, erro.Mensagem);

        ValidationProblemDetails problemDetails = new(modelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Requisição Inválida",
            Type = ProblemDetailsTypes.BadRequest
        };

        return controller.ValidationProblem(problemDetails);
    }

    public static ActionResult Conflito(
        this ControllerBase controller,
        string mensagem
    )
    {
        return controller.Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Conflito",
            detail: mensagem,
            type: ProblemDetailsTypes.Conflict
        );
    }

    public static ActionResult CredenciaisInvalidas(
        this ControllerBase controller
    )
    {
        return controller.Problem(
            statusCode: StatusCodes.Status401Unauthorized,
            title: "Não Autenticado",
            detail: "Email ou senha inválidos.",
            type: ProblemDetailsTypes.Unauthorized
        );
    }

    public static ActionResult ErrosDeCriacaoUsuario(
        this ControllerBase controller,
        IdentityResult resultado
    )
    {
        if (resultado.Errors.Any(erro =>
            erro.Code is "DuplicateEmail" or "DuplicateUserName"
        ))
        {
            return controller.Conflito("Já existe um usuário cadastrado com este email.");
        }

        return controller.ErroDeValidacao(resultado.Errors.Select(erro =>
        {
            string campo = erro.Code.StartsWith("Password", StringComparison.Ordinal)
                ? "Senha"
                : "Email";

            return new ErroValidacao(campo, erro.Description);
        }));
    }
}
