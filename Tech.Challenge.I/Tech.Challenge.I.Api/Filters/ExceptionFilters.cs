using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Tech.Challenge.I.Communication.Response;
using Tech.Challenge.I.Exceptions;
using Tech.Challenge.I.Exceptions.ExceptionBase;

namespace Tech.Challenge.I.Api.Filters;

public class ExceptionFilters : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is TechChallengeException)
        {
            ProcessTechChallengeException(context);
        }
        else
        {
            LancarErroDesconhecido(context);
        }
    }

    private static void ProcessTechChallengeException(ExceptionContext context)
    {
        if (context.Exception is ValidationErrorsException)
        {
            TratarErrosDeValidacaoException(context);
        }
        else if (context.Exception is InvalidLoginException)
        {
            TratarLoginException(context);
        };
    }

    private static void TratarErrosDeValidacaoException(ExceptionContext context)
    {
        var validationErrorException = context.Exception as ValidationErrorsException;

        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Result = new ObjectResult(new ErrorResponseJson(validationErrorException.ErrorMessages));
    }

    private static void TratarLoginException(ExceptionContext context)
    {
        var loginError = context.Exception as InvalidLoginException;

        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Result = new ObjectResult(new ErrorResponseJson(loginError.Message));
    }

    private static void LancarErroDesconhecido(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Result = new ObjectResult(new ErrorResponseJson(ErrorsMessages.UnknowError));
    }
}
