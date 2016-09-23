using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using Traffix.Common.Dtos;
using Traffix.Common.IocAttributes;
using Traffix.Common.Model;
using Traffix.Common.Providers;
using Traffix.Web.Database.OrmLiteInfrastructure;

namespace Traffix.Web.Database.Repositories
{
    public interface IAuthRepository : IAbstractRepository<AuthClient>
    {
        
    }

    [PerRequest]
    public class AuthRepository : AbstractRepository<AuthClient>, IAuthRepository
    {
        public AuthRepository(IUnitOfWork unitOfWork, IDateResolver dateResolver) : base(unitOfWork, dateResolver)
        {
        }

    }
}