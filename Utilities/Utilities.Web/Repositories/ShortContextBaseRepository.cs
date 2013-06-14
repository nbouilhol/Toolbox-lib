using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;

namespace Mvc.Infrastructure.Repositories
{
    public abstract class ShortContextBaseRepository<TContext, TEntity, TKey>
        where TEntity : EntityObject
        where TContext : ObjectContext
    {
        public abstract Func<TContext> GetContext { get; }

        protected abstract InvalidOperationException HasNoResult { get; }

        protected abstract Func<TEntity, InvalidOperationException> IsNotFound { get; }

        protected abstract Func<TEntity, InvalidOperationException> IsInvalid { get; }
        protected abstract ObjectQuery<TEntity> EntityQuery(TContext context);

        public virtual TEntity FindFirst(TContext context, Expression<Func<TEntity, bool>> filter)
        {
            return EntityQuery(context).FirstOrDefault(filter);
        }

        public virtual TEntity FindOne(TContext context, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                return EntityQuery(context).Single(filter);
            }
            catch (InvalidOperationException)
            {
                throw HasNoResult;
            }
        }

        public virtual IQueryable<TEntity> FindAll(TContext context)
        {
            return EntityQuery(context);
        }

        public virtual IQueryable<TEntity> Query(TContext context, Expression<Func<TEntity, bool>> filter)
        {
            return EntityQuery(context).Where(filter);
        }

        public virtual TEntity Delete(TContext context, TKey id)
        {
            TEntity entity = DeleteWoSave(context, id);
            SaveChanges(context, entity);

            return entity;
        }

        public virtual TEntity DeleteWoSave(TContext context, TKey id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            TEntity entity = Get(context, id);
            DeleteObject(context, entity);

            return entity;
        }

        public virtual TEntity DeleteWoSave(TContext context, TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            DeleteObject(context, entity);

            return entity;
        }

        public virtual void DeleteWoSave(TContext context, IEnumerable<TEntity> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException("entities");

            foreach (TEntity entity in entities)
            {
                try
                {
                    DeleteWoSave(context, entity);
                }
                catch (InvalidOperationException)
                {
                    Get(context, entity);
                    try
                    {
                        DeleteWoSave(context, entity);
                    }
                    catch (InvalidOperationException)
                    {
                        throw IsNotFound(entity);
                    }
                }
            }
        }

        public virtual TEntity Save(TContext context, TEntity entity)
        {
            SaveWoSaveChanges(context, entity);
            SaveChanges(context, entity);

            return entity;
        }

        public virtual TEntity SaveWoSaveChanges(TContext context, TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.EntityKey == null || entity.EntityKey.IsTemporary)
                InsertWoSave(context, entity);
            else
                ApplyCurrentValues(context, entity);
            return entity;
        }

        public virtual void SaveWoSaveChanges(TContext context, IEnumerable<TEntity> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException("entities");

            foreach (TEntity entity in entities)
            {
                try
                {
                    SaveWoSaveChanges(context, entity);
                }
                catch (InvalidOperationException)
                {
                    Get(context, entity);
                    try
                    {
                        SaveWoSaveChanges(context, entity);
                    }
                    catch (InvalidOperationException)
                    {
                        throw IsInvalid(entity);
                    }
                }
            }
        }

        public virtual TEntity Insert(TContext context, TEntity entity)
        {
            InsertWoSave(context, entity);
            SaveChanges(context, entity);

            return entity;
        }

        public virtual TEntity InsertWoSave(TContext context, TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            AddObject(context, entity);

            return entity;
        }

        private int SaveChanges(TContext context, object entity)
        {
            try
            {
                return context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                context.Refresh(RefreshMode.StoreWins, entity);
                return context.SaveChanges();
            }
        }

        public virtual int SaveChanges(TContext context, SaveOptions options)
        {
            return context.SaveChanges(options);
        }

        public virtual int SaveChanges(TContext context)
        {
            return context.SaveChanges();
        }

        public virtual void Save(TContext context, IEnumerable<TEntity> entities)
        {
            SaveWoSaveChanges(context, entities);
            SaveChanges(context, entities);
        }

        public virtual TEntity Get(TContext context, TKey id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            return EntityQuery(context).SingleOrDefault(GetFromIdPredicate(id));
        }

        protected virtual TEntity Get(TContext context, TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            return EntityQuery(context).SingleOrDefault(GetFromEntityPredicate(entity));
        }

        public abstract Expression<Func<TEntity, bool>> GetFromIdPredicate(TKey id);

        public abstract Expression<Func<TEntity, bool>> GetFromEntityPredicate(TEntity entity);

        protected abstract void AddObject(TContext context, TEntity entity);

        protected abstract void ApplyCurrentValues(TContext context, TEntity entity);

        protected abstract void DeleteObject(TContext context, TEntity entity);

        protected abstract void Attach(TContext context, TEntity entity);

        protected abstract void Detach(TContext context, TEntity entity);
    }
}