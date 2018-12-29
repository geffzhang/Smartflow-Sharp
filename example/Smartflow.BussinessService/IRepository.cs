using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Smartflow.BussinessService
{
    public interface IRepository<T>
    {
        void Insert(T entity);

        void Update(T entity);

        void Delete(T entity);

        void Delete(long id);

        void Delete(Expression<Func<T, bool>> expression);

        T Get(long id);

        T Get(Expression<Func<T,bool>> expression);

        List<T> Query();

        List<T> Query(Expression<Func<T, bool>> expression);
    }
}
