using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StaffManagementSystem.Api.Domain.Entities;
using StaffManagementSystem.Api.Infrastructure.IRepositories;

namespace StaffManagementSystem.Api.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext context;
        private readonly DbSet<Admin> dbSet;

        public AdminRepository(AppDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<Admin>();
        }

        public async Task DeleteAsync(Admin admin)
        {
            EntityEntry<Admin> deletedEntity = this.dbSet.Remove(admin);
            await this.context.SaveChangesAsync();
        }

        public async Task<Admin> GetAdminByEmailAsync(string email) =>
            await this.dbSet.FirstOrDefaultAsync(a => a.Email == email);

        public IQueryable<Admin> GetAll() => this.dbSet;

        public async Task<Admin> GetByIdAsync(Guid id) => await this.dbSet.FindAsync(id);

        public IQueryable<Admin> GetSelected(Expression<Func<Admin, bool>> expression) =>
            this.dbSet.Where(expression);

        public async Task<Admin> InsertAsync(Admin admin)
        {
            var insertedEntity = await this.dbSet.AddAsync(admin);
            await this.context.SaveChangesAsync();

            return insertedEntity.Entity;
        }

        public async Task<Admin> UpdateAsync(Admin admin)
        {
            var updatedEntity = this.dbSet.Update(admin);
            await this.context.SaveChangesAsync();

            return updatedEntity.Entity;
        }
    }
}
