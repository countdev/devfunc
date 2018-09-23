using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WhyNotRun.BO;
using WhyNotRun.Models;

namespace WhyNotRun.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class WhyNotRunJwtAuthAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var hashToken = UtilBO.ValorAuthorizationHeader(System.Web.HttpContext.Current);
            // Valida se o usuário está logado.
            if (string.IsNullOrEmpty(hashToken))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    message = "Você não está logado."
                });
                return;
            }
            else
            {
                // Se estiver, busca o token que ele enviou no Authorization Header
                Token _token = new Token();
                _token.DecodeToken(hashToken);
            }
        }


    }
}