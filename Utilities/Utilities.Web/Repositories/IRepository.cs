using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace Mvc.Infrastructure.Repositories
{
    internal interface IRepository<TEntity, TKey>
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