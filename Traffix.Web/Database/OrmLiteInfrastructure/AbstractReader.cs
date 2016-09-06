using System;
using System.Data;

namespace Traffix.Web.Database.OrmLiteInfrastructure
{
    public abstract class AbstractReader
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public AbstractReader(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;

        }

        protected IDbConnection Db
        {
            get { return _unitOfWork.Db; }
        }
    }
}
