using Microsoft.EntityFrameworkCore;
using TVServiceCRM.Server.Business.IRepository;
using TVServiceCRM.Server.Business.Repository;
using TVServiceCRM.Server.DataAccess;
using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.Business.IServices
{
    public interface IDataService : IDisposable 
    {
        ApiDbContext Query { get; }
        GenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;

        GenericRepository<Maker> Makers { get; }
        GenericRepository<Ticket> Tickets { get; }
        GenericRepository<Customer> Customers { get; }
        GenericRepository<ApplicationUser> Users { get; }
        GenericRepository<MakerModel> MakerModels { get; }
        GenericRepository<ContactInformation> ContactInformations { get; }



        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
