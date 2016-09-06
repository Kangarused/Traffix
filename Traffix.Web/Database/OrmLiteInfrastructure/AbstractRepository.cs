using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Traffix.Common.Model;
using Traffix.Common.Providers;
using Traffix.Common.Providers.Logging;
using Traffix.Web.Providers;
using ServiceStack.OrmLite;

namespace Traffix.Web.Database.OrmLiteInfrastructure
{
    public interface IAbstractRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> LoadByIdAsync(int id);
        Task<long> InsertAsync(T entity);
        Task<T> SaveAsync(T entity);
        Task UpdateAllAsync(IList<T> entities);
        Task DeleteAsync(Expression<Func<T,bool>> expression);
        Task UpdateAsync(T entity);
        int? TransactionId { get; }
    }
    public class AbstractRepository<T> : IAbstractRepository<T> 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateResolver _dateResolver;

        public DateTime Now
        {
            get
            {
                return _dateResolver.Now();
            }
        }

        public ILoggingProvider Logger
        {
            get; set;
        }


        public AbstractRepository(IUnitOfWork unitOfWork, IDateResolver dateResolver)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            

            _unitOfWork = unitOfWork;
            _dateResolver = dateResolver;
        }
        

        protected  IDbConnection Db
        {
            get { return _unitOfWork.Db; }
        }

        public Task<List<T>> GetAllAsync()
        {
            var q = Db.From<T>();
            return Db.SelectAsync(q);
        }

        public virtual Task<T> GetByIdAsync(int id)
        {
            return Db.SingleByIdAsync<T>(id);
        }
        

        public virtual async Task<T> SaveAsync(T entity)
        {
            await _unitOfWork.Db.SaveAsync(entity);
            return entity;
        }

        public Task<T> LoadByIdAsync(int id)
        {
            return Db.LoadSingleByIdAsync<T>(id);
        }

       

        public virtual async Task<long> InsertAsync(T entity)
        {
            var id = await _unitOfWork.Db.InsertAsync(entity, selectIdentity: true);
            return id;
        }

        public async Task DeleteAsync(Expression<Func<T,bool>> expression)
        {
            List<Task> tasks = new List<Task>();

            tasks.Add(_unitOfWork.Db.DeleteAsync(expression));
            await Task.WhenAll(tasks);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(_unitOfWork.Db.UpdateAsync(entity));

            await Task.WhenAll(tasks);
        }

        public virtual async Task UpdateAllAsync(IList<T> entities)
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(_unitOfWork.Db.UpdateAllAsync(entities));

            await Task.WhenAll(tasks);
        }

        public int? TransactionId => _unitOfWork.TransactionId;
    }
}
