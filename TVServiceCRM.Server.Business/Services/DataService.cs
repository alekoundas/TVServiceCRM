using Microsoft.EntityFrameworkCore;
using TVServiceCRM.Server.Business.IServices;
using TVServiceCRM.Server.Business.Repository;
using TVServiceCRM.Server.DataAccess;
using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.Business.Services
{
    /// <summary>
    /// Unit of work design pattern.
    /// </summary>
    public class DataService : IDataService, IDisposable
    {
        public ApiDbContext Query { get; }
        private ApiDbContext _dbContext { get; }


        private GenericRepository<Maker> _makers;
        private GenericRepository<Ticket> _tickets;
        private GenericRepository<Customer> _customers;
        private GenericRepository<MakerModel> _makerModels;
        private GenericRepository<ApplicationUser> _applicationUsers;
        private GenericRepository<ContactInformation> _contactInformations;

        public DataService(ApiDbContext apiDbContext)
        {
            Query = apiDbContext;
            _dbContext = apiDbContext;
        }


        public GenericRepository<ContactInformation> ContactInformations
        {
            get
            {
                if (_contactInformations == null)
                    _contactInformations = new GenericRepository<ContactInformation>(_dbContext);

                return _contactInformations;
            }
        }
        public GenericRepository<Customer> Customers
        {
            get
            {
                if (_customers == null)
                    _customers = new GenericRepository<Customer>(_dbContext);

                return _customers;
            }
        }
        public GenericRepository<Ticket> Tickets
        {
            get
            {
                if (_tickets == null)
                    _tickets = new GenericRepository<Ticket>(_dbContext);

                return _tickets;
            }
        }
        public GenericRepository<ApplicationUser> Users
        {
            get
            {
                if (_applicationUsers == null)
                    _applicationUsers = new GenericRepository<ApplicationUser>(_dbContext);

                return _applicationUsers;
            }
        }
        public GenericRepository<Maker> Makers
        {
            get
            {
                if (_makers == null)
                    _makers = new GenericRepository<Maker>(_dbContext);

                return _makers;
            }
        }
        public GenericRepository<MakerModel> MakerModels
        {
            get
            {
                if (_makerModels == null)
                    _makerModels = new GenericRepository<MakerModel>(_dbContext);

                return _makerModels;
            }
        }


        public GenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class
        {
            return new GenericRepository<TEntity>(_dbContext);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }


        // Dispose.
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
