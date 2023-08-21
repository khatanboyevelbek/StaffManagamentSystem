using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StaffManagementSystem.Api.Domain.Entities;
using StaffManagementSystem.Api.Infrastructure.IRepositories;

namespace StaffManagementSystem.Api.Infrastructure.Repositories
{
    public class VacationRepository : IVacationRepository
    {
        private readonly AppDbContext context;
        private readonly DbSet<Vacation> dbSet;

        public VacationRepository(AppDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<Vacation>();
        }

        public async Task DeleteAsync(Vacation vacation)
        {
            EntityEntry<Vacation> deletedEntity = this.dbSet.Remove(vacation);
            await this.context.SaveChangesAsync();
        }

        public IQueryable<Vacation> GetAll() => this.dbSet;

        public async Task<Vacation> GetByIdAsync(Guid id) => await this.dbSet.FindAsync(id);

        public IQueryable<Vacation> GetSelected(Expression<Func<Vacation, bool>> expression) =>
            this.dbSet.Where(expression);

        public async Task<Vacation> InsertAsync(Vacation vacation)
        {
            var insertedEntity = await this.dbSet.AddAsync(vacation);
            await this.context.SaveChangesAsync();

            return insertedEntity.Entity;
        }

        public async Task<Vacation> UpdateAsync(Vacation vacation)
        {
            var updatedEntity = this.dbSet.Update(vacation);
            await this.context.SaveChangesAsync();

            return updatedEntity.Entity;
        }
    }
}
