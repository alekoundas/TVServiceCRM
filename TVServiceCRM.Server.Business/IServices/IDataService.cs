﻿using Microsoft.EntityFrameworkCore;
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

        GenericRepository<ContactInformation> ContactInformations { get; }
        GenericRepository<Customer> Customers { get; }
        GenericRepository<Ticket> Tickets { get; }
        GenericRepository<ApplicationUser> Users { get; }



        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
