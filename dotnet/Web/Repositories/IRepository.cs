using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Objects;
using System.Collections.Generic;

namespace Mvc.Infrastructure.Repositories
{
    interface IRepository<TEntity, TKey>
    {
        IQueryable<TEntity> FindAll();
        TEntity FindOne(Expression<Func<TEntity, bool>> filter);
        TEntity FindFirst(Expression<Func<TEntity, bool>> filter);
        TEntity Insert(TEntity entity);
        TEntity InsertWoSave(TEntity entity);
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter);
        TEntity Delete(TKey id);
        TEntity DeleteWoSave(TKey id);
        TEntity Save(TEntity entity);
        TEntity SaveWoSaveChanges(TEntity entity);
        void SaveWoSaveChanges(IEnumerable<TEntity> entities);
        void Save(IEnumerable<TEntity> entities);
        int SaveChanges(SaveOptions options);
        int SaveChanges();
        TEntity Get(TKey id);
    }
}
