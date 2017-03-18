using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Wiz.Gringotts.UIWeb.Data.Interceptors
{
    public interface IInterceptor
    {
        void Before(InterceptionContext context);
        void After(InterceptionContext context);
    }

    public abstract class TypeInterceptor : IInterceptor
    {
        private readonly System.Type _targetType;
        public Type TargetType { get { return _targetType; } }

        protected TypeInterceptor(System.Type targetType)
        {
            this._targetType = targetType;
        }

        protected virtual bool IsTargetEntity(DbEntityEntry item, EntityState state)
        {
            //
            // can't use the state from DbEntityEntry because if it is after a delete
            // the state will be set to Detached.
            //
            return state != EntityState.Detached &&
                   this.TargetType.IsInstanceOfType(item.Entity);
        }

        public void Before(InterceptionContext context)
        {
            var states = new[] { EntityState.Added, EntityState.Modified, EntityState.Deleted, };
            foreach (var state in states)
            {
                var entries = context.EntriesByState[state];
                foreach (var entry in entries)
                {
                    if (IsTargetEntity(entry, state))
                    {
                        OnBefore(entry, state, context);
                    }
                }
            }

        }

        public void After(InterceptionContext context)
        {
            var states = new[] { EntityState.Added, EntityState.Modified, EntityState.Deleted, };
            foreach (var state in states)
            {
                var entries = context.EntriesByState[state];
                foreach (var entry in entries)
                {
                    if (IsTargetEntity(entry, state))
                    {
                        OnAfter(entry, state, context);
                    }
                }
            }
        }

        protected virtual void OnBefore(DbEntityEntry item, EntityState state, InterceptionContext context)
        { }
        protected virtual void OnAfter(DbEntityEntry item, EntityState state, InterceptionContext context)
        { }
    }

    public class ChangeInterceptor<T> : TypeInterceptor
    {
        protected override void OnBefore(DbEntityEntry item, EntityState state, InterceptionContext context)
        {
            T tItem = (T)item.Entity;
            switch (state)
            {
                case EntityState.Added:
                    OnBeforeInsert(item, tItem, context);
                    break;
                case EntityState.Deleted:
                    OnBeforeDelete(item, tItem, context);
                    break;
                case EntityState.Modified:
                    OnBeforeUpdate(item, tItem, context);
                    break;
            }
        }

        protected override void OnAfter(DbEntityEntry item, EntityState state, InterceptionContext context)
        {
            T tItem = (T)item.Entity;
            switch (state)
            {
                case EntityState.Added:
                    this.OnAfterInsert(item, tItem, context);
                    break;
                case EntityState.Deleted:
                    this.OnAfterDelete(item, tItem, context);
                    break;
                case EntityState.Modified:
                    this.OnAfterUpdate(item, tItem, context);
                    break;
            }
        }

        protected virtual void OnBeforeInsert(DbEntityEntry entry, T item, InterceptionContext context)
        { }

        protected virtual void OnAfterInsert(DbEntityEntry entry, T item, InterceptionContext context)
        { }

        protected virtual void OnBeforeUpdate(DbEntityEntry entry, T item, InterceptionContext context)
        { }

        protected virtual void OnAfterUpdate(DbEntityEntry entry, T item, InterceptionContext context)
        { }

        protected virtual void OnBeforeDelete(DbEntityEntry entry, T item, InterceptionContext context)
        { }

        protected virtual void OnAfterDelete(DbEntityEntry entry, T item, InterceptionContext context)
        { }

        protected ChangeInterceptor()
            : base(typeof(T))
        {

        }
    }
}