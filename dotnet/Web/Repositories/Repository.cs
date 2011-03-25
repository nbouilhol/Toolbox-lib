using System;
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
    public abstract class Repository<TEntity, TKey> : BaseRepository<TEntity, TKey>, IRepository<TEntity, TKey> where TEntity : EntityObject
    {
        public Repository(Lazy<Entities> context) : base(context) { }
        protected abstract ObjectSet<TEntity> EntitySet { get; }

        protected override ObjectQuery<TEntity> EntityQuery { get { return EntitySet; } }

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
