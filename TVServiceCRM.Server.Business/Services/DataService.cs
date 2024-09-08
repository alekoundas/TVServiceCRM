using TVServiceCRM.Server.Business.IRepository;
using TVServiceCRM.Server.Business.IServices;
using TVServiceCRM.Server.Business.Repository;
using TVServiceCRM.Server.DataAccess;
using Microsoft.EntityFrameworkCore;
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


        private GenericRepository<ContactInformation> _contactInformations;
        private GenericRepository<Customer> _customers;
        private GenericRepository<Ticket> _tickets;
        private GenericRepository<ApplicationUser> _users;

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
                if (_tickets == null)
                    _users = new GenericRepository<ApplicationUser>(_dbContext);

                return _users;
            }
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
