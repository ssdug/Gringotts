using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;

namespace Wiz.Gringotts.UIWeb.Data
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();

        void BeginTransaction();
        void CloseTransaction();
        void CloseTransaction(Exception exception);
    }

    public abstract class TransactionCoordinatorDbContext : ApplicationDbContext, IUnitOfWork
    {
        public ILogger Logger { get; set; }

        private DbContextTransaction currentTransaction;

        protected TransactionCoordinatorDbContext()
        { }
        protected TransactionCoordinatorDbContext(DbConnection connection) : base(connection)
        { }

        void IUnitOfWork.BeginTransaction()
        {
            try
            {
                if (currentTransaction != null)
                {
                    return;
                }

                currentTransaction = Database.BeginTransaction(IsolationLevel.Snapshot);
                Logger.Trace("Transaction Started");
            }
            catch (Exception e)
            {
                if (Logger != null)
                    Logger.Error("Unable to begin transaction.", e);
                throw;
            }
        }

        void IUnitOfWork.CloseTransaction()
        {
          (this as IUnitOfWork).CloseTransaction(exception: null);
        }

        void IUnitOfWork.CloseTransaction(Exception exception)
        {
            try
            {
                if (currentTransaction != null && exception != null)
                {
                    if (Logger != null)
                        Logger.Error("Rolling back transaction.", exception);
                    currentTransaction.Rollback();
                    return;
                }

                SaveChanges();

                if (currentTransaction != null)
                {
                    currentTransaction.Commit();
                    Logger.Trace("Transaction Committed");
                }
            }
            catch (Exception e)
            {
                if (Logger != null)
                    Logger.Error("Unable to commit transaction.", e);

                if (currentTransaction != null && currentTransaction.UnderlyingTransaction.Connection != null)
                {
                    if (Logger != null)
                        Logger.Error("Rolling back transaction.", exception);
                    currentTransaction.Rollback();
                }

                throw;
            }
            finally
            {
                if (currentTransaction != null)
                {
                    currentTransaction.Dispose();
                    if (Logger != null)
                        Logger.Trace("Transaction Disposed");
                    currentTransaction = null;
                }
            }
        }
    }
}