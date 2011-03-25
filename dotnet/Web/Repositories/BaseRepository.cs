﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Linq.Expressions;
using System.Data;
using System.Globalization;
using SuiviCRA.Domain;
using System.Data.Objects.DataClasses;

namespace Mvc.Infrastructure.Repositories
{
    public abstract class BaseRepository<TEntity, TKey> where TEntity : EntityObject
    {
        protected Entities Context { get { return context.Value; } }
        private Lazy<Entities> context { get; set; }
        protected abstract ObjectQuery<TEntity> EntityQuery { get; }
        protected abstract InvalidOperationException HasNoResult { get; }
        protected abstract Func<TEntity, InvalidOperationException> IsNotFound { get; }
        protected abstract Func<TEntity, InvalidOperationException> IsInvalid { get; }

        public BaseRepository(Lazy<Entities> context)
        {
            this.context = context;
        }

        public virtual TEntity FindFirst(Expression<Func<TEntity, bool>> filter)
        {
            return EntityQuery.FirstOrDefault(filter);
        }

        public virtual TEntity FindOne(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                return EntityQuery.Single(filter);
            }
            catch (InvalidOperationException)
            {
                throw HasNoResult;
            }
        }

        public virtual IQueryable<TEntity> FindAll()
        {
            return EntityQuery;
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter)
        {
            return EntityQuery.Where(filter);
        }

        public virtual TEntity Delete(TKey id)
        {
            TEntity entity = DeleteWoSave(id);
            SaveChanges(entity);

            return entity;
        }

        public virtual TEntity DeleteWoSave(TKey id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            TEntity entity = Get(id);
            DeleteObject(entity);

            return entity;
        }

        public virtual TEntity DeleteWoSave(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            DeleteObject(entity);

            return entity;
        }

        public virtual void DeleteWoSave(IEnumerable<TEntity> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
            {
                try
                {
                    DeleteWoSave(entity);
                }
                catch (InvalidOperationException)
                {
                    Get(entity);
                    try
                    {
                        DeleteWoSave(entity);
                    }
                    catch (InvalidOperationException)
                    {
                        throw IsNotFound(entity);
                    }
                }
            }
        }

        public virtual TEntity Save(TEntity entity)
        {
            SaveWoSaveChanges(entity);
            SaveChanges(entity);

            return entity;
        }

        public virtual TEntity SaveWoSaveChanges(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.EntityKey == null || entity.EntityKey.IsTemporary)
                InsertWoSave(entity);
            else
                ApplyCurrentValues(entity);
            return entity;
        }

        public virtual void SaveWoSaveChanges(IEnumerable<TEntity> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
            {
                try
                {
                    SaveWoSaveChanges(entity);
                }
                catch (InvalidOperationException)
                {
                    Get(entity);
                    try
                    {
                        SaveWoSaveChanges(entity);
                    }
                    catch (InvalidOperationException)
                    {
                        throw IsInvalid(entity);
                    }
                }
            }
        }

        public virtual TEntity Insert(TEntity entity)
        {
            InsertWoSave(entity);
            SaveChanges(entity);

            return entity;
        }

        public virtual TEntity InsertWoSave(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            AddObject(entity);

            return entity;
        }

        private int SaveChanges(object entity)
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                Context.Refresh(RefreshMode.StoreWins, entity);
                return Context.SaveChanges();
            }
        }

        public virtual int SaveChanges(SaveOptions options)
        {
            return Context.SaveChanges(options);
        }

        public virtual int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public virtual void Save(IEnumerable<TEntity> entities)
        {
            SaveWoSaveChanges(entities);
            SaveChanges(entities);
        }

        public abstract TEntity Get(TKey id);
        protected abstract TEntity Get(TEntity entity);
        protected abstract void AddObject(TEntity entity);
        protected abstract void ApplyCurrentValues(TEntity entity);
        protected abstract void DeleteObject(TEntity entity);
        protected abstract void Attach(TEntity entity);
        protected abstract void Detach(TEntity entity);
    }
}
