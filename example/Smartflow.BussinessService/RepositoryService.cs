using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using System.Linq.Expressions;

namespace Smartflow.BussinessService
{
    public class RepositoryService<T> : IRepository<T> where T : class
    {
        protected IDbConnection Connection
        {
            get { return DblHelper.CreateConnection(); }
        }

        public void Insert(T entity)
        {
            Connection.Insert<T>(entity);
        }

        public void Update(T entity)
        {
            Connection.Update<T>(entity);
        }

        public void Delete(T entity)
        {
            Connection.Delete<T>(entity);
        }

        public void Delete(long Id)
        {
            var entity = Get(Id);
            if (entity == null) return;

            Delete(entity);
        }

        public void Delete(Expression<Func<T, bool>> expression)
        {
            Connection.Delete<T>(ExpressionUtils.ParseToWhere(expression.Body));
        }

        public T Get(long id)
        {
            return Connection.Get<T>(id);
        }

        public T Get(Expression<Func<T, bool>> expression)
        {
            return Connection.GetAll<T>(ExpressionUtils.ParseToWhere(expression.Body))
                .FirstOrDefault();
        }

        public List<T> Query()
        {
            return Connection.GetAll<T>();
        }

        public List<T> Query(Expression<Func<T, bool>> expression)
        {
            return Connection.GetAll<T>(ExpressionUtils.ParseToWhere(expression.Body));
        }
    }
}
