﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using ServiceStack.OrmLite;
using Traffix.Common.Dtos;
using Traffix.Common.IocAttributes;
using Traffix.Common.Model;
using Traffix.Common.Providers;
using Traffix.Web.Database.OrmLiteInfrastructure;

namespace Traffix.Web.Database.Repositories
{
    public interface ITrafficMeterRepository : IAbstractRepository<TrafficMeter>
    {
        Task<List<TrafficMeter>> GetAllOrderByLinkId();
        Task<List<TrafficMeter>> GetMetersForRegion(string region);
    }

    [PerRequest]
    public class TrafficMeterRepository : AbstractRepository<TrafficMeter>, ITrafficMeterRepository
    {
        public TrafficMeterRepository(IUnitOfWork unitOfWork, IDateResolver dateResolver) : base(unitOfWork, dateResolver)
        {
        }

        public async Task<List<TrafficMeter>> GetAllOrderByLinkId()
        {
            var results = await GetAllAsync();
            return results.OrderBy(x => x.LinkId).ToList();
        }

        public async Task<List<TrafficMeter>> GetMetersForRegion(string region)
        {
            var query = Db.From<TrafficMeter>().Where(x => x.Region == region);
            var results = await Db.LoadSelectAsync(query);
            return results;
        }
    }
}