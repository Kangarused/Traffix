using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Traffix.Web.Database.OrmLiteInfrastructure;

namespace Traffix.Web.WebApiFilters
{
    public class TransactionFilterAttribute : ActionFilterAttribute, IAutofacActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionFilterAttribute(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _unitOfWork.StartTransaction();
            base.OnActionExecuting(actionContext);
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            _unitOfWork.StartTransaction();
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Exception==null)
                _unitOfWork.Commit();
            base.OnActionExecuted(context);
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext.Exception == null)
                _unitOfWork.Commit();
            
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}