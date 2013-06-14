using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;

namespace Mvc.Infrastructure.Repositories
{
    public abstract class Repository<TEntity, TKey> : BaseRepository<TEntity, TKey>, IRepository<TEntity, TKey>
        where TEntity : EntityObject
    {
        public Repository(Lazy<ObjectContext> context)
            : base(context)
        {
        }

        protected abstract ObjectSet<TEntity> EntitySet { get; }

        protected override ObjectQuery<TEntity> EntityQuery
        {
            get { return EntitySet; }
        }

        protected override void AddObject(TEntity entity)
        {
            EntitySet.AddObject(entity);
        }

        protected override void ApplyCurrentValues(TEntity entity)
        {
            EntitySet.ApplyCurrentValues(entity);
        }

        protected override void DeleteObject(TEntity entity)
        {
            EntitySet.DeleteObject(entity);
        }

        protected override void Attach(TEntity entity)
        {
            EntitySet.Attach(entity);
        }

        protected override void Detach(TEntity entity)
        {
            EntitySet.Detach(entity);
        }
    }
}