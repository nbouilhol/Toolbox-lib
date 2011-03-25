using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Objects;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;

namespace Mvc.Outlook.Infrastructure.Repositories
{
    interface IShortContextRepository<TContext, TEntity, TKey>
        where TContext : ObjectContext
        where TEntity : EntityObject
    {
        IQueryable<TEntity> FindAll(TContext context);
        TEntity FindOne(TContext context, Expression<Func<TEntity, bool>> filter);
        TEntity FindFirst(TContext context, Expression<Func<TEntity, bool>> filter);
        TEntity Insert(TContext context, TEntity entity);
        TEntity InsertWoSave(TContext context, TEntity entity);
        IQueryable<TEntity> Query(TContext context, Expression<Func<TEntity, bool>> filter);
        TEntity Delete(TContext context, TKey id);
        TEntity DeleteWoSave(TContext context, TKey id);
        TEntity Save(TContext context, TEntity entity);
        TEntity SaveWoSaveChanges(TContext context, TEntity entity);
        void SaveWoSaveChanges(TContext context, IEnumerable<TEntity> entities);
        void Save(TContext context, IEnumerable<TEntity> entities);
        int SaveChanges(TContext context, SaveOptions options);
        int SaveChanges(TContext context);
        TEntity Get(TContext context, TKey id);
    }
}
