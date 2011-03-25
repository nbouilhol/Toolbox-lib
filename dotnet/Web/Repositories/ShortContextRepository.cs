using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Linq.Expressions;
using System.Data;
using System.Globalization;
using SuiviCRA.Outlook.Domain;
using System.Data.Objects.DataClasses;

namespace Mvc.Infrastructure.Repositories
{
    public abstract class ShortContextRepository<TContext, TEntity, TKey> : ShortContextBaseRepository<TContext, TEntity, TKey>, IShortContextRepository<TContext, TEntity, TKey> 
        where TEntity : EntityObject
        where TContext : ObjectContext
    {
        public ShortContextRepository() : base() { }

        protected abstract ObjectSet<TEntity> EntitySet(TContext context);

        protected override ObjectQuery<TEntity> EntityQuery(TContext context) { return EntitySet(context); }

        protected override void AddObject(TContext context, TEntity entity)
        {
            EntitySet(context).AddObject(entity);
        }

        protected override void ApplyCurrentValues(TContext context, TEntity entity)
        {
            EntitySet(context).ApplyCurrentValues(entity);
        }

        protected override void DeleteObject(TContext context, TEntity entity)
        {
            EntitySet(context).DeleteObject(entity);
        }

        protected override void Attach(TContext context, TEntity entity)
        {
            EntitySet(context).Attach(entity);
        }

        protected override void Detach(TContext context, TEntity entity)
        {
            EntitySet(context).Detach(entity);
        }
    }
}
