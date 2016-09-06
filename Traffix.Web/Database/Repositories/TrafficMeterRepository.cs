using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Traffix.Common.IocAttributes;
using Traffix.Common.Model;
using Traffix.Common.Providers;
using Traffix.Web.Database.OrmLiteInfrastructure;

namespace Traffix.Web.Database.Repositories
{
    public interface ITrafficMeterRepository : IAbstractRepository<TrafficMeter>
    {
        
    }

    [PerRequest]
    public class TrafficMeterRepository : AbstractRepository<TrafficMeter>, ITrafficMeterRepository
    {
        public TrafficMeterRepository(IUnitOfWork unitOfWork, IDateResolver dateResolver) : base(unitOfWork, dateResolver)
        {
        }
    }
}