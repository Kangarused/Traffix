using System;
using System.Collections.Generic;
using System.Linq;
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
    public interface ITrafficLogRepoistory : IAbstractRepository<TrafficLog>
    {
        Task<List<TrafficLog>> GetTrafficLogsForMeter(int meterId);
        Task<List<TrafficLogsRequest>> GetTrafficLogsForMeters(TrafficMetersList list);
    }

    [PerRequest]
    public class TrafficLogRepository : AbstractRepository<TrafficLog>, ITrafficLogRepoistory
    {
        public TrafficLogRepository(IUnitOfWork unitOfWork, IDateResolver dateResolver) : base(unitOfWork, dateResolver)
        {
        }

        public Task<List<TrafficLog>> GetTrafficLogsForMeter(int meterId)
        {
            var query = Db.From<TrafficLog>().Where(x => x.MeterId == meterId);
            var result = Db.LoadSelectAsync(query);
            return result;
        }

        public async Task<List<TrafficLogsRequest>> GetTrafficLogsForMeters(TrafficMetersList list)
        {
            List<TrafficLogsRequest> request = new List<TrafficLogsRequest>();

            foreach (int id in list.MeterIds)
            {
                var query = Db.From<TrafficLog>().Where(x => x.MeterId == id);
                var result = await Db.LoadSelectAsync(query);
                request.Add(new TrafficLogsRequest
                {
                    MeterId = id,
                    Logs = result
                });
            }

            return request;
        }
    }
}