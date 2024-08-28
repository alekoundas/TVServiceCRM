using TVServiceCRM.Server.Business.IRepository;
using TVServiceCRM.Server.Business.Repository;
using TVServiceCRM.Server.DataAccess;
using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.Business.IServices
{
    public interface IDataService : IDisposable
    {
        ApiDbContext Query { get; }

        GenericRepository<ContactInformation> ContactInformations { get; }
        GenericRepository<Customer> Customers { get;}
        GenericRepository<Ticket> Tickets { get;}
        GenericRepository<User> Users { get; }



        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
