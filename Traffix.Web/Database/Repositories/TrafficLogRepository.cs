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
        Task<List<SortedTrafficLogs>> GetTrafficLogsForMeters(TrafficMetersList list);
        Task<List<TrafficLog>> GetSampleOfTrafficLogs(int meterId);
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

        public async Task<List<SortedTrafficLogs>> GetTrafficLogsForMeters(TrafficMetersList list)
        {
            List<SortedTrafficLogs> request = new List<SortedTrafficLogs>();

            foreach (int id in list.MeterIds)
            {
                var query = Db.From<TrafficLog>().Where(x => x.MeterId == id);
                var result = await Db.LoadSelectAsync(query);
                request.Add(new SortedTrafficLogs
                {
                    MeterId = id,
                    Logs = result
                });
            }

            return request;
        }

        public async Task<List<TrafficLog>> GetSampleOfTrafficLogs(int meterId)
        {
            var query = Db.From<TrafficLog>().Where(x => x.MeterId == meterId);
            query.OrderByDescending(x => x.Id);
            query.Limit(10);

            var result = await Db.LoadSelectAsync(query);
            return result;
        }
    }
}